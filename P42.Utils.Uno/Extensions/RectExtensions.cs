using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace P42.Utils.Uno;

/// <summary>
/// Extensions for Windows.Foundation.Rect
/// </summary>
public static class RectExtensions
{

    /// <summary>
    /// Grow rect by Thickness
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="thickness"></param>
    /// <returns>update Rect</returns>
    public static Rect Grow(this Rect rect, Thickness thickness)
        => new Rect(rect.Left - thickness.Left, rect.Top - thickness.Top, rect.Width + thickness.Left + thickness.Right, rect.Height + thickness.Top + thickness.Bottom);

    /// <summary>
    /// Shrink rect by thickness
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="thickness"></param>
    /// <returns>updated Rect</returns>
    public static Rect Shrink(this Rect rect, Thickness thickness)
        => new Rect(rect.Left + thickness.Left, rect.Top + thickness.Top, rect.Width - thickness.Left - thickness.Right, rect.Height - thickness.Top - thickness.Bottom);

    /// <summary>
    /// Grow rect by moving sides by value away from center
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="value"></param>
    /// <returns>updated rect</returns>
    public static Rect Grow(this Rect rect, double value)
        => rect.Grow(new Thickness(value));

    /// <summary>
    /// Shrink rect by moving sides by value toward center
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="value"></param>
    /// <returns>updated rect</returns>
    public static Rect Shrink(this Rect rect, double value)
        => rect.Shrink(new Thickness(value));

    /// <summary>
    /// Horizontal center of rect
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static double CenterX(this Rect rect)
        => (rect.Right + rect.Left)/2.0;

    /// <summary>
    /// Vertical center of rect
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public static double CenterY(this Rect rect)
        => (rect.Bottom + rect.Top)/2.0;    
}
