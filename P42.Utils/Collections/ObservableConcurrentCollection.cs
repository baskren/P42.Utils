using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace P42.Utils
{
    public class ObservableConcurrentCollection<T> : ObservableCollection<T>
    {
        protected object _lock = new object();
        //protected SemaphoreSlim Lock = new SemaphoreSlim(1, 1);

        protected override void ClearItems()
        {
            lock (_lock)
                base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            lock (_lock)
            {
                if (index > Count)
                    index = Count;
                base.InsertItem(index, item);
            }
        }

        protected override void RemoveItem(int index)
        {
            if (index >= Count)
                return;
            lock (_lock)
                base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            lock (_lock)
            {
                if (index > Count)
                    index = Count;
                base.SetItem(index, item);
            }
        }

        public T[] GetArray()
        {
            lock (_lock)
            {
                var result = new T[Count];
                CopyTo(result, 0);
                return result;
            }
        }

        public List<T> GetList()
        {
            lock (_lock)
            {
                return new List<T>(this);
            }
        }

        protected bool _editingRange;
        public virtual NotifyCollectionChangedEventArgs AddRange(IEnumerable<T> range)
        {
            if (!range.Any())
                return null;
            _editingRange = true;
            foreach (var item in range)
                Add(item);
            _editingRange = false;
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, range);
            OnCollectionChanged(args);
            return args;
        }

        public virtual NotifyCollectionChangedEventArgs RemoveRange(IEnumerable<T> range)
        {
            if (!range.Any())
                return null;
            _editingRange = true;
            foreach (var item in range)
                Remove(item);
            _editingRange = false;
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, range);
            OnCollectionChanged(args);
            return args;
        }

        public virtual NotifyCollectionChangedEventArgs AddAndRemoveRanges(IEnumerable<T> addRange, IEnumerable<T> removeRange)
        {
            if (!addRange.Any() && !removeRange.Any())
                return null;
            _editingRange = true;
            foreach (var item in removeRange)
                Remove(item);
            foreach (var item in addRange)
                Add(item);
            _editingRange = false;
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, addRange, removeRange);
            OnCollectionChanged(args);
            return args;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_editingRange)
                base.OnCollectionChanged(e);
        }
    }
}