using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace P42.Utils;

/// <summary>
/// IDictionary extensions
/// </summary>
public static class IDictionaryExtensions
{
    /// <summary>
    /// Add range to IDictionary
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TQ"></typeparam>
    /// <returns></returns>
    public static IDictionary<T,TQ> AddRange<T,TQ>(this IDictionary<T,TQ> dictionary, IDictionary<T,TQ> range)
    {
        foreach (var item in range)
            dictionary[item.Key] = item.Value;
        
        return dictionary;
    }

    /// <summary>
    /// does a dictionary contain a key that contains a subkey
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="subkey"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [Obsolete("Use LINQ instead?")]
    public static bool ContainsKeyThatContains<TValue>(this IDictionary<string, TValue> dictionary, string subkey)
        => dictionary.Keys.Any(key => key.Contains(subkey));
    
    /// <summary>
    /// Find keys in dictionary that contain subkey
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="subkey"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [Obsolete("Use LINQ instead?")]
    public static List<string> KeysThatContain<TValue>(this IDictionary<string, TValue> dictionary, string subkey)
        => dictionary.Keys.Where(key => key.Contains(subkey)).ToList();
        

    /// <summary>
    /// Find items in dictionary whose keys contain subkey 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="subkey"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    [Obsolete("Use LINQ instead?")]
    public static List<TValue> ItemsWithKeysThatContain<TValue>(this IDictionary<string, TValue> dictionary, string subkey)
        => (from key in dictionary.Keys where key.Contains(subkey) select dictionary[key]).ToList();
        

    /// <summary>
    /// Try to get a Key from a dictionary
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="value"></param>
    /// <param name="key"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static bool TryGetKey<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TValue value, [MaybeNullWhen(false)] out TKey key)
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
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IDictionary<TKey, TValue> FluentAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        dict.Add(key, value); 
        return dict;
    }
}
