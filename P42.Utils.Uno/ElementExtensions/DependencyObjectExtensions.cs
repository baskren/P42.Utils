using System.Reflection;

namespace P42.Utils.Uno;

public static class DependencyObjectExtensions
{
    /// <param name="parent"></param>
    extension(DependencyObject parent)
    {
        /// <summary>
        /// Find's child of a FrameworkElement by name
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public DependencyObject? FindChildByName(string controlName)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement element && element.Name == controlName)
                    return element;

                var findResult = child.FindChildByName(controlName);
                if (findResult != null)
                    return findResult;
            }

            return null;
        }

        /// <summary>
        /// Gets the Children of a DependencyObject
        /// </summary>
        /// <param name="strictTypeCheck">true: will not check for derived classes</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> FindChildren<T>(bool strictTypeCheck = true) where T : DependencyObject
        {
            var results = new List<T>();
            FindChildrenInternal(results, parent, strictTypeCheck);
            return results;
        }
    }

    internal static void FindChildrenInternal<T>(List<T> results, DependencyObject? startNode, bool strictTypeCheck) where T : DependencyObject
    {
        startNode ??= Platform.MainWindow.Content;

        var count = VisualTreeHelper.GetChildrenCount(startNode);
        for (var i = 0; i < count; i++)
        {
            var current = VisualTreeHelper.GetChild(startNode, i);
            if (current is T || (!strictTypeCheck && current.GetType().GetTypeInfo().IsSubclassOf(typeof(T))))
            {
                var asType = (T)current;
                results.Add(asType);
            }
            FindChildrenInternal(results, current, strictTypeCheck);
        }
    }

}
