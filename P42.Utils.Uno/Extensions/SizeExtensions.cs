using System;
using Windows.Foundation;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

public static class SizeExtensions
{
    /// <summary>
    /// Increase the size of a Size by a Thickness
    /// </summary>
    /// <param name="size"></param>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static Size Add(this Size size, Thickness thickness)
    {
        size.Width += thickness.Horizontal();
        size.Height += thickness.Vertical();
        return size;
    }
    
    /// <summary>
    /// Uniformly increase size by a value
    /// </summary>
    /// <param name="size"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Size Add(this Size size, double t)
    {
        size.Width += t;
        size.Height += t;
        return size;
    }
    
    /// <summary>
    /// Increase size by horizontal and vertical values
    /// </summary>
    /// <param name="size"></param>
    /// <param name="h"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Size Add(this Size size, double h, double v)
    {
        size.Width += h;
        size.Height += v;
        return size;
    }
    
    /// <summary>
    /// Subtract a Thickness from a Size
    /// </summary>
    /// <param name="size"></param>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static Size Subtract(this Size size, Thickness thickness)
    {
        size.Width -= thickness.Horizontal();
        size.Height -= thickness.Vertical();
        return size;
    }

    /// <summary>
    /// Is a size zero?
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static bool IsZero(this Size size)
        => size.Width <= 0 || size.Height <= 0;

    /// <summary>
    /// Math.Floor of Size values
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Size Floor(this Size size) => new(Math.Floor(size.Width), Math.Floor(size.Height));

    /// <summary>
    /// Math.Ceiling of Size values
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Size Ceiling(this Size size) => new(Math.Ceiling(size.Width), Math.Ceiling(size.Height));

    /// <summary>
    /// Math.Round of Size values
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public static Size Round(this Size size) => new(Math.Round(size.Width), Math.Round(size.Height));
}
