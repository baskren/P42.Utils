using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace P42.Utils.Uno
{
    public static class AppWindow
    {
        public static Size Size()
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

    }
}
