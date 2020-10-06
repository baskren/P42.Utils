using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Shapes;

namespace P42.Utils.Uno
{
    public static class UIElementExtensions
    {
        public static Rect GetBounds(this UIElement element)
        {
            var ttv = element.TransformToVisual(Windows.UI.Xaml.Window.Current.Content);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.DesiredSize.Width, element.DesiredSize.Height));
        }

    }
}
