﻿//-------------------------------------------------------------------------- 
//  
//  Copyright (c) Microsoft Corporation.  All rights reserved.  
//  
//  File: ObservableConcurrentDictionary.cs 
// 
//-------------------------------------------------------------------------- 

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections;
using AsyncAwaitBestPractices;

namespace P42.Utils;

/// <summary> 
/// Provides a thread-safe dictionary for use with data binding. 
/// </summary> 
/// <typeparam name="TKey">Specifies the type of the keys in this collection.</typeparam> 
/// <typeparam name="TValue">Specifies the type of the values in this collection.</typeparam> 
[DebuggerDisplay("Count={Count}")]
public class ObservableConcurrentDictionary<TKey, TValue> :
    IDictionary<TKey, TValue>,
    INotifyCollectionChanged, INotifyPropertyChanged where TKey : notnull
{
    private readonly SynchronizationContext _context;
    private readonly ConcurrentDictionary<TKey, TValue> _dictionary;

    /// <summary> 
    /// Initializes an instance of the ObservableConcurrentDictionary class. 
    /// </summary> 
    public ObservableConcurrentDictionary()
    {
        _context = AsyncOperationManager.SynchronizationContext;
        _dictionary = new ConcurrentDictionary<TKey, TValue>();
    }

    private readonly WeakEventManager _weakNotifyCollectionChanged = new();
    /// <summary>Event raised when the collection changes.</summary> 
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _weakNotifyCollectionChanged.AddEventHandler(value);
        remove => _weakNotifyCollectionChanged.RemoveEventHandler(value);
    }
    
    private readonly WeakEventManager _weakPropertyChanged = new();
    /// <summary>Event raised when a property on the collection changes.</summary> 
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => _weakPropertyChanged.AddEventHandler(value);
        remove => _weakPropertyChanged.RemoveEventHandler(value);
    }

    /// <summary> 
    /// Notifies observers of CollectionChanged or PropertyChanged of an update to the dictionary. 
    /// </summary> 
    private void NotifyObserversOfChange()
    {
            _context.Post(_ =>
            {
                _weakNotifyCollectionChanged.RaiseEvent(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset), nameof(CollectionChanged));

                _weakPropertyChanged.RaiseEvent(this, new PropertyChangedEventArgs("Count"), nameof(PropertyChanged));
                _weakPropertyChanged.RaiseEvent(this, new PropertyChangedEventArgs("Keys"), nameof(PropertyChanged));
                _weakPropertyChanged.RaiseEvent(this, new PropertyChangedEventArgs("Values"), nameof(PropertyChanged));
            }, null);
    }

    /// <summary>Attempts to add an item to the dictionary, notifying observers of any changes.</summary> 
    /// <param name="item">The item to be added.</param> 
    /// <returns>Whether the add was successful.</returns> 
    public bool TryAddWithNotification(KeyValuePair<TKey, TValue> item)
        => TryAddWithNotification(item.Key, item.Value);
    

    /// <summary>Attempts to add an item to the dictionary, notifying observers of any changes.</summary> 
    /// <param name="key">The key of the item to be added.</param> 
    /// <param name="value">The value of the item to be added.</param> 
    /// <returns>Whether the add was successful.</returns> 
    private bool TryAddWithNotification(TKey key, TValue value)
    {
        var result = _dictionary.TryAdd(key, value);
        if (result) NotifyObserversOfChange();
        return result;
    }

    /// <summary>Attempts to remove an item from the dictionary, notifying observers of any changes.</summary> 
    /// <param name="key">The key of the item to be removed.</param> 
    /// <param name="value">The value of the item removed.</param> 
    /// <returns>Whether the removal was successful.</returns> 
    private bool TryRemoveWithNotification(TKey key, out TValue? value)
    {
        value = default;
        if (!_dictionary.TryRemove(key, out var value1))
            return false;

        NotifyObserversOfChange();
        value = value1;
        return true;
    }

    /// <summary>Attempts to add or update an item in the dictionary, notifying observers of any changes.</summary> 
    /// <param name="key">The key of the item to be updated.</param> 
    /// <param name="value">The new value to set for the item.</param> 
    /// <returns>Whether the update was successful.</returns> 
    private void UpdateWithNotification(TKey key, TValue value)
    {
        _dictionary[key] = value;
        NotifyObserversOfChange();
    }

    #region ICollection<KeyValuePair<TKey,TValue>> Members 
    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="item"></param>
    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        TryAddWithNotification(item);
    }

    /// <summary>
    /// clear items
    /// </summary>
    void ICollection<KeyValuePair<TKey, TValue>>.Clear()
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
        NotifyObserversOfChange();
    }

    /// <summary>
    /// test if contains item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
    }

    /// <summary>
    /// Copy items to array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Count of items
    /// </summary>
    int ICollection<KeyValuePair<TKey, TValue>>.Count
    {
        get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count; }
    }

    /// <summary>
    /// Is read only?
    /// </summary>
    bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
    {
        get { return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly; }
    }

    /// <summary>
    /// Remove item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        => TryRemoveWithNotification(item.Key, out _);
        
    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members 
    /// <summary>
    /// Get enumerable
    /// </summary>
    /// <returns></returns>
    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }

    /// <summary>
    /// get enumerator
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
    }
    #endregion

    #region IDictionary<TKey,TValue> Members 
    /// <summary>
    /// Add member
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Add(TKey key, TValue value)
    {
        TryAddWithNotification(key, value);
    }

    /// <summary>
    /// test if member with key exists
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    /// <summary>
    /// Gets collection of keys
    /// </summary>
    public ICollection<TKey> Keys
    {
        get { return _dictionary.Keys; }
    }

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
    public bool TryGetValue(TKey key, out TValue value)
        => _dictionary.TryGetValue(key, out value!);
        

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public IEnumerator GetEnumerator()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// get collection of values
    /// </summary>
    public ICollection<TValue> Values
    {
        get { return _dictionary.Values; }
    }

    /// <summary>
    /// get value for key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TValue this[TKey key]
    {
        get { return _dictionary[key]; }
        set { UpdateWithNotification(key, value); }
    }
    #endregion

    /// <summary>
    /// Clear members
    /// </summary>
    public void Clear()
    {
        foreach (var key in Keys)
            Remove(key);
    }
}
