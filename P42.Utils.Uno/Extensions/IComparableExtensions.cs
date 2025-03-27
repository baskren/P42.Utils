using System;
using System.Collections.Generic;

namespace P42.Utils.Uno;

public static class IComparableExtensions
{
    /// <summary>
    /// Limit value to a range
    /// </summary>
    /// <param name="self"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Clamp<T>(this T self, T min, T max) where T : IComparable
    {
        if (Comparer<T>.Default.Compare(max, min) < 0)
            (min, max) = (max, min);

        if (Comparer<T>.Default.Compare(self, min) < 0)
            return min;
        
        return Comparer<T>.Default.Compare(self, max) > 0 
            ? max 
            : self;
    }
}
