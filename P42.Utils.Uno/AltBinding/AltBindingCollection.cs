
namespace P42.Utils.Uno;


internal class AltBindingCollection : ObservableConcurrentCollection<AltBinding>
{
    
    public AltBindingCollection()
    {
        CollectionChanged += (_, e) =>
        {
            if (e.OldItems == null)
                return;
            
            var oldItems = e.OldItems.Cast<AltBinding>().ToList();

            if (e.NewItems?.Cast<AltBinding>() is { } newItems)
                foreach (var item in newItems)
                    oldItems.Remove(item);
            
            foreach (var item in oldItems)
                item.Dispose();
        };
    }
    
}
