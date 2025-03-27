using System.Collections;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class VisibilityExtensions
{
    /// <summary>
    /// Bool to Visibility
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Visibility ToVisibility(this bool value)
        => value ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Color to Visibility (based upon Alpha value)
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Visibility ToVisibility(this Color color)
        => color.A > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// Int to Visibility (based upon positive value)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Visibility ToVisibility(this int value)
        => value > 0 ? Visibility.Visible : Visibility.Collapsed;
    /// <summary>
    /// Double to Visibility (based upon positive value)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Visibility ToVisibility(this double value)
        => value > 0 ? Visibility.Visible : Visibility.Collapsed;

    /// <summary>
    /// String to Visibility (based upon  non-null/empty value)
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static Visibility ToVisibility(this string? text)
        => string.IsNullOrWhiteSpace(text) ? Visibility.Collapsed : Visibility.Visible;

    /// <summary>
    /// IEnumerable to Visibility (based upon any items)
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public static Visibility ToVisibility(this IEnumerable? items)
    {
        if (items is null)
            return Visibility.Collapsed;
        
        var any = false;
        if (items is ICollection collection)
            any = collection.Count > 0;
        // ReSharper disable once GenericEnumeratorNotDisposed
        else if (items.GetEnumerator() is { } enumerator)
            any = enumerator.MoveNext();
        return any
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    /// <summary>
    /// Visibility pass through
    /// </summary>
    /// <param name="visibility"></param>
    /// <returns></returns>
    public static bool ToBool(this Visibility visibility)
        => visibility == Visibility.Visible;

}
