using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.UI.ViewManagement;
#if __ANDROID__
using Android.Runtime;
#elif __IOS__
using UIKit;
#endif

namespace P42.Utils.Uno;

public static class AppWindow
{
    public static Size Size(FrameworkElement element)
    {
        // Doens't work in Uno
        //var windowWidth = ((Frame)Microsoft.UI.Xaml.Window.Current.Content).ActualWidth;
        //var windowHeight = ((Frame)Microsoft.UI.Xaml.Window.Current.Content).ActualHeight;

        var windowWidth = Platform.Window?.Bounds.Width ?? 1920;
        var windowHeight = Platform.Window?.Bounds.Height ?? 1080;
        return new Size(windowWidth, windowHeight);
    }

    public static Page CurrentPage
        => (Page)CurrentFrame?.Content;

    public static Frame CurrentFrame
        => (Frame)Platform.Window.Content;

    public static double StatusBarHeight()
    {
#if __ANDROID__


        var rect = new Android.Graphics.Rect();

        var context = global::Uno.UI.ContextHelper.Current;
        var window = ((Android.App.Activity)context).Window;

        window.DecorView.GetWindowVisibleDisplayFrame(rect);

        using var displayMetrics = new Android.Util.DisplayMetrics();

        using var service = context.GetSystemService(Android.Content.Context.WindowService);
        using var windowManager = service?.JavaCast<Android.Views.IWindowManager>();

#pragma warning disable CA1422 // Validate platform compatibility
        windowManager?.DefaultDisplay?.GetRealMetrics(displayMetrics);
#pragma warning restore CA1422 // Validate platform compatibility

        var statusBarHeight = rect.Top / displayMetrics?.Density ?? 1;
        return statusBarHeight;

#else
        return 0;
#endif
    }

    public static Thickness SafeMargin(FrameworkElement element)
    {
#if HAS_UNO
        var windowBounds = P42.Utils.Uno.Platform.Window.Bounds;
        
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

    public static double DisplayScale(FrameworkElement element)
    {
#if __ANDROID__

        using var displayMetrics = new Android.Util.DisplayMetrics();

        var context = global::Uno.UI.ContextHelper.Current;
        using var service = context.GetSystemService(Android.Content.Context.WindowService);
        using var windowManager = service?.JavaCast<Android.Views.IWindowManager>();

#pragma warning disable CA1422 // Validate platform compatibility
        windowManager?.DefaultDisplay?.GetRealMetrics(displayMetrics);
#pragma warning restore CA1422 // Validate platform compatibility
        return displayMetrics?.Density ?? 1;

#elif __IOS__
        return UIKit.UIScreen.MainScreen.Scale;
#else
        return 1;
#endif
    }

#if __IOS__
    public static UIViewController GetTopUIViewController()
    {
        var window = UIApplication.SharedApplication
            .ConnectedScenes
            .OfType<UIWindowScene>()
            .SelectMany(s => s.Windows)
            .FirstOrDefault(w => w.IsKeyWindow);

        var root = window?.RootViewController;
        while (root?.PresentedViewController != null)
        {
            root = root.PresentedViewController;
        }

        return root;
    }

    public static UIView GetTopUIView()
    {
        var controller = GetTopUIViewController();
        return controller?.View;
    }
#endif

}
