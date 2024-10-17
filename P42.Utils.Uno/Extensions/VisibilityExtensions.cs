using System.Collections;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace P42.Utils.Uno;

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

}