using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace P42.Utils.Uno
{
    public static class SizeExtensions
    {
        public static Size Add(this Size size, Thickness thickness)
        {
            size.Width += thickness.Horizontal();
            size.Height += thickness.Vertical();
            return size;
        }
        public static Size Add(this Size size, double t)
        {
            size.Width += t;
            size.Height += t;
            return size;
        }
        public static Size Add(this Size size, double h, double v)
        {
            size.Width += h;
            size.Height += v;
            return size;
        }
        public static Size Subtract(this Size size, Thickness thickness)
        {
            size.Width -= thickness.Horizontal();
            size.Height -= thickness.Vertical();
            return size;
        }

        public static bool IsZero(this Size size)
            => size.Width <= 0 || size.Height <= 0;

        public static Size Floor(this Size size)
            => new Size(Math.Floor(size.Width), Math.Floor(size.Height));

        public static Size Ceiling(this Size size)
            => new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));

        public static Size Round(this Size size)
            => new Size(Math.Round(size.Width), Math.Round(size.Height));
    }
}
