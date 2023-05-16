using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace P42.Utils.Uno
{
    public static class RectExtensions
    {

        public static Rect Grow(this Rect t1, Thickness t2)
            => new Rect(t1.Left - t2.Left, t1.Top - t2.Top, t1.Width + t2.Left + t2.Right, t1.Height + t2.Top + t2.Bottom);

        public static Rect Shrink(this Rect t1, Thickness t2)
            => new Rect(t1.Left + t2.Left, t1.Top + t2.Top, t1.Width - t2.Left - t2.Right, t1.Height - t2.Top - t2.Bottom);

        public static Rect Grow(this Rect t1, double value)
            => t1.Grow(new Thickness(value));

        public static Rect Shrink(this Rect t1, double value)
            => t1.Shrink(new Thickness(value));

        public static double CenterX(this Rect r)
            => (r.Right + r.Left)/2.0;

        public static double CenterY(this Rect r)
            => (r.Bottom + r.Top)/2.0;    
    }
}
