namespace P42.Utils.Uno
{
    public static class DataTemplateExtensions
    {
        /*
        public static T GetCellContainer<T>(DataTemplateSet<T> templateItem, T container, Func<T> func) where T : ContentControl
        {

            // args.ItemContainer is used to indicate whether the ListView is proposing an
            // ItemContainer (ListViewItem) to use. If args.Itemcontainer != null, then there was a
            // recycled ItemContainer available to be reused.
            if (container != null)
            {
                if (container.Tag.Equals(templateItem.Type))
                {
                    // Suggestion matches what we want, so remove it from the recycle queue
                    templateItem.RecycleStore.Remove(container);
                }
                else
                {
                    // The ItemContainer's datatemplate does not match the needed
                    // datatemplate.
                    // Don't remove it from the recycle queue, since XAML will resuggest it later
                    container = null;
                }
            }

            // If there was no suggested container or XAML's suggestion was a miss, pick one up from the recycle queue
            // or create a new one
            if (container == null)
            {
                // See if we can fetch from the correct list.
                if (templateItem.RecycleStore.Count > 0)
                {
                    // Unfortunately have to resort to LINQ here. There's no efficient way of getting an arbitrary
                    // item from a hashset without knowing the item. Queue isn't usable for this scenario
                    // because you can't remove a specific element (which is needed in the block above).
                    container = templateItem.RecycleStore.First();
                    templateItem.RecycleStore.Remove(container);
                }
                else
                {
                    // There aren't any (recycled) ItemContainers available. So a new one
                    // needs to be created.

                    container = func.Invoke();
                    // below are necessary ... until I can figure a better way?
                    container.Margin = new Thickness(0);
                    container.Padding = new Thickness(0);
                    container.ContentTemplate = templateItem.Template;
                    container.Tag = templateItem.Type;
                }
            }

            // Indicate to XAML that we picked a container for it
            container.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            return container;
        }
        */

    }
}