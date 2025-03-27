using System;
using System.Collections.Generic;
using System.Linq;

namespace P42.Utils;

/// <summary>
/// IList extensions
/// </summary>
public static class IListExtensions
{
    /// <summary>
    /// Insert range into IList
    /// </summary>
    /// <param name="iList"></param>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public static IList<T> InsertRange<T>(this IList<T> iList, int index, IList<T> range)
    {
        for (var i=range.Count-1; i >= 0; i--)
            iList.Insert(index, range[i]);
        
        return iList;
    }

    /// <summary>
    /// Remove range from IList
    /// </summary>
    /// <param name="iList"></param>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public static IList<T> RemoveRange<T>(this IList<T> iList, IEnumerable<T> range)
    {
        foreach (var item in range)
            iList.Remove(item);
        
        return iList;
    }

    /// <summary>
    /// Sync IList with another so as not to cause a ListView to reset its scroll offset
    /// </summary>
    /// <param name="iList"></param>
    /// <param name="newOrder"></param>
    /// <param name="newReversed"></param>
    /// <param name="isReset"></param>
    /// <typeparam name="T"></typeparam>
    public static IList<T> SyncWith<T>(this IList<T> iList, IList<T>? newOrder, bool isReset = false, bool newReversed = false)
    {
        if (isReset)
            iList.Clear();

        if (newOrder is null)
        {
            iList.Clear();
            return iList;
        }

        if (newReversed)
            newOrder = newOrder.Reverse().ToArray();

        for (var i = 0; i < newOrder.Count; i++)
            iList[i] = newOrder[i];
        
        for (var i = newOrder.Count; i < iList.Count; i++)
            iList.RemoveAt(i);
        
        return iList;   
    }

    /// <summary>
    /// Conditionally remove value from IList
    /// </summary>
    /// <param name="iList"></param>
    /// <param name="conditional"></param>
    /// <typeparam name="T"></typeparam>
    public static IList<T> RemoveIf<T>(this IList<T> iList, Func<T, bool> conditional)
    {
        var items = iList.ToArray();
        foreach (var item in items)
            if (conditional.Invoke(item))
                iList.Remove(item);
        
        return iList;
    }
    
}
