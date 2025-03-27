using System.Linq;

namespace P42.Utils.Uno;


internal class WorkaroundBindingCollection : ObservableConcurrentCollection<WorkaroundBinding>
{
    
    public WorkaroundBindingCollection()
    {
        CollectionChanged += (_, e) =>
        {
            if (e.OldItems == null)
                return;
            
            var oldItems = e.OldItems.Cast<WorkaroundBinding>().ToList();

            if (e.NewItems?.Cast<WorkaroundBinding>() is { } newItems)
                foreach (var item in newItems)
                    oldItems.Remove(item);
            
            foreach (var item in oldItems)
                item.Dispose();
        };
    }
    
}
