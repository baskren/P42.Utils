using System.Collections;

namespace P42.Utils;

public static class EnumerableExtensions
{
    /// <summary>
    /// Performs nested search upon IEnumerable to search for an item
    /// </summary>
    /// <param name="enumerable">items</param>
    /// <param name="obj">item to search for</param>
    /// <returns></returns>
    public static bool Contains(this IEnumerable enumerable, object obj)
    {
        foreach (var item in enumerable)
        {
            if (item is IEnumerable subEnumerable)
            {
                if (subEnumerable.Contains(obj))
                    return true;
            }
            if (item.Equals(obj))
                return true;
        }
        return false;
    }
    
}
