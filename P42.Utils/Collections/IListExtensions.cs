using System.Diagnostics.CodeAnalysis;

namespace P42.Utils;

/// <summary>
/// IList extensions
/// </summary>
public static class IListExtensions
{
    /// <param name="iList"></param>
    /// <typeparam name="T"></typeparam>
    extension<T>(IList<T> iList)
    {
        /// <summary>
        /// Insert range into IList
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        public IList<T> InsertRange(int index, IList<T> range)
        {
            for (var i=range.Count-1; i >= 0; i--)
                iList.Insert(index, range[i]);
        
            return iList;
        }

        /// <summary>
        /// Remove range from IList
        /// </summary>
        /// <param name="range"></param>
        public IList<T> RemoveRange(IEnumerable<T> range)
        {
            foreach (var item in range)
                iList.Remove(item);
        
            return iList;
        }

        /// <summary>
        /// Sync IList with another so as not to cause a ListView to reset its scroll offset
        /// </summary>
        /// <param name="newOrder"></param>
        /// <param name="newReversed"></param>
        /// <param name="isReset"></param>
        public IList<T> SyncWith(IList<T>? newOrder, bool isReset = false, bool newReversed = false)
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
        /// <param name="conditional"></param>
        public IList<T> RemoveIf(Func<T, bool> conditional)
        {
            var items = iList.ToArray();
            foreach (var item in items)
                if (conditional.Invoke(item))
                    iList.Remove(item);
        
            return iList;
        }

        /// <summary>
        /// Remove last item in list
        /// </summary>
        /// <returns></returns>
        /// <exception cref="AccessViolationException"></exception>
        public T RemoveLast()
        {
            var count = iList.Count;
            if (count <= 0) throw new AccessViolationException();
            var item = iList[count - 1];
            iList.RemoveAt(count - 1);
            return item;

        }

        /// <summary>
        /// Try to remove last item in list
        /// </summary>
        /// <param name="last"></param>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public bool TryRemoveLast([MaybeNullWhen(false)] out T last)
        {
            try
            {
                last = iList.RemoveLast();
                return true;
            }
            catch 
            {
                last = default;
                return false; 
            }
        }

        /// <summary>
        /// Try to remove last item in list
        /// </summary>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public bool TryRemoveLast()
            => iList.TryRemoveLast(out  _);
    }
}
