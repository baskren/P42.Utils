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
        public static Size Subtract(this Size size, Thickness thickness)
        {
            size.Width -= thickness.Horizontal();
            size.Height -= thickness.Vertical();
            return size;
        }

        public static bool IsZero(this Size size)
            => size.Width <= 0 || size.Height <= 0;
    }
}
