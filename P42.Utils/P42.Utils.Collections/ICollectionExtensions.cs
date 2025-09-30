using System;
using System.Collections.Generic;

namespace P42.Utils;

/// <summary>
/// ICollectionExtensions
/// </summary>
public static class ICollectionExtensions
{
    /// <summary>
    /// Is ICollection null or empty?
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Obsolete("Should not be necessary when NULLABLE is ENABLED.  This may be removed in a future version.")]
    public static bool IsNullOrEmpty<T>(this ICollection<T>? collection)
        => collection == null || collection.Count == 0;
    

    /// <summary>
    /// Add range to collection
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public static ICollection<T> AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
    {
        foreach (var item in range)
            collection.Add(item);
        
        return collection;
    }

    /// <summary>
    /// Remove range from collection
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public static ICollection<T> RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> range)
    {
        foreach (var item in range)
            collection.Remove(item);
        
        return collection;
    }
}
