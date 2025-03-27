using Windows.Foundation;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

/// <summary>
/// Descendent bounds.
/// </summary>
// ReSharper disable once UnusedType.Global
public static class CoordinateTransformExtensions
{
    
    #region ILocation implementation

    /// <summary>
    /// Transforms the point from the coordinate system of the fromElement to the coordinate system of the toElement.
    /// </summary>
    /// <param name="fromElement"></param>
    /// <param name="p"></param>
    /// <param name="toElement"></param>
    /// <returns></returns>
    public static Point CoordTransform(this FrameworkElement fromElement, Point p, FrameworkElement toElement) 
    {
        var transform = fromElement.TransformToVisual(toElement);
        return transform.TransformPoint(p);
    }

    /// <summary>
    /// Transforms the rectangle from the coordinate system of the fromElement to the coordinate system of the toElement.
    /// </summary>
    /// <param name="fromElement"></param>
    /// <param name="r"></param>
    /// <param name="toElement"></param>
    /// <returns></returns>
    public static Rect CoordTransform(this FrameworkElement fromElement, Rect r, FrameworkElement toElement) 
    {
        if (r.Width < 0 || r.Height < 0)
            return new Rect(-1, -1, -1, -1);

        var transform = fromElement.TransformToVisual(toElement);
        return transform.TransformBounds(r);
    }

    /// <summary>
    /// Gets the coordinates of the point in the coordinate system of the app window.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Point PointInWindowCoord(this FrameworkElement element, Point point)
    {
            var transform = element.TransformToVisual(Platform.Frame);
            return transform.TransformPoint(point);
    }

    /// <summary>
    /// Gets the bounds of the element in the coordinate system of the app window.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Rect BoundsInWindowCoord(this FrameworkElement element)
    {
            var transform = element.TransformToVisual(Platform.Frame);
            var transformedRectangle = transform.TransformBounds(new Rect(0,0, element.ActualWidth, element.ActualHeight));
            return transformedRectangle;
    }
    #endregion

}
