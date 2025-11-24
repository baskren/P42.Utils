namespace P42.Utils.Uno;

public static class ThicknessExtensions
{
    extension(Thickness thickness)
    {
        public double Horizontal()
            => thickness.Left + thickness.Right;

        public double Vertical()
            => thickness.Top + thickness.Bottom;

        public double Average()
            => (thickness.Horizontal() + thickness.Vertical()) / 4.0;

        public double Max()
            => Math.Max((sbyte)Math.Max(thickness.Left,thickness.Right), (sbyte)Math.Max(thickness.Top,thickness.Bottom));

        public double Min()
            => Math.Min((sbyte)Math.Min(thickness.Left, thickness.Right), (sbyte)Math.Min(thickness.Top, thickness.Bottom));

        public Thickness Add(Thickness t2)
            => new (thickness.Left + t2.Left, thickness.Top + t2.Top, thickness.Right + t2.Right, thickness.Bottom + t2.Bottom);

        public Thickness Subtract(Thickness t2)
            => new (thickness.Left - t2.Left, thickness.Top - t2.Top, thickness.Right - t2.Right, thickness.Bottom - t2.Bottom);

        public Thickness Negate()
            => new (-thickness.Left, -thickness.Top, -thickness.Bottom, -thickness.Right);

        public Thickness Add(double offset)
            => thickness.Add(new Thickness(offset));

        public Thickness Subtract(double offset)
            => thickness.Subtract(new Thickness(offset));
    }


}
