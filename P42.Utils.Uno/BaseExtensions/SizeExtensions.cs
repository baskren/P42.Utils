using Windows.Foundation;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class SizeExtensions
{
    extension(Size size)
    {
        /// <summary>
        /// Add thickness values to size
        /// </summary>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public Size Add(Thickness thickness)
        {
            size.Width += thickness.Horizontal();
            size.Height += thickness.Vertical();
            return size;
        }

        /// <summary>
        /// Uniformly increase size
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Size Add(double t)
        {
            size.Width += t;
            size.Height += t;
            return size;
        }

        /// <summary>
        /// Add h to Width and v to Height
        /// </summary>
        /// <param name="h"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public Size Add(double h, double v)
        {
            size.Width += h;
            size.Height += v;
            return size;
        }

        /// <summary>
        /// Subtract thickness from size
        /// </summary>
        /// <param name="thickness"></param>
        /// <returns></returns>
        public Size Subtract(Thickness thickness)
        {
            size.Width -= thickness.Horizontal();
            size.Height -= thickness.Vertical();
            return size;
        }

        /// <summary>
        /// Is size less than or equal to zero?
        /// </summary>
        /// <returns></returns>
        public bool IsZero()
            => size.Width <= 0 || size.Height <= 0;

        /// <summary>
        /// Floor Width and Height of size
        /// </summary>
        /// <returns></returns>
        public Size Floor()
            => new (Math.Floor(size.Width), Math.Floor(size.Height));

        /// <summary>
        /// Ceiling Width and Height of size
        /// </summary>
        /// <returns></returns>
        public Size Ceiling()
            => new (Math.Ceiling(size.Width), Math.Ceiling(size.Height));

        /// <summary>
        /// Round Width and Height of size
        /// </summary>
        /// <returns></returns>
        public Size Round()
            => new (Math.Round(size.Width), Math.Round(size.Height));
    }
}
