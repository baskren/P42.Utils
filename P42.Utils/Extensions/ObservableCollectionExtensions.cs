using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace P42.Utils;

public static class ObservableCollectionExtensions
{

    public static void UpdateFrom<T>(this ObservableCollection<T> collection, IList<T> newOrder, bool isReset = false)
    {
        if (isReset)
            collection.Clear();

        if (!newOrder.Any())
        {
            collection.Clear();
            return;
        }

        if (!collection.Any())
        {
            collection.AddRange(newOrder);
            return;
        }
        
        for (var newOrderIndex = 0; newOrderIndex < newOrder.Count; newOrderIndex++)
        {
            var newOrderItem = newOrder[newOrderIndex];
            if (newOrderIndex >= collection.Count)
            {
                collection.Add(newOrderItem);
            }
            else
            {
                var oldOrderItem = collection[newOrderIndex];
                if (oldOrderItem is null && newOrderItem is null)
                    continue;
                if (newOrderItem?.Equals(oldOrderItem) ?? false)
                    continue;

                if (!collection.Contains(newOrderItem))
                {
                    collection.Insert(newOrderIndex, newOrderItem);
                    continue;                    
                }
                
                // the item is already in the oldOrder - so we're going to have to move it ... UNLESS oldOrderItem (or a series of items, starting at oldOrderItem) needs to be deleted first
                while (!newOrder.Contains(oldOrderItem))
                {
                    collection.RemoveAt(newOrderIndex);
                    oldOrderItem = collection[newOrderIndex];
                }

                // ok, we've purged out the deleted items, we're starting all over again ...
                if (!newOrderItem!.Equals(oldOrderItem))
                    // we're not going to move the oldOrderItem because the indexes are going to change after we do the following ...
                    collection.Insert(newOrderIndex, newOrderItem);
                
            }
            

        }
        
        while (collection.Count > newOrder.Count)
        {
            collection.RemoveAt(collection.Count - 1);
        }
    }
}
