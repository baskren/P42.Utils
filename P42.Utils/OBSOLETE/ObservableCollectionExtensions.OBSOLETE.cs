using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace P42.Utils;

/// <summary>
/// ObservableCollection extensions
/// </summary>
public static class ObservableCollectionExtensions
{

    /// <summary>
    /// Sync ObservableCollection with an IList so as not to cause a ListView to reset its scroll offset 
    /// </summary>
    /// <param name="collection"></param>
    /// <param name="newOrder"></param>
    /// <param name="isReset"></param>
    /// <typeparam name="T"></typeparam>
    [Obsolete("Use ObservableCollection<T>.SyncWith() instead.", true)]
    public static void UpdateFrom<T>(this ObservableCollection<T> collection, IList<T>? newOrder, bool isReset = false)
    {
        if (isReset)
            collection.Clear();

        if (newOrder is null || !newOrder.Any())
        {
            collection.Clear();
            return;
        }

        if (!collection.Any())
        {
            collection.AddRange(newOrder);
            return;
        }
        
        for (var i = 0; i < newOrder.Count; i++)
        {
            var newOrderItem = newOrder[i];
            if (newOrderItem is null)
                throw new InvalidDataException($"UpdateFrom<{typeof(T)}> cannot process null items");
            
            if (i >= collection.Count)
                collection.Add(newOrderItem);
            else
            {
                var oldOrderItem = collection[i];
                if (newOrderItem.Equals(oldOrderItem))
                    continue;

                if (collection.IndexOf(newOrderItem) >= 0)
                {
                    // the item is already in the oldOrder - so we're going to have to move it ... UNLESS oldOrderItem (or a series of items, starting at oldOrderItem) needs to be deleted first
                    while (newOrder.IndexOf(oldOrderItem) is -1)
                    {
                        collection.RemoveAt(i);
                        oldOrderItem = collection[i];
                    }

                    // ok, we've purged out the deleted items, we're starting all over again ...
                    if (!newOrderItem.Equals(oldOrderItem))
                        // we're not going to move the oldOrderItem because the indexes are going to change after we do the following ...
                        collection.Insert(i, newOrderItem);
                    
                }
                else
                    collection.Insert(i, newOrderItem);
                
            }
        }

        while (collection.Count > newOrder.Count)
        {
            collection.RemoveAt(collection.Count - 1);
        }
    }
}
