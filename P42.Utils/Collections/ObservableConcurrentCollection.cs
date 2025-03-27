using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace P42.Utils;

public class ObservableConcurrentCollection<T> :
    IList<T>,
    INotifyCollectionChanged, INotifyPropertyChanged
{
    #region Properties

    /// <summary>
    /// number of items in collection
    /// </summary>
    public int Count 
    {
        get
        {
            lock (_lock)
                return _collection.Count;
        }
    }

    /// <summary>
    /// Is collection read only?
    /// </summary>
    public bool IsReadOnly 
    { 
        get
        {
            lock (_lock)
                return ((ICollection<T>)_collection).IsReadOnly;
        }
    }
    #endregion
    
    
    #region Fields
    private readonly SynchronizationContext _context = new();
    private readonly ObservableCollection<T> _collection = [];
    private readonly Lock _lock = new();
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
    
    
    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public ObservableConcurrentCollection()
    {
        _collection.CollectionChanged += OnCollectionChanged;
        ((INotifyPropertyChanged)_collection).PropertyChanged += OnPropertyChanged;
        
    }
    #endregion Constructors

    
    #region Base Event Handlers
    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _context.Post(_ =>
        {        
            _propertyChanged.RaiseEvent(this, e, nameof(PropertyChanged));
        }, null);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _context.Post(_ =>
        {
            _collectionChange.RaiseEvent(this,e, nameof(CollectionChanged));
        }, null);
    }
    #endregion
    
    
    #region Methods
    /// <summary>
    /// Provides an enumerator that iterates through the collection
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator()
    {
        lock (_lock)
            return _collection.GetEnumerator();
    }
    
    /// <summary>
    /// Provides an enumerator that iterates through the collection
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        lock (_lock)
            return ((IEnumerable)_collection).GetEnumerator();    
    } 
    
    /// <summary>
    /// Adds an object ot the end of the collection
    /// </summary>
    /// <param name="item"></param>
    public void Add(T item)
    {
        lock (_lock)
            _collection.Add(item);
    }

    /// <summary>
    /// Removes all elements from the collection
    /// </summary>
    public void Clear()
    {
        lock (_lock)
            _collection.Clear();
    }

    /// <summary>
    /// Determines if an element is in the collection
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(T item)
    {
        lock (_lock)
            return _collection.Contains(item);
    }

    /// <summary>
    /// Copies the contents of the collection to a one-dimensional array, starting at the arrayIndex of that array
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(T[] array, int arrayIndex)
    {
        lock (_lock)
            _collection.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes the first occurrence of the item from the collection
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public virtual bool Remove(T item)
    {
        lock (_lock)
            return _collection.Remove(item);
    }


    #endregion ICollection<T> Members


    #region Useful Extension Methods

    /// <summary>
    /// Add range into ObservableConcurrentCollection
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public ObservableConcurrentCollection<T> InsertRange(int index, IList<T> range)
    {
        lock (_lock)
        {
            for (var i = range.Count - 1; i >= 0; i--)
                _collection.Insert(index, range[i]);

            return this;
        }
    }

    /// <summary>
    /// Remove range from ObservableConcurrentCollection
    /// </summary>
    /// <param name="range"></param>
    /// <typeparam name="T"></typeparam>
    public  ObservableConcurrentCollection<T> RemoveRange(IEnumerable<T> range)
    {
        lock (_lock)
        {
            foreach (var item in range)
                _collection.Remove(item);

            return this;
        }
    }

    /// <summary>
    /// Sync ObservableConcurrentCollection with another so as not to cause a ListView to reset its scroll offset
    /// </summary>
    /// <param name="newOrder"></param>
    /// <param name="newReversed"></param>
    /// <param name="isReset"></param>
    /// <typeparam name="T"></typeparam>
    public  ObservableConcurrentCollection<T> SyncWith(IList<T>? newOrder, bool isReset = false, bool newReversed = false)
    {
        lock (_lock)
        {
            if (isReset)
                _collection.Clear();

            if (newOrder is null)
            {
                _collection.Clear();
                return this;
            }

            if (newReversed)
                newOrder = newOrder.Reverse().ToArray();

            for (var i = 0; i < newOrder.Count; i++)
                _collection[i] = newOrder[i];

            for (var i = newOrder.Count; i < _collection.Count; i++)
                _collection.RemoveAt(i);

            return this;
        }
    }

    /// <summary>
    /// Conditionally remove value from IList
    /// </summary>
    /// <param name="conditional"></param>
    /// <typeparam name="T"></typeparam>
    public  ObservableConcurrentCollection<T> RemoveIf(Func<T, bool> conditional)
    {
        lock (_lock)
        {
            var items = this.ToArray();
            foreach (var item in items)
                if (conditional.Invoke(item))
                    _collection.Remove(item);

            return this;
        }
    }
    

    #endregion

    
    #region IList Members
    public int IndexOf(T item)
    {
        lock (_lock)
            return _collection.IndexOf(item);
    }

    public virtual void Insert(int index, T item)
    {
        lock (_lock)
            _collection.Insert(index, item);
    }

    public virtual void RemoveAt(int index)
    {
        lock (_lock)
            _collection.RemoveAt(index);
    }

    public virtual T this[int index]
    {
        get  
        {
            lock(_lock)
                return _collection[index];
        }
        set
        {
            lock (_lock)
                _collection[index] = value;
        }
    }
    
    public bool TryGetValue(int index, [MaybeNullWhen(false)] out T value)
    {
        lock (_lock)
        {
            if (index < 0 || index >= _collection.Count)
            {
                value = default;
                return false;
            }
            value = _collection[index];
            return true;
        }
    }
    #endregion
}
