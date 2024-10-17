using System;
using System.Collections.Generic;
using System.Linq;

namespace P42.Utils;

public static class ListExtensions
{
    /// <summary>
    /// Insert items in range to list, starting at index
    /// </summary>
    /// <param name="iList">destination list</param>
    /// <param name="index">index to start insertion</param>
    /// <param name="range">items to insert</param>
    /// <typeparam name="T">Collection item type</typeparam>
    public static void InsertRange<T>(this IList<T> iList, int index, IEnumerable<T> range)
    {
        foreach (var item in range)
            iList.Insert(index++, item);
        
    }


    /// <summary>
    /// Remove items in range from List
    /// </summary>
    /// <param name="iList">destination list</param>
    /// <param name="range">items to remove</param>
    /// <typeparam name="T">Collection item type</typeparam>
    public static void RemoveRange<T>(this IList<T> iList, IEnumerable<T> range)
    {
        foreach (var item in range)
            iList.Remove(item);
        
    }

    /// <summary>
    /// Updates List to match Source list
    /// </summary>
    /// <param name="iList">List to be updated</param>
    /// <param name="source">Source of updates</param>
    /// <param name="reversed">optional, reverse order</param>
    /// <typeparam name="T">Collection item type</typeparam>
    public static void SyncWith<T>(this IList<T> iList, IList<T>? source, bool reversed = false)
    {
        if (source is null)
        {
            iList.Clear();
            return;
        }    
        
        foreach (var item in iList.ToArray())
        {
            if (!source.Contains(item))
                iList.Remove(item);
        }
        if (reversed)
            source = source.Reverse().ToList();
        
        for (var i=0; i < source.Count; i++)
        {
            var item = source[i];
            if (iList.Count > i && ReferenceEquals(iList[i], item))
                continue;

            var currentIndex = iList.IndexOf(item);
            if (currentIndex >= 0)
            {
                if (i == currentIndex)
                    continue;

                iList.Remove(item);
            }

            iList.Insert(i, item);
        }
    }

    /// <summary>
    /// Remove items from list that meet condition
    /// </summary>
    /// <param name="iList">List to prune</param>
    /// <param name="conditional">condition</param>
    /// <typeparam name="T">Collection item type</typeparam>
    public static void RemoveIf<T>(this IList<T> iList, Func<T, bool> conditional)
    {
        var items = iList.ToArray();
        foreach (var item in items)
        {
            if (conditional.Invoke(item))
                iList.Remove(item);
        }
    }
}
