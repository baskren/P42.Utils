using System.Collections;
using System.Collections.Generic;

namespace P42.Utils;

/// <summary>
/// IEnumerable Extensions
/// </summary>
public static class IEnumerableExtensions
{
    
    /// <summary>
    /// Does a nested IEnumerable contain an object
    /// </summary>
    /// <param name="enumerable"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool DeepContains(this IEnumerable enumerable, object obj)
    {
        foreach (var item in enumerable)
        {
            if (item is IEnumerable subEnumerable)
            {
                if (subEnumerable.DeepContains(obj))
                    return true;
            }
            
            if (item.Equals(obj))
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// provides indexer with with IEnumerable
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<(int index, T value)> WithIndex<T>(this IEnumerable<T> source)
    {
        var index = 0;
        foreach (var value in source)
        {
            yield return (index, value);
            index++;
        }
    }

}
