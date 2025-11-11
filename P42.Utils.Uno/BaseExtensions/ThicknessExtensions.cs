namespace P42.Utils.Uno;

public static class ThicknessExtensions
{
    public static double Horizontal(this Thickness thickness)
        => thickness.Left + thickness.Right;

    public static double Vertical(this Thickness thickness)
        => thickness.Top + thickness.Bottom;

    public static double Average(this Thickness thickness)
        => (thickness.Horizontal() + thickness.Vertical()) / 4.0;

    public static double Max(this Thickness thickness)
        => Math.Max(Math.Max(thickness.Left,thickness.Right), Math.Max(thickness.Top,thickness.Bottom));

    public static double Min(this Thickness thickness)
        => Math.Min(Math.Min(thickness.Left, thickness.Right), Math.Min(thickness.Top, thickness.Bottom));


    public static Thickness Add(this Thickness t1, Thickness t2)
        => new (t1.Left + t2.Left, t1.Top + t2.Top, t1.Right + t2.Right, t1.Bottom + t2.Bottom);

    public static Thickness Subtract(this Thickness t1, Thickness t2)
        => new (t1.Left - t2.Left, t1.Top - t2.Top, t1.Right - t2.Right, t1.Bottom - t2.Bottom);

    public static Thickness Negate(this Thickness t)
        => new (-t.Left, -t.Top, -t.Bottom, -t.Right);

    public static Thickness Add(this Thickness thickness, double offset)
        => thickness.Add(new Thickness(offset));

    public static Thickness Subtract(this Thickness thickness, double offset)
        => thickness.Subtract(new Thickness(offset));

    private static ThicknessConverter? _thicknessConverter ;
    public static ThicknessConverter ThicknessConverter => _thicknessConverter ??= new ();

}
