using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace P42.Utils
{
    public class OneWayCollectionUnnester : IList, INotifyCollectionChanged
    {
        ObservableCollection<object> _store = new ObservableCollection<object>();

        Dictionary<IList, OneWayCollectionUnnester> _subFlatteners = new Dictionary<IList, OneWayCollectionUnnester>();

        IList _source;
        public IList Source
        {
            get => _source;
            private set
            {
                _source = value;
                CreateStore();
                if (_source is INotifyCollectionChanged newNotifySource)
                    newNotifySource.CollectionChanged += OnSourceCollectionChanged;
            }
        }


        #region Construction / Initialization
        public OneWayCollectionUnnester()  { }

        public OneWayCollectionUnnester(IList source) : this()
            => Source = source;

        void CreateStore()
        {
            foreach (var item in _store)
            {
                _store.Add(item);
                if (item is IList subSource)
                {
                    var subFlattener = new OneWayCollectionUnnester(subSource);
                    _subFlatteners.Add(subSource, subFlattener);
                    _store.Add(subFlattener.GetEnumerator());
                    subFlattener.CollectionChanged += OnSubFlattenerCollectionChanged;
                }
            }
        }
        #endregion


        #region Collection Management
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsSynchronized = false;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                StoreInsert(e.NewStartingIndex, e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                StoreRemove(e.OldItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                StoreClear();
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace || e.Action == NotifyCollectionChangedAction.Move)
            {
                StoreRemove(e.OldItems);
                StoreInsert(e.NewStartingIndex, e.NewItems);
            }
            IsSynchronized = true;
        }

        void StoreRemove(IList oldItems)
        {
            foreach (var oldItem in oldItems)
                StoreRemove(oldItem);
        }

        void StoreInsert(int startingIndex, IList newItems)
        {
            int startIndex = 0;
            if (startingIndex > 0)
            {
                var preceedingItem = _source[startingIndex - 1];
                var preceedingStoreIndex = _store.IndexOf(preceedingItem);
                if (preceedingStoreIndex < 0 || preceedingStoreIndex >= _store.Count)
                    throw new InvalidOperationException("Cannot find preceding item in _store");
                startIndex = preceedingStoreIndex + 1;
            }
            var index = startIndex;
            for (int i = 0; i < newItems.Count; i++)
                index = StoreInsert(index, newItems[i]);
        }

        // returns next index
        int StoreInsert(int index, object item)
        {
            _store.Insert(index++, item);
            if (item is IList list)
            {
                var subFlattener = new OneWayCollectionUnnester(list);
                _subFlatteners.Add(list, subFlattener);
                for (int i = 0; i < subFlattener.Count; i++)
                    _store.Insert(index++, subFlattener[i]);
                subFlattener.CollectionChanged += OnSubFlattenerCollectionChanged;
            }
            return index;
        }

        void StoreRemove(object item)
        {
            if (item is IList list && _subFlatteners.TryGetValue(list, out OneWayCollectionUnnester subFlattener))
            {
                var subItems = subFlattener._store.ToArray();
                subFlattener.CollectionChanged -= OnSubFlattenerCollectionChanged;
                subFlattener.StoreClear();
                foreach (var subItem in subItems)
                    _store.Remove(subItem);
            }
            _store.Remove(item);
        }

        void StoreClear()
        {
            foreach (var subFlattener in _subFlatteners)
            {
                subFlattener.Value.CollectionChanged -= OnSubFlattenerCollectionChanged;
                subFlattener.Value.StoreClear();
            }
            _subFlatteners.Clear();
            _store.Clear();
        }
        #endregion


        #region SubFlatter Collection Change Handler
        private void OnSubFlattenerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsSynchronized = false;
            if (sender is OneWayCollectionUnnester subFlattener)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    SubStoreInsert(subFlattener, e.NewStartingIndex, e.NewItems);
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    SubStoreRemove(subFlattener, e.OldStartingIndex, e.OldItems);
                }
                else if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    SubStoreRemove(subFlattener, 0, e.OldItems);
                }
                else if (e.Action == NotifyCollectionChangedAction.Replace)
                {
                    SubStoreRemove(subFlattener, e.OldStartingIndex, e.OldItems);
                    SubStoreInsert(subFlattener, e.NewStartingIndex, e.NewItems);
                }
                else if (e.Action == NotifyCollectionChangedAction.Move)
                {
                    SubStoreRemove(subFlattener, e.OldStartingIndex, e.OldItems);
                    SubStoreInsert(subFlattener, e.NewStartingIndex, e.NewItems);
                }
            }
            IsSynchronized = true;
        }

        void SubStoreInsert(OneWayCollectionUnnester subFlattener, int newStartingIndex, IList newItems)
        {
            var startIndex = _store.IndexOf(subFlattener.Source);
            startIndex += newStartingIndex;
            for (int i = 0; i < newItems.Count; i++)
                _store.Insert(startIndex + i, newItems[i]);
        }

        void SubStoreRemove(OneWayCollectionUnnester subFlattener, int oldStartingIndex, IList oldItems)
        {
            var startIndex = _store.IndexOf(subFlattener.Source);
            startIndex += oldStartingIndex;
            for (int i = 0; i < oldItems.Count; i++)
                _store.RemoveAt(startIndex);
        }
        #endregion


        #region ICollectionChanged
        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add
            {
                ((INotifyCollectionChanged)_store).CollectionChanged += value;
            }

            remove
            {
                ((INotifyCollectionChanged)_store).CollectionChanged -= value;
            }
        }
        #endregion


        #region IList

        public object this[int index]
        {
            get => _store[index];
            set => throw new NotImplementedException();
        }

        public bool IsFixedSize
            => false;

        public bool IsReadOnly
            => true;

        public int Add(object item)
            => throw new NotImplementedException();

        public void Clear()
            => throw new NotImplementedException();

        public bool Contains(object item)
            => _store.Contains(item);

        public int IndexOf(object item)
            => _store.IndexOf(item);

        public void Insert(int index, object item)
            => throw new NotImplementedException();

        public void Remove(object item)
            => throw new NotImplementedException();

        public void RemoveAt(int index)
            => throw new NotImplementedException();

        #region ICollection

        public int Count
            => _store.Count;

        int _syncCount = 0;
        public bool IsSynchronized
        {
            get => _syncCount <= 0;
            private set
            {
                _syncCount += value ? +1 : -1;
            }
        }

        public object SyncRoot
            => _source;

        public void CopyTo(Array array, int index)
            => _store.ToArray().CopyTo(array, index);

        #region IEnumerable
        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_store).GetEnumerator();
        }

        #endregion

        #endregion

        #endregion
    }
}
