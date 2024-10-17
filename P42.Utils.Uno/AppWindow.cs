using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
#if __ANDROID__
using Android.App;
using Android.OS;
using Android.Runtime;
#endif

namespace P42.Utils.Uno;

public static class AppWindow
{
    /// <summary>
    /// Current size of App's main window
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Size Size(FrameworkElement element)
    {
        // Doesn't work in Uno
        //var windowWidth = ((Frame)Microsoft.UI.Xaml.Window.Current.Content).ActualWidth;
        //var windowHeight = ((Frame)Microsoft.UI.Xaml.Window.Current.Content).ActualHeight;
        var window = Platform.Window;
        return window?.Size() ?? new Size(1920,1080);
    }
    
    /// <summary>
    /// Size of Window
    /// </summary>
    /// <param name="window"></param>
    /// <returns>size</returns>
    public static Size Size(this Window window)
        => new Size(window.Bounds.Width, window.Bounds.Height);

    /// <summary>
    /// Current Page in App's main window
    /// </summary>
    public static Page? CurrentPage
        => (Page?)CurrentFrame?.Content;

    /// <summary>
    /// Current Frame in App's main window
    /// </summary>
    public static Frame? CurrentFrame
        => (Frame?)Platform.Window?.Content;

    /// <summary>
    /// Height of StatusBar
    /// </summary>
    /// <returns>Android: StatusBar height; 0 otherwise</returns>
    public static double StatusBarHeight()
    {
#if __ANDROID__
        if (Platform.Window?.Content is not (Frame frame and Android.Views.View view))
            return 0;

        var rect = new Android.Graphics.Rect();
        if (((Activity?)view.Context)?.Window is not {} window)
            return 0;
        
        window.DecorView.GetWindowVisibleDisplayFrame(rect);
        var scale = DisplayScale(frame);
        return rect.Top / scale;
#else
        return 0;
#endif
    }

    /// <summary>
    /// Margins for visible portion of app's main window 
    /// </summary>
    /// <param name="element"></param>
    /// <returns>Thickness</returns>
    public static Thickness VisibleMargin()
    {
#if HAS_UNO
        if (Platform.Window?.Bounds is not {} windowBounds)
            return default;
            
        var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;
        return new Thickness(
            visibleBounds.Left - windowBounds.Left,
            visibleBounds.Top - windowBounds.Top,
            windowBounds.Right - visibleBounds.Right,
            windowBounds.Bottom - visibleBounds.Bottom
        );
#else
        return default(Thickness);
#endif
    }

    public static double DisplayScale(UIElement element)
    {
#if __ANDROID__
        if (element is not Android.Views.View view)
            return  1;
        
        using var service = view.Context?.GetSystemService(Android.Content.Context.WindowService);
        using var windowService = service?.JavaCast<Android.Views.IWindowManager>();
        if (windowService is null)
            return 1;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.S)
        {
            var metrics = windowService.CurrentWindowMetrics;
            return metrics.Density;
        }

        using var displayMetrics = new Android.Util.DisplayMetrics();
#pragma warning disable CA1422
        windowService.DefaultDisplay?.GetRealMetrics(displayMetrics);
#pragma warning restore CA1422
        return displayMetrics.Density;
        
#elif __IOS__
        return UIKit.UIScreen.MainScreen.Scale;
#else
        return 1;
#endif
    }

    
    
}
