using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;

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

        public static Thickness Add(this Thickness thickness, double offset)
            => thickness.Add(new Thickness(offset));

        public static Thickness Subtract(this Thickness thickness, double offset)
            => thickness.Subtract(new Thickness(offset));

        static ThicknessConverter thicknessConverter;
        public static ThicknessConverter ThicknessConverter => thicknessConverter ??= new ThicknessConverter();

    }

    public class ThicknessConverter : IValueConverter
    {
        internal ThicknessConverter() { }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var zero = new Thickness(0);
            var fallback = zero;
            if (parameter is Thickness param)
                fallback = param;

            if (value is null)
                return new Thickness(0);
            if (value is bool tf)
                return tf ? fallback : zero;
            if (value is int intValue)
                return new Thickness(intValue);
            if (value is double doubleValue)
                return new Thickness(doubleValue);
            return zero;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidCastException($"Cannot P42.Utils.Uno.ThicknessConverter.ConvertBack({value},{targetType}) ");
        }
    }

}
