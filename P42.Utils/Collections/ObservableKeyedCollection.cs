using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace P42.Utils
{
    /// <summary>
    /// Keyed collection (like dictionary but keys are provided by items) that is observable
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
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
            var oldItems = this.ToList();
            base.ClearItems();
            Dictionary?.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        /// Remove item at index
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            var item = this[index];            
            base.RemoveItem(index);
            if (TryGetKey(item, out var key))
                Dictionary?.Remove(key);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        /// <summary>
        /// Called when item(s) added or removed from collection
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
            => CollectionChanged?.Invoke(this, e);
        
        #region INotifyCollectionChanged Members
        /// <summary>
        /// Event fired when collection has changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        /// <summary>
        /// Method to get key for item : MUST OVERRIDE!!!
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override TKey GetKeyForItem(TItem item)
        {
            if (Dictionary is not null &&  Dictionary.TryGetKey(item, out TKey key))
                return (TKey)key;
            throw new System.Exception("No key for item");
        }

        public virtual bool TryGetKey(TItem item, out TKey key)
        {
            if (Dictionary is not null)
                return Dictionary.TryGetKey(item, out key);
            key = default;
            return false;
        } 

        /// <summary>
        /// Test if item with key is in collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool ContainsKey(TKey key)
        {
            return Dictionary?.ContainsKey(key) ?? false;
        }
    }

}

