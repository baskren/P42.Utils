using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace P42.Utils.Uno
{
    public static class VisibilityExtensions
    {
        public static Visibility ToVisibility(this bool value)
            => value ? Visibility.Visible : Visibility.Collapsed;

        public static Visibility ToVisibility(this IEnumerable collection)
            => collection?.Any() ?? false
                    ? Visibility.Visible
                    : Visibility.Collapsed;

        public static bool ToBool(this Visibility visibility)
            => visibility == Visibility.Visible;

        static NullEmptyWhiteSpaceVisiblityConverter nullEmptyWhiteSpaceVisiblityConverter;
        public static NullEmptyWhiteSpaceVisiblityConverter NullEmptyWhiteSpaceVisiblityConverter => nullEmptyWhiteSpaceVisiblityConverter = nullEmptyWhiteSpaceVisiblityConverter ?? new NullEmptyWhiteSpaceVisiblityConverter();
    }

    public class NullEmptyWhiteSpaceVisiblityConverter : IValueConverter
    {
        internal NullEmptyWhiteSpaceVisiblityConverter() { }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool tf)
                return tf ? Visibility.Visible : Visibility.Collapsed;
            if (value is string txt)
                return string.IsNullOrWhiteSpace(txt) ? Visibility.Collapsed : Visibility.Visible;
            if (value is null)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
