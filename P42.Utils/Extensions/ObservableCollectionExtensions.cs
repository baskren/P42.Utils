using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace P42.Utils
{
public static class ObservableCollectionExtensions
{

    public static void UpdateFrom<T>(this ObservableCollection<T> collection, IList<T> newOrder, bool isReset = false)
    {
        if (collection is null)
            return;

        if (isReset)
            collection.Clear();

        if (newOrder is null || !newOrder.Any())
        {
            collection.Clear();
            return;
        }

        else if (!collection.Any())
        {
            collection.AddRange(newOrder);
            return;
        }
        else
        {
            for (int i = 0; i < newOrder.Count; i++)
            {
                var newOrderItem = newOrder[i];
                if (i >= collection.Count)
                {
                    collection.Add(newOrderItem);
                }
                else
                {
                    var oldOrderItem = collection[i];
                    if (!newOrderItem.Equals(oldOrderItem))
                    {
                        if (collection.IndexOf(newOrderItem) is int oldIndex && oldIndex >= 0)
                        {
                            // the item is already in the oldOrder - so we're going to have to move it ... UNLESS oldOrderItem (or a series of items, starting at oldOrderItem) needs to be deleted first
                            while (newOrder.IndexOf(oldOrderItem) is int newIndex && newIndex == -1)
                            {
                                collection.RemoveAt(i);
                                oldOrderItem = collection[i];
                            }

                            // ok, we've purged out the deleted items, we're starting all over again ...
                            if (!newOrderItem.Equals(oldOrderItem))
                            {
                                // we're not going to move the oldOrderItem because the indexes are going to change after we do the following ...
                                collection.Insert(i, newOrderItem);
                            }
                        }
                        else
                        {
                            collection.Insert(i, newOrderItem);
                        }

                    }
                }
            }

            while (collection.Count > newOrder.Count)
            {
                collection.RemoveAt(collection.Count - 1);
            }
        }
    }
}
}
