using Windows.Foundation;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class RectExtensions
{

    extension(Rect t1)
    {
        public Rect Grow(Thickness t2)
            => new (t1.Left - t2.Left, t1.Top - t2.Top, t1.Width + t2.Left + t2.Right, t1.Height + t2.Top + t2.Bottom);

        public Rect Shrink(Thickness t2)
            => new (t1.Left + t2.Left, t1.Top + t2.Top, t1.Width - t2.Left - t2.Right, t1.Height - t2.Top - t2.Bottom);

        public Rect Grow(double value)
            => t1.Grow(new Thickness(value));

        public Rect Shrink(double value)
            => t1.Shrink(new Thickness(value));

        public double CenterX()
            => (t1.Right + t1.Left)/2.0;

        public double CenterY()
            => (t1.Bottom + t1.Top)/2.0;
    }
}
