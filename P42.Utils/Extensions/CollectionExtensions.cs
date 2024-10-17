using System.Collections.Generic;

namespace P42.Utils
{
    public static class CollectionExtensions
    {
        
        /// <summary>
        /// Is collection null or empty
        /// </summary>
        /// <param name="collection"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>true on success</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T>? collection)
            => collection == null || collection.Count == 0;
        

        /// <summary>
        /// Add items to a collection
        /// </summary>
        /// <param name="collection">Destination</param>
        /// <param name="range">Source</param>
        /// <typeparam name="T">Item type</typeparam>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
        {
            foreach (var item in range)
                collection.Add(item);
        }


    }
}
