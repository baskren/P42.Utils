using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.ViewManagement;
#if __ANDROID__
using Android.Runtime;
#endif

namespace P42.Utils.Uno
{
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

            if (Platform.Window.Content is Frame frame)
            {
                if (frame is Android.Views.View view)
                {

                    var rect = new Android.Graphics.Rect();
                    var window = ((Android.App.Activity)view.Context).Window;
                    window.DecorView.GetWindowVisibleDisplayFrame(rect);

                    using var displayMetrics = new Android.Util.DisplayMetrics();

                    using var service = view.Context.GetSystemService(Android.Content.Context.WindowService);
                    using var windowManager = service?.JavaCast<Android.Views.IWindowManager>();

#pragma warning disable CA1422 // Validate platform compatibility
                    windowManager?.DefaultDisplay?.GetRealMetrics(displayMetrics);
#pragma warning restore CA1422 // Validate platform compatibility

                    var statusBarHeight = rect.Top / displayMetrics?.Density ?? 1;
                    return statusBarHeight;
                }
            }
#endif
            return 0;
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
            if (element is Android.Views.View view)
            {
                //var window = ((Android.App.Activity)view.Context).Window;
                //window.DecorView.GetWindowVisibleDisplayFrame(rect);

                using var displayMetrics = new Android.Util.DisplayMetrics();

                using var service = view.Context.GetSystemService(Android.Content.Context.WindowService);
                using var windowManager = service?.JavaCast<Android.Views.IWindowManager>();

                //if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.S)
                {
#pragma warning disable CA1422 // Validate platform compatibility
                    windowManager?.DefaultDisplay?.GetRealMetrics(displayMetrics);
#pragma warning restore CA1422 // Validate platform compatibility
                    return displayMetrics?.Density ?? 1;
                }

                
                
            }
            return 1;
#elif __IOS__
            return UIKit.UIScreen.MainScreen.Scale;
#else
            return 1;
#endif
        }

    }
}
