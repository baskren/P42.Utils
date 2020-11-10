using Windows.Foundation;
using Windows.UI.Xaml;

namespace P42.Utils.Uno

{
    /// <summary>
    /// Descendent bounds.
    /// </summary>
    public static class CoordinateTransformExtensions
	{
        #region ILocation implementation

        public static Point CoordTransform(this FrameworkElement fromElement, Point p, FrameworkElement toElement) 
        {
            if (fromElement != null && toElement != null)
            {
                var transform = fromElement.TransformToVisual(toElement);
                return transform.TransformPoint(p);
            }
            return new Point(double.NegativeInfinity, double.NegativeInfinity);
        }

        public static Rect CoordTransform(this FrameworkElement fromElement, Rect r, FrameworkElement toElement) 
        {
            if (r.Width < 0 || r.Height < 0)
                return new Rect(-1, -1, -1, -1);

            if (fromElement != null && toElement != null)
            {
                    var transform = fromElement.TransformToVisual(toElement);
                    return transform.TransformBounds(r);
            }
            return new Rect(double.NegativeInfinity, double.NegativeInfinity, fromElement.ActualWidth, fromElement.ActualHeight);
        }

        public static Point PointInWindowCoord(this FrameworkElement element, Point point)
        {
            if (element != null)
            {
                var transform = element.TransformToVisual(Window.Current.Content);
                return transform.TransformPoint(point);
            }
            return new Point(double.NegativeInfinity, double.NegativeInfinity);
        }

        public static Rect BoundsInWindowCoord(this FrameworkElement element)
        {
            if (element != null)
            {
                var transform = element.TransformToVisual(Window.Current.Content);
                var transformedRectangle = transform.TransformBounds(new Rect(0,0, element.ActualWidth, element.ActualHeight));
                return transformedRectangle;
            }
            return new Rect(double.NegativeInfinity, double.NegativeInfinity, element.Width, element.Height);
        }
        #endregion

    }
}

