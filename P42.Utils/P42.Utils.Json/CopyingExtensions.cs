using System;
using System.Collections.Generic;
using System.Linq;

namespace P42.Utils;

public static class CopyingExtensions
{
    
    /// <summary>
    /// Copies the source to a new List with new copies of each item.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T">ICopiable</typeparam>
    /// <returns></returns>
    public static List<T> ValueCopy<T>(this IEnumerable<T> source) where T : ICopiable<T>, new()
        => source.Select(member => member.Copy()).ToList();
    

    /// <summary>
    /// Create copy of an ICopiable
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Copy<T>(this T source) where T : ICopiable<T>, new()
    {
        if (new T() is not { } result)
            throw new NotSupportedException($"Copying from {source.GetType()} is not supported.");
        
        result.PropertiesFrom(source);
        return result;
    }
}
