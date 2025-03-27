using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace P42.Utils.Uno;

public static class ThicknessExtensions
{
    /// <summary>
    /// Horizontal value of Thickness
    /// </summary>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static double Horizontal(this Thickness thickness)
        => thickness.Left + thickness.Right;

    /// <summary>
    /// Vertical value of Thickness
    /// </summary>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static double Vertical(this Thickness thickness)
        => thickness.Top + thickness.Bottom;

    /// <summary>
    /// Average value of Thickness
    /// </summary>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static double Average(this Thickness thickness)
        => (thickness.Horizontal() + thickness.Vertical()) / 4.0;

    /// <summary>
    /// Max value of Thickness
    /// </summary>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static double Max(this Thickness thickness)
        => Math.Max(Math.Max(thickness.Left,thickness.Right), Math.Max(thickness.Top,thickness.Bottom));

    /// <summary>
    /// Min value of Thickness
    /// </summary>
    /// <param name="thickness"></param>
    /// <returns></returns>
    public static double Min(this Thickness thickness)
        => Math.Min(Math.Min(thickness.Left, thickness.Right), Math.Min(thickness.Top, thickness.Bottom));

    /// <summary>
    /// Add two thicknesses
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public static Thickness Add(this Thickness t1, Thickness t2) =>
        new(t1.Left + t2.Left, t1.Top + t2.Top, t1.Right + t2.Right, t1.Bottom + t2.Bottom);

    /// <summary>
    /// Subtract two thicknesses
    /// </summary>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <returns></returns>
    public static Thickness Subtract(this Thickness t1, Thickness t2) =>
        new(t1.Left - t2.Left, t1.Top - t2.Top, t1.Right - t2.Right, t1.Bottom - t2.Bottom);

    /// <summary>
    /// get the negative of a thickness
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Thickness Negate(this Thickness t) => new(-t.Left, -t.Top, -t.Bottom, -t.Right);

    /// <summary>
    /// Add a value to all sides of a thickness
    /// </summary>
    /// <param name="thickness"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Thickness Add(this Thickness thickness, double offset)
        => thickness.Add(new Thickness(offset));

    /// <summary>
    /// subtract a value from all sides of a thickness
    /// </summary>
    /// <param name="thickness"></param>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static Thickness Subtract(this Thickness thickness, double offset)
        => thickness.Subtract(new Thickness(offset));

    private static ThicknessConverter? _thicknessConverter;
    /// <summary>
    /// Instance of ThicknessConverter ready to go to work
    /// </summary>
    public static ThicknessConverter ThicknessConverter => _thicknessConverter ??= new ThicknessConverter();

}

