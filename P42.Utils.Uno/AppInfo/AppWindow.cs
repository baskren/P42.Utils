using System;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
#if __ANDROID__
using Android.Runtime;
#endif

namespace P42.Utils.Uno;

public static class AppWindow
{
    /// <summary>
    /// Size of AppWindow
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Size Size(FrameworkElement element)
    {
        // Doesn't work in Uno because we're using this before Microsoft.UI.Xaml.Window.Current is null
        //var windowWidth = ((Frame)Microsoft.UI.Xaml.Window.Current.Content).ActualWidth;
        //var windowHeight = ((Frame)Microsoft.UI.Xaml.Window.Current.Content).ActualHeight;

        var windowWidth = Platform.MainWindow.Bounds.Width ;
        var windowHeight = Platform.MainWindow.Bounds.Height ;
        return new Size(windowWidth, windowHeight);
    }

    /// <summary>
    /// Current Page
    /// </summary>
    public static Page? CurrentPage
        => CurrentFrame?.Content as Page;

    /// <summary>
    /// Current Frame
    /// </summary>
    public static Frame? CurrentFrame
        => Platform.MainWindow.Content as Frame;

    /// <summary>
    /// Height of the Status Bar (applicable in Android)
    /// </summary>
    /// <returns></returns>
    [Obsolete("Use SafeMargin, instead", true)]
    public static double StatusBarHeight()
        => throw new NotSupportedException();

    /// <summary>
    /// Application Window Safe Margin : Importantly, the correct value lags behind the
    /// Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().OrientationChanged event.  Adding a Task.Delay(500)
    /// before calling this method seems to work
    /// </summary>
    /// <param name="desiredMargin"></param>
    /// <returns></returns>
    public static Thickness SafeMargin(Thickness desiredMargin = new ())
    {
#if HAS_UNO
        var windowBounds = Platform.MainWindow.Bounds;
            
        var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;
        return new Thickness(
            Math.Max(visibleBounds.Left - windowBounds.Left, desiredMargin.Left),
            Math.Max(visibleBounds.Top - windowBounds.Top, desiredMargin.Top),
            Math.Max(windowBounds.Right - visibleBounds.Right, desiredMargin.Right),
            Math.Max(windowBounds.Bottom - visibleBounds.Bottom, desiredMargin.Bottom)
        );
#else
        return default;
#endif
    }

    [Obsolete("Use .ResolutionScale or .RawPixelsPerViewPixel in Windows.Graphics.Display.DisplayInformation.GetForCurrentView(), instead", true)]
    public static double DisplayScale(FrameworkElement _)
        => throw new NotImplementedException();
    

}
