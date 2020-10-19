using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace P42.Utils
{
    public class OneWayCollectionUnnester<T> : ObservableCollection<T>
    {
        #region Fields
        Dictionary<IList<T>, OneWayCollectionUnnester<T>> _subFlatteners = new Dictionary<IList<T>, OneWayCollectionUnnester<T>>();
        #endregion


        #region Properties
        IList<T> _source;
        public IList<T> Source
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
        #endregion


        #region Construction / Initialization
        public OneWayCollectionUnnester()  
        {
        }

        public OneWayCollectionUnnester(IList<T> source) : this()
            => Source = source;

        void CreateStore()
        {
            foreach (var item in _source)
                StoreAdd(item);
        }
        #endregion


        public override string ToString()
        {
            foreach (var item in this)
            {
                System.Diagnostics.Debug.WriteLine("\t ITEM: " + item.ToString()); ;
            }
            return base.ToString();
        }

        #region Collection Management
        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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
                var preceedingItem = this[startingIndex - 1];
                var preceedingStoreIndex = IndexOf(preceedingItem);
                if (preceedingStoreIndex < 0 || preceedingStoreIndex >= Count)
                    throw new InvalidOperationException("Cannot find preceding item in _store");
                startIndex = preceedingStoreIndex + 1;
            }
            var index = startIndex;
            for (int i = 0; i < newItems.Count; i++)
                index = StoreInsert(index, newItems[i]);
        }

        void StoreAdd(object item)
        {
            StoreInsert(Count, item);
        }

        // returns next index
        int StoreInsert(int index, object item)
        {
            Insert(index++, (T)item);
            if (item is IList<T> list)
            {
                var subFlattener = new OneWayCollectionUnnester<T>(list);
                _subFlatteners.Add(list, subFlattener);
                for (int i = 0; i < subFlattener.Count; i++)
                    Insert(index++, subFlattener[i]);
                subFlattener.CollectionChanged += OnSubFlattenerCollectionChanged;
            }
            return index;
        }

        void StoreRemove(object item)
        {
            if (item is IList<T> list && _subFlatteners.TryGetValue(list, out OneWayCollectionUnnester<T> subFlattener))
            {
                var subItems = subFlattener.ToArray();
                subFlattener.CollectionChanged -= OnSubFlattenerCollectionChanged;
                subFlattener.StoreClear();
                foreach (var subItem in subItems)
                    Remove(subItem);
            }
            Remove((T)item);
        }

        void StoreClear()
        {
            foreach (var subFlattener in _subFlatteners)
            {
                subFlattener.Value.CollectionChanged -= OnSubFlattenerCollectionChanged;
                subFlattener.Value.StoreClear();
            }
            _subFlatteners.Clear();
            Clear();
        }
        #endregion


        #region SubFlatter Collection Change Handler
        private void OnSubFlattenerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is OneWayCollectionUnnester<T> subFlattener)
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
        }

        void SubStoreInsert(OneWayCollectionUnnester<T> subFlattener, int newStartingIndex, IList newItems)
        {
            var startIndex = IndexOf((T)subFlattener.Source) + 1;
            startIndex += newStartingIndex;
            for (int i = 0; i < newItems.Count; i++)
                Insert(startIndex + i, (T)newItems[i]);
        }

        void SubStoreRemove(OneWayCollectionUnnester<T> subFlattener, int oldStartingIndex, IList oldItems)
        {
            /*
            var startIndex = IndexOf((T)subFlattener.Source) + 1;
            startIndex += oldStartingIndex;
            for (int i = 0; i < oldItems.Count; i++)
                RemoveAt(startIndex);
            */
            foreach (var item in oldItems)
                if (item is T tItem)
                    Remove(tItem);
        }

        #endregion



    }
}
