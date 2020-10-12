using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uno.Extensions.Specialized;
using Windows.UI.Xaml;

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
    }
}
