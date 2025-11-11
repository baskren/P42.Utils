using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace P42.Utils;

/// <summary> 
/// Provides a thread-safe dictionary for use with data binding. 
/// </summary> 
/// <typeparam name="TKey">Specifies the type of the keys in this collection.</typeparam> 
/// <typeparam name="TValue">Specifies the type of the values in this collection.</typeparam> 
[DebuggerDisplay("Count={Count}")]
// ReSharper disable once UnusedType.Global
public class ObservableConcurrentDictionary<TKey, TValue> :
    IDictionary<TKey, TValue>,
    INotifyCollectionChanged, INotifyPropertyChanged where TKey : notnull
{
    #region Fields
    private readonly SynchronizationContext _context = new();
    private readonly ConcurrentDictionary<TKey, TValue> _dictionary = [];
    #endregion Fields
    
    #region Events
    private readonly AsyncAwaitBestPractices.WeakEventManager _collectionChange = new();
    /// <summary>Event raised when the collection changes.</summary> 
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _collectionChange.AddEventHandler(value);
        remove => _collectionChange.RemoveEventHandler(value);
    }
    
    private readonly AsyncAwaitBestPractices.WeakEventManager _propertyChanged = new();
    /// <summary>Event raised when a property on the collection changes.</summary> 
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => _propertyChanged.AddEventHandler(value);
        remove => _propertyChanged.RemoveEventHandler(value);
    }
    #endregion Events

    /// <summary> 
    /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary. 
    /// </summary> 
    private void NotifyObserversOfChange(NotifyCollectionChangedEventArgs e)
    {
        _context.Post(_ =>
        {
            _collectionChange.RaiseEvent(this, e, nameof(CollectionChanged));
            _propertyChanged.RaiseEvent(this, new PropertyChangedEventArgs(nameof(Count)), nameof(PropertyChanged));
            _propertyChanged.RaiseEvent(this, new PropertyChangedEventArgs(nameof(Keys)), nameof(PropertyChanged));
            _propertyChanged.RaiseEvent(this, new PropertyChangedEventArgs(nameof(Values)), nameof(PropertyChanged));
        }, null);
        
    }

    /// <summary>Attempts to add an item to the dictionary, notifying observers of any changes.</summary> 
    /// <param name="item">The item to be added.</param> 
    /// <returns>Whether the add was successful.</returns> 
    // ReSharper disable once UnusedMethodReturnValue.Local
    private bool TryAddWithNotification(KeyValuePair<TKey, TValue> item)
        => TryAddWithNotification(item.Key, item.Value);
    

    /// <summary>Attempts to add an item to the dictionary, notifying observers of any changes.</summary> 
    /// <param name="key">The key of the item to be added.</param> 
    /// <param name="value">The value of the item to be added.</param> 
    /// <returns>Whether the add was successful.</returns> 
    private bool TryAddWithNotification(TKey key, TValue value)
    {
        var index = Count;
        if (!_dictionary.TryAdd(key, value))
            return false;

        var kvp = new KeyValuePair<TKey, TValue>(key, value);
        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, kvp, index);
        NotifyObserversOfChange(args);
        return true;
    }

    /// <summary>Attempts to remove an item from the dictionary, notifying observers of any changes.</summary> 
    /// <param name="key">The key of the item to be removed.</param> 
    /// <param name="value">The value of the item removed.</param> 
    /// <returns>Whether the removal was successful.</returns> 
    // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Local
    private bool TryRemoveWithNotification(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (!_dictionary.TryRemove(key, out value))
            return false;

        var kvp = new KeyValuePair<TKey, TValue>(key, value);
        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, kvp);
        NotifyObserversOfChange(args);
        return true;
    }

    /// <summary>Attempts to add or update an item in the dictionary, notifying observers of any changes.</summary> 
    /// <param name="key">The key of the item to be updated.</param> 
    /// <param name="value">The new value to set for the item.</param> 
    /// <returns>Whether the update was successful.</returns> 
    private void UpdateWithNotification(TKey key, TValue value)
    {
        TryRemoveWithNotification(key, out _);
        TryAddWithNotification(key, value);
        /* Since when was NotifyCollectionChangedAction.Relace not supported in the constructor???
        _dictionary[key] = value;
        foreach (var (index,pair) in _dictionary.WithIndex())
        {
            if (!pair.Key.Equals(key))
                continue;

            var kvp = new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, kvp, index);
            NotifyObserversOfChange(args);
            return;
        }
        */
    }

    #region ICollection<KeyValuePair<TKey,TValue>> Members 
    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="item"></param>
    public void Add(KeyValuePair<TKey, TValue> item)
        => TryAddWithNotification(item);
    

    /// <summary>
    /// clear items
    /// </summary>
    public void Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
        NotifyObserversOfChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// test if contains item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
        => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    

    /// <summary>
    /// Copy items to array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    

    /// <summary>
    /// Count of items
    /// </summary>
    public int Count
        => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count; 
    

    /// <summary>
    /// Is read only?
    /// </summary>
    public bool IsReadOnly
        => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly; 
    

    /// <summary>
    /// Remove item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
        => TryRemoveWithNotification(item.Key, out _);
    
    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members 
    /// <summary>
    /// Get enumerable
    /// </summary>
    /// <returns></returns>
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    

    /// <summary>
    /// get enumerator
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator()
        => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    
    #endregion

    #region IDictionary<TKey,TValue> Members 
    /// <summary>
    /// Add member
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
        => TryAddWithNotification(key, value);
    

    /// <summary>
    /// test if member with key exists
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(TKey key)
        => _dictionary.ContainsKey(key);
    

    /// <summary>
    /// Gets collection of keys
    /// </summary>
    public ICollection<TKey> Keys
        => _dictionary.Keys; 
    

    /// <summary>
    /// removes item at key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool Remove(TKey key)
        => TryRemoveWithNotification(key, out _);
    

    /// <summary>
    /// Try to get value for key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        => _dictionary.TryGetValue(key, out value);
    

    /// <summary>
    /// get collection of values
    /// </summary>
    public ICollection<TValue> Values
        => _dictionary.Values; 
    

    /// <summary>
    /// get value for key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue this[TKey key]
    {
        get => _dictionary[key]; 
        set => UpdateWithNotification(key, value); 
    }
    #endregion

}
