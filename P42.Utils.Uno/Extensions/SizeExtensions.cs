using System;
using Windows.Foundation;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

/// <summary>
/// Extensions for Windows.Foundation.Size
/// </summary>
public static class SizeExtensions
{
    /// <summary>
    /// Add thickness to size
    /// </summary>
    /// <param name="size"></param>
    /// <param name="thickness"></param>
    /// <returns>updated Size</returns>
    public static Size Add(this Size size, Thickness thickness)
    {
        size.Width += thickness.Horizontal();
        size.Height += thickness.Vertical();
        return size;
    }
    
    /// <summary>
    /// Add value to Width and Height to size
    /// </summary>
    /// <param name="size"></param>
    /// <param name="t"></param>
    /// <returns>updated Size</returns>
    public static Size Add(this Size size, double t)
    {
        size.Width += t;
        size.Height += t;
        return size;
    }
    
    /// <summary>
    /// Add hz and v to width and height of Size
    /// </summary>
    /// <param name="size"></param>
    /// <param name="hz"></param>
    /// <param name="vt"></param>
    /// <returns>updates Size</returns>
    public static Size Add(this Size size, double hz, double vt)
    {
        size.Width += hz;
        size.Height += vt;
        return size;
    }
    
    /// <summary>
    /// Subtract thickness from Size
    /// </summary>
    /// <param name="size"></param>
    /// <param name="thickness"></param>
    /// <returns>updated Size</returns>
    public static Size Subtract(this Size size, Thickness thickness)
    {
        size.Width -= thickness.Horizontal();
        size.Height -= thickness.Vertical();
        return size;
    }

    /// <summary>
    /// Subtract value from Width and Height to size
    /// </summary>
    /// <param name="size"></param>
    /// <param name="t"></param>
    /// <returns>updated Size</returns>
    public static Size Subtract(this Size size, double t)
    {
        size.Width -= t;
        size.Height -= t;
        return size;
    }
    
    /// <summary>
    /// Subtract hz and v from width and height of Size
    /// </summary>
    /// <param name="size"></param>
    /// <param name="hz"></param>
    /// <param name="vt"></param>
    /// <returns>updates Size</returns>
    public static Size Subtract(this Size size, double hz, double vt)
    {
        size.Width -= hz;
        size.Height -= vt;
        return size;
    }

    /// <summary>
    /// Is size zero
    /// </summary>
    /// <param name="size"></param>
    /// <returns>true on success</returns>
    public static bool IsZero(this Size size)
        => size.Width <= 0 || size.Height <= 0;

    /// <summary>
    /// Assure Size rounded down
    /// </summary>
    /// <param name="size"></param>
    /// <returns>updated size</returns>
    public static Size Floor(this Size size)
        => new(Math.Floor(size.Width), Math.Floor(size.Height));

    /// <summary>
    /// Assure Size is rounded up
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Size Ceiling(this Size size)
        => new (Math.Ceiling(size.Width), Math.Ceiling(size.Height));

    /// <summary>
    /// Round size width and height to nearest integers
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Size Round(this Size size)
        => new (Math.Round(size.Width), Math.Round(size.Height));
}
