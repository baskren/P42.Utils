using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno
{
    public static class ThicknessExtensions
    {
        public static double Horizontal(this Thickness thickness)
            => thickness.Left + thickness.Right;

        public static double Vertical(this Thickness thickness)
            => thickness.Top + thickness.Bottom;

        public static double Average(this Thickness thickness)
            => (thickness.Horizontal() + thickness.Vertical()) / 4.0;

        public static Thickness Add(this Thickness t1, Thickness t2)
            => new Thickness(t1.Left + t2.Left, t1.Top + t2.Top, t1.Right + t2.Right, t1.Bottom + t2.Bottom);

        public static Thickness Subtract(this Thickness t1, Thickness t2)
            => new Thickness(t1.Left - t2.Left, t1.Top - t2.Top, t1.Right - t2.Right, t1.Bottom - t2.Bottom);

        public static Thickness Negate(this Thickness t)
            => new Thickness(-t.Left, -t.Top, -t.Bottom, -t.Right);
    }
}
