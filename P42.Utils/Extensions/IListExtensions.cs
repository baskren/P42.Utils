using System;
using System.Collections.Generic;
using System.Linq;
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

        public static void SyncWith<T>(this IList<T> iList, IList<T> source, bool reversed = false)
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
            for (int i=0; i < source.Count; i++)
            {
                var item = source[i];
                if (iList.Count <= i || !ReferenceEquals(iList[i], item))
                {
                    var currentIndex = iList.IndexOf(item);
                    if (currentIndex >= 0)
                    {
                        if (i != currentIndex)
                        {
                            iList.Remove(item);
                            iList.Insert(i, item);
                        }
                    }
                    else
                        iList.Insert(i, item);
                }
            }
        }
    }
}
