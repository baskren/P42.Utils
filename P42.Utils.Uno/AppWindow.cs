using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
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

        public static double StatusBarHeight(FrameworkElement element)
        {
#if __ANDROID__
            if (element is Android.Views.View view)
            {

                var rect = new Android.Graphics.Rect();
                var window = ((Android.App.Activity)view.Context).Window;
                window.DecorView.GetWindowVisibleDisplayFrame(rect);

                using var displayMetrics = new Android.Util.DisplayMetrics();

                using var service = view.Context.GetSystemService(Android.Content.Context.WindowService);
                using var windowManager = service?.JavaCast<Android.Views.IWindowManager>();

                windowManager?.DefaultDisplay?.GetRealMetrics(displayMetrics);

                var statusBarHeight = rect.Top / displayMetrics?.Density ?? 1;
                return statusBarHeight;
            }
#endif
            return 0;
        }

        public static Thickness SafeArea(FrameworkElement element)
        {
            var result = new Thickness();
            var winBounds = Platform.Window.Bounds;
            var pageBounds = Platform.Window.Content.GetBounds();

#if !NET7_0_WINDOWS10_0_19041_0

            result.Left = pageBounds.Left - winBounds.Left;
            result.Top = pageBounds.Top - winBounds.Top;
            result.Right = winBounds.Right - pageBounds.Right;
            result.Bottom = winBounds.Bottom - pageBounds.Bottom;
#endif

#if __iOS__
#endif
            return result;
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

                windowManager?.DefaultDisplay?.GetRealMetrics(displayMetrics);

                return displayMetrics?.Density ?? 1;
            }
#elif __IOS__
            return UIKit.UIScreen.MainScreen.Scale;
#endif
            return 1;
        }

    }
}
