using Windows.Foundation;

namespace P42.Utils.Uno;

/// <summary>
/// Descendent bounds.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class CoordinateTransformExtensions
{
    
    #region ILocation implementation

    /// <param name="fromElement"></param>
    extension(FrameworkElement fromElement)
    {
        /// <summary>
        /// Transforms the point from the coordinate system of the fromElement to the coordinate system of the toElement.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="toElement"></param>
        /// <returns></returns>
        public Point CoordTransform(Point p, FrameworkElement toElement) 
        {
            var transform = fromElement.TransformToVisual(toElement);
            return transform.TransformPoint(p);
        }

        /// <summary>
        /// Transforms the rectangle from the coordinate system of the fromElement to the coordinate system of the toElement.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="toElement"></param>
        /// <returns></returns>
        public Rect CoordTransform(Rect r, FrameworkElement toElement) 
        {
            if (r.Width < 0 || r.Height < 0)
                return new Rect(-1, -1, -1, -1);

            var transform = fromElement.TransformToVisual(toElement);
            return transform.TransformBounds(r);
        }

        /// <summary>
        /// Gets the coordinates of the point in the coordinate system of the app window.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Point PointInWindowCoord(Point point)
        {
            var transform = fromElement.TransformToVisual(Platform.Frame);
            return transform.TransformPoint(point);
        }

        /// <summary>
        /// Gets the bounds of the element in the coordinate system of the app window.
        /// </summary>
        /// <returns></returns>
        public Rect BoundsInWindowCoord()
        {
            var transform = fromElement.TransformToVisual(Platform.Frame);
            var transformedRectangle = transform.TransformBounds(new Rect(0,0, fromElement.ActualWidth, fromElement.ActualHeight));
            return transformedRectangle;
        }
    }

    #endregion

}
