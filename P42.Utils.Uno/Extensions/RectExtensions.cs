using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace P42.Utils.Uno;

public static class RectExtensions
{

    /// <summary>
    /// Increase the size of a Rect by a Thickness
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public static Rect Grow(this Rect t1, Thickness t2)
        => new(t1.Left - t2.Left, t1.Top - t2.Top, t1.Width + t2.Left + t2.Right, t1.Height + t2.Top + t2.Bottom);

    /// <summary>
    /// Descrease the size of a Rect by a Thickness
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public static Rect Shrink(this Rect t1, Thickness t2)
        => new(t1.Left + t2.Left, t1.Top + t2.Top, t1.Width - t2.Left - t2.Right, t1.Height - t2.Top - t2.Bottom);

    /// <summary>
    /// Uniformly increase the size of a Rect by a value
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Rect Grow(this Rect t1, double value)
        => t1.Grow(new Thickness(value));

    /// <summary>
    /// Uniformly decrease the size of a Rect by a value
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Rect Shrink(this Rect t1, double value)
        => t1.Grow(-value);

    /// <summary>
    /// Horizontal center of a rect
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public static double CenterX(this Rect r)
        => (r.Right + r.Left)/2.0;

    /// <summary>
    /// Vertical center of a rect
    /// </summary>
    /// <param name="r"></param>
    /// <returns></returns>
    public static double CenterY(this Rect r)
        => (r.Bottom + r.Top)/2.0;    
}
