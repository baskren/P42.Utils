using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using static System.Net.Mime.MediaTypeNames;

namespace P42.Utils.Uno
{
    public static class VisibilityExtensions
    {
        public static Visibility ToVisibility(this bool value)
            => value ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility ToVisibility(this Color color)
            => color.A > 0 ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility ToVisibility(this int value)
            => value > 0 ? Visibility.Visible : Visibility.Collapsed;
        public static Visibility ToVisibility(this double value)
            => value > 0 ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility ToVisibility(this string text)
            => string.IsNullOrWhiteSpace(text) ? Visibility.Collapsed : Visibility.Visible;

        public static Visibility ToVisibility(this IEnumerable items)
        {
            var any = false;
            if (items is ICollection collection)
                any = collection.Count > 0;
            else if (items.GetEnumerator() is IEnumerator enumerator)
                any = enumerator.MoveNext();
            return any
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        public static bool ToBool(this Visibility visibility)
            => visibility == Visibility.Visible;

        static VisibilityConverter visibilityConverter;
        public static VisibilityConverter VisibilityConverter => visibilityConverter = visibilityConverter ?? new VisibilityConverter();
    }

    public class VisibilityConverter : IValueConverter
    {
        internal VisibilityConverter() { }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null)
                return Visibility.Collapsed;
            if (value is bool tf)
                return tf.ToVisibility();
            if (value is Color color)
                return color.ToVisibility();
            if (value is Brush brush)
            {
                if (brush is SolidColorBrush solidBrush)
                    return solidBrush.Color.ToVisibility();
            }
            if (value is int intValue)
                return intValue.ToVisibility();
            if (value is double doubleValue)
                return doubleValue.ToVisibility();
            if (value is string txt)
                return txt.ToVisibility();
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                if (targetType == typeof(bool))
                    return visibility == Visibility.Visible;
                if (targetType == typeof(Color))
                {
                    return visibility == Visibility.Visible
                        ? parameter as Color? ?? Colors.Red
                        : Colors.Transparent;
                }
                if (targetType == typeof(Brush))
                {
                    if (parameter is Brush brush)
                        return visibility == Visibility.Visible ? brush : null;
                    if (parameter is Color color)
                        return visibility == Visibility.Visible ? new SolidColorBrush(color) : null;
                    return visibility == Visibility.Visible ? new SolidColorBrush(Colors.Red) : null;
                }
                if (targetType == typeof(string))
                    return visibility == Visibility.Visible
                        ? parameter as string ?? "visible"
                        : null;
                if (targetType == typeof(int))
                    return visibility == Visibility.Visible
                        ? parameter as int? ?? 1
                        : 0;
                if (targetType == typeof(double))
                    return visibility == Visibility.Visible
                        ? parameter as double? ?? 1.0
                        : 0.0;
            }

            throw new InvalidCastException($"Cannot P42.Utils.Uno.VisibilityConverter.ConvertBack({value},{targetType}) ");
        }
    }


}
