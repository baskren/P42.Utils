
namespace P42.Utils;

/// <summary>
/// ICollectionExtensions
/// </summary>
// ReSharper disable once UnusedType.Global
public static class ICollectionExtensions
{

    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    // ReSharper disable once UnusedType.Global
    extension<T>(ICollection<T> collection)
    {
        /// <summary>
        /// Add range to collection
        /// </summary>
        /// <param name="range"></param>
        public ICollection<T> AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
                collection.Add(item);
        
            return collection;
        }

        /// <summary>
        /// Remove range from collection
        /// </summary>
        /// <param name="range"></param>
        public ICollection<T> RemoveRange(IEnumerable<T> range)
        {
            foreach (var item in range)
                collection.Remove(item);
        
            return collection;
        }
        
        /// <summary>
        /// No members?
        /// </summary>
        public bool IsEmpty => collection.Count == 0;

    }
}
