using System.Collections.Generic;
using System.Linq;

namespace P42.Utils;

public static class DictionaryExtensions
{
    /// <summary>
    /// Look for key that contains the text of the subkey
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="subkey"></param>
    /// <typeparam name="TValue">item type</typeparam>
    /// <returns>true on success</returns>
    public static bool ContainsKeyThatContains<TValue>(this IDictionary<string, TValue> dictionary, string subkey)
        => dictionary.Keys.Any(key => key.Contains(subkey));

    /// <summary>
    /// Enumerate keys in dictionary that contains subkey
    /// </summary>
    /// <param name="dictionary">dictionary</param>
    /// <param name="subkey">subkey</param>
    /// <typeparam name="TValue">item type</typeparam>
    /// <returns>matching keys</returns>
    public static IEnumerable<string> KeysThatContain<TValue>(this IDictionary<string, TValue> dictionary, string subkey)
        => dictionary.Keys.Where(key => key.Contains(subkey));

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="subkey"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TValue> ItemsWithKeysThatContain<TValue>(this IDictionary<string, TValue> dictionary, string subkey)
        => dictionary.Where(kvp => kvp.Key.Contains(subkey)).Select(kvp => kvp.Value);

    /// <summary>
    /// Find key for item in dictionary
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="value"></param>
    /// <param name="key"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, out TKey? key)
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
    /// Add item to dictionary
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns>dictionary</returns>
    public static IDictionary<TKey, TValue> FluintAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        TValue value)
    {
        dict.Add(key, value); 
        return dict;
    }
        
    /// <summary>
    /// Add range to dictionary
    /// </summary>
    /// <param name="dictionary">destination dictionary</param>
    /// <param name="range">source to add</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Q"></typeparam>
    public static IDictionary<T, Q> AddRange<T,Q>(this IDictionary<T,Q> dictionary, IDictionary<T,Q> range)
    {
        foreach (var item in range)
            dictionary[item.Key] = item.Value;
        
        return dictionary;
    }
        
        
}
