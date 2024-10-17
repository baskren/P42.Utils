using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AsyncAwaitBestPractices;

namespace P42.Utils;

/// <summary>
/// Keyed collection (like dictionary but keys are provided by items) that is observable
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged where TKey : notnull
{
    // Overrides a lot of methods that can cause collection change
    /// <summary>
    /// Set item at index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected override void SetItem(int index, TItem item)
    {
        if (index < 0 || index > Count)
            return;
        if (index == Count)
        {
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        else
        {
            var oldItem = base[index];
            base.SetItem(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
        }
    }

    /// <summary>
    /// Insert item at index
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    protected override void InsertItem(int index, TItem item)
    {
        base.InsertItem(index, item);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
    }

    /// <summary>
    /// Clear items
    /// </summary>
    protected override void ClearItems()
    {
        if (Dictionary is null)
            throw new Exception("Backing Dictionary is null");
            
        var oldItems = this.ToList();
        base.ClearItems();
        Dictionary.Clear();
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    /// <summary>
    /// Remove item at index
    /// </summary>
    /// <param name="index"></param>
    protected override void RemoveItem(int index)
    {
        if (Dictionary is null)
            throw new Exception("Backing Dictionary is null");
            
        var item = this[index];            
        base.RemoveItem(index);
        if (TryGetKey(item, out var key))
            Dictionary.Remove(key!);
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
    }

    /// <summary>
    /// Called when item(s) added or removed from collection
    /// </summary>
    /// <param name="e"></param>
    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        => _weakNotifyCollectionChanged.RaiseEvent(this, e, nameof(CollectionChanged));
        
    #region INotifyCollectionChanged Members
    private readonly WeakEventManager _weakNotifyCollectionChanged = new();
    /// <summary>Event raised when the collection changes.</summary> 
    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => _weakNotifyCollectionChanged.AddEventHandler(value);
        remove => _weakNotifyCollectionChanged.RemoveEventHandler(value);
    }

    #endregion

    /// <summary>
    /// Method to get key for item : MUST OVERRIDE!!!
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override TKey GetKeyForItem(TItem item)
    {
        if (TryGetKey(item, out var key))
            return key!;
        throw new Exception("No key for item");
    }

    public virtual bool TryGetKey(TItem item, out TKey? key)
    {
        if (Dictionary is null)
            throw new Exception("Backing Dictionary is null");
            
        key = default;
        return Dictionary.TryGetKey(item, out key);
    }

    /// <summary>
    /// Test if item with key is in collection
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public virtual bool ContainsKey(TKey key)
    {
        if (Dictionary is null)
            throw new Exception("Backing Dictionary is null");
            
        return Dictionary?.ContainsKey(key) ?? false;
    }
}
