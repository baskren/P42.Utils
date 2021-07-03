using System;
using System.Collections.Generic;
using System.Text;

namespace P42.Utils
{
    public static class IListExtensions
    {
        public static void InsertRange<T>(this IList<T> iList, int index, IList<T> range)
        {
            for (int i=range.Count-1; i >= 0; i--)
            {
                iList.Insert(index, range[i]);
            }
        }

        public static void RemoveRange<T>(this IList<T> iList, IList<T> range)
        {
            foreach (var item in range)
            {
                if (iList.Contains(item))
                    iList.Remove(item);
            }
        }
    }
}
