using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace P42.Utils.Uno
{
    public static class VisibilityExtensions
    {
        public static Visibility ToVisibility(this bool value)
            => value ? Visibility.Visible : Visibility.Collapsed;

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
