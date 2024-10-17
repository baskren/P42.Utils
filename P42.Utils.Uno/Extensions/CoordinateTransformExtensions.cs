using Windows.Foundation;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

/// <summary>
/// Descendent bounds.
/// </summary>
public static class CoordinateTransformExtensions
{
    #region ILocation implementation

    /// <summary>
    /// Transform point from one element's coordinate system to another 
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
    /// Transform rect from one element's coordinate system to another
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
    /// Get window coordinates for a point within an element
    /// </summary>
    /// <param name="element"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Point PointInWindowCoord(this FrameworkElement element, Point point)
    {
        if (Window.Current?.Content is null)
            return new Point(double.NegativeInfinity, double.NegativeInfinity);

        var transform = element.TransformToVisual(Window.Current.Content);
        return transform.TransformPoint(point);
    }

    /// <summary>
    /// Get window coordinates for the bounds of an element
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static Rect BoundsInWindowCoord(this FrameworkElement element)
    {
        if (Window.Current?.Content is null)
            return new Rect(double.NegativeInfinity, double.NegativeInfinity, element.Width, element.Height);

        var transform = element.TransformToVisual(Window.Current.Content);
        var transformedRectangle = transform.TransformBounds(new Rect(0,0, element.ActualWidth, element.ActualHeight));
        return transformedRectangle;
    }
    #endregion

}
