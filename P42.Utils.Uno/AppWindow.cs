using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
            //var windowWidth = ((Frame)Windows.UI.Xaml.Window.Current.Content).ActualWidth;
            //var windowHeight = ((Frame)Windows.UI.Xaml.Window.Current.Content).ActualHeight;

            var windowWidth = Windows.UI.Xaml.Window.Current.Bounds.Width;
            var windowHeight = Windows.UI.Xaml.Window.Current.Bounds.Height;
            return new Size(windowWidth, windowHeight);
        }

        public static Page CurrentPage
            => (Page)CurrentFrame?.Content;

        public static Frame CurrentFrame
            => (Frame)Windows.UI.Xaml.Window.Current.Content;

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
            var winBounds = Windows.UI.Xaml.Window.Current.Bounds;
            var pageBounds = Windows.UI.Xaml.Window.Current.Content.GetBounds();

#if !WINDOWS_UWP
            result.Left = pageBounds.Left - winBounds.Left;
            result.Top = pageBounds.Top - winBounds.Top;
            result.Right = winBounds.Right - pageBounds.Right;
            result.Bottom = winBounds.Bottom - pageBounds.Bottom;
#endif

#if __iOS__
#endif
            return result;
        }


    }
}
