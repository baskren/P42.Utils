using Windows.Foundation;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class SizeExtensions
{
    extension(Size size)
    {
        public Size Add(Thickness thickness)
        {
            size.Width += thickness.Horizontal();
            size.Height += thickness.Vertical();
            return size;
        }

        public Size Add(double t)
        {
            size.Width += t;
            size.Height += t;
            return size;
        }

        public Size Add(double h, double v)
        {
            size.Width += h;
            size.Height += v;
            return size;
        }

        public Size Subtract(Thickness thickness)
        {
            size.Width -= thickness.Horizontal();
            size.Height -= thickness.Vertical();
            return size;
        }

        public bool IsZero()
            => size.Width <= 0 || size.Height <= 0;

        public Size Floor()
            => new (Math.Floor(size.Width), Math.Floor(size.Height));

        public Size Ceiling()
            => new (Math.Ceiling(size.Width), Math.Ceiling(size.Height));

        public Size Round()
            => new (Math.Round(size.Width), Math.Round(size.Height));
    }
}
