using System.Diagnostics.CodeAnalysis;

namespace P42.Utils;

/// <summary>
/// IDictionary extensions
/// </summary>
public static class IDictionaryExtensions
{
    /// <param name="dictionary"></param>
    /// <typeparam name="TValue"></typeparam>
    // ReSharper disable once UnusedType.Global
    extension<TValue>(IDictionary<string, TValue> dictionary)
    {
        /// <summary>
        /// does a dictionary contain a key that contains a subkey
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        [Obsolete("Use LINQ instead?")]
        public bool ContainsKeyThatContains(string subkey)
            => dictionary.Keys.Any(key => key.Contains(subkey));

        /// <summary>
        /// Find keys in dictionary that contain subkey
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        [Obsolete("Use LINQ instead?")]
        public List<string> KeysThatContain(string subkey)
            => dictionary.Keys.Where(key => key.Contains(subkey)).ToList();

        /// <summary>
        /// Find items in dictionary whose keys contain subkey 
        /// </summary>
        /// <param name="subkey"></param>
        /// <returns></returns>
        [Obsolete("Use LINQ instead?")]
        public List<TValue> ItemsWithKeysThatContain(string subkey)
            => (from key in dictionary.Keys where key.Contains(subkey) select dictionary[key]).ToList();
    }


    /// <param name="dictionary"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    extension<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
    {
        /// <summary>
        /// Try to get a Key from a dictionary
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetKey(TValue value, [MaybeNullWhen(false)] out TKey key)
        {
            foreach (var kvp in dictionary)
            {
                if (!Equals(value, kvp.Value))
                    continue;

                key = kvp.Key;
                return true;
            }
        
            key = default;
            return false;
        }

        /// <summary>
        /// Add item to dictionary in Fluent style
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDictionary<TKey, TValue> FluentAdd(TKey key, TValue value)
        {
            dictionary.Add(key, value); 
            return dictionary;
        }

        /// <summary>
        /// Add range to IDictionary
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public IDictionary<TKey,TValue> AddRange(IDictionary<TKey,TValue> range)
        {
            foreach (var item in range)
                dictionary[item.Key] = item.Value;
        
            return dictionary;
        }
    }
}
