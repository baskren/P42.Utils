using Windows.Foundation;
using Microsoft.UI.Xaml.Markup;
using System.Reflection;
using System.Runtime.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

// ReSharper disable CommentTypo
// ReSharper disable once InconsistentNaming
public static class UIElementExtensions
{
    /// <param name="element"></param>
    extension(FrameworkElement element)
    {
        /// <summary>
        /// Does the element have a prescribed width
        /// </summary>
        /// <returns></returns>
        public bool HasPrescribedWidth() => !double.IsNaN(element.Width) && element.Width >= 0;

        /// <summary>
        /// Does the element have a prescrib`ed height
        /// </summary>
        /// <returns></returns>
        public bool HasPrescribedHeight() => !double.IsNaN(element.Height) && element.Height >= 0;

        /// <summary>
        /// Does the element have a prescribed minimum width
        /// </summary>
        /// <returns></returns>
        public bool HasMinWidth() => !double.IsNaN(element.MinWidth) && element.MinWidth >= 0;

        /// <summary>
        /// Does the element have a prescribed minimum height
        /// </summary>
        /// <returns></returns>
        public bool HasMinHeight() => !double.IsNaN(element.MinHeight) && element.MinHeight >= 0;

        /// <summary>
        /// Does the element have a prescribed maximum width
        /// </summary>
        /// <returns></returns>
        public bool HasMaxWidth() => !double.IsNaN(element.MaxWidth) && element.MaxWidth >= 0;

        /// <summary>
        /// Does the element have a prescribed maximum height
        /// </summary>
        /// <returns></returns>
        public bool HasMaxHeight() => !double.IsNaN(element.MaxHeight) && element.MaxHeight >= 0;

        /// <summary>
        /// Get Bounds of FrameworkElement
        /// </summary>
        /// <param name="relativeTo">default: AppWindow.Frame</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        private Rect GetBounds(UIElement? relativeTo = null)
        {
            relativeTo ??= Platform.Frame;
            var ttv = element.TransformToVisual(relativeTo);
            var location = ttv.TransformPoint(new Point(0, 0));
            return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
        }
    }

    private static MethodInfo? GetActualWidthMethod 
        => field ??= typeof(FrameworkElement).GetMethod("GetActualWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            
    private static MethodInfo? GetActualHeightMethod
        => field ??= typeof(FrameworkElement).GetMethod("GetActualHeight", BindingFlags.NonPublic | BindingFlags.Instance);

    /// <param name="element"></param>
    extension(UIElement element)
    {
        /// <summary>
        /// Get Bounds of UIElement
        /// </summary>
        /// <param name="relativeTo">default: AppWindow.Frame</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private Rect GetBounds(UIElement? relativeTo = null)
        {
            if (element is FrameworkElement fe)
                return GetBounds(fe, relativeTo);
        
            relativeTo ??= Platform.Frame;
            var ttv = element.TransformToVisual(relativeTo);
            var location = ttv.TransformPoint(new Point(0, 0));
            
            if (GetActualWidthMethod?.Invoke(element, null) is not double width)
                width = element.DesiredSize.Width;
            
            if (GetActualHeightMethod?.Invoke(element, null) is not double height)
                height = element.DesiredSize.Height;
            
            return new Rect(location, new Size(width, height));
        }

        /// <summary>
        /// Get Bounds of UIElement relative to another UIElement
        /// </summary>
        /// <param name="relativeToElement"></param>
        /// <returns></returns>
        [Obsolete("Use GetBounds() instead.")]
        public Rect GetBoundsRelativeTo(UIElement relativeToElement)
        {
            var ttv = element.TransformToVisual(relativeToElement);
            var location = ttv.TransformPoint(new Point(0, 0));
            //return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
            if (GetActualWidthMethod?.Invoke(element, null) is not double width)
                width = element.DesiredSize.Width;
            
            if (GetActualHeightMethod?.Invoke(element, null) is not double height)
                height = element.DesiredSize.Height;
            
            return new Rect(location, new Size(width, height));
        }

    }

    /// <param name="element"></param>
    extension(UIElement element)
    {
        /// <summary>
        /// Find first ancestor of type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T? FindAncestor<T>() where T : UIElement
        {
            var parent = VisualTreeHelper.GetParent(element); // as FrameworkElement;
            while (parent != null)
            {
                if (parent is T uiElement)
                    return uiElement;
            
                parent = VisualTreeHelper.GetParent(parent);
            }
        
            return null;
        }

        /// <summary>
        /// Try to find first ancestor of type T
        /// </summary>
        /// <param name="ancestor"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool TryFindAncestor<T>(out T? ancestor) where T : UIElement
        {
            ancestor = element.FindAncestor<T>();
            return ancestor != null;
        }
    }

    /// <param name="templateType"></param>
    extension(Type templateType)
    {
        /// <summary>
        /// Convert a type into a DataTemplate Xaml string
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string AsDataTemplateXaml()
        {
            if (templateType == null || !typeof(FrameworkElement).IsAssignableFrom(templateType))
                throw new Exception($"Cannot convert type [{templateType}] into DataTemplate");

            var markup = $"<DataTemplate \n\t xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:local=\"using:{templateType.Namespace}\"> \n\t\t<local:{templateType.Name} /> \n</DataTemplate>";
            //if (dataType.Namespace == typeof(Type).Namespace)
            //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:tlocal=\"using:{templateType.Namespace}\" xmlns:system=\"using:System\" x:DataType=\"system:{dataType.Name}\"><tlocal:{templateType.Name} /></DataTemplate>";
            //else
            //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:tlocal=\"using:{templateType.Namespace}\" \n\t xmlns:dlocal=\"using:{dataType.Namespace}\" \n\t x:DataType=\"dlocal:{dataType.Name}\"> \n\t\t<tlocal:{templateType.Name} /> \n</DataTemplate>";
            // System.Diagnostics.Debug.WriteLine("BcGroupView.GenerateDataTemplate: markup: " + markup);
            //template.
            return markup;
        }

        /// <summary>
        /// Convert type to a DataTemplate
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public DataTemplate? AsDataTemplate([CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                var markup = templateType.AsDataTemplateXaml();
                return (DataTemplate)XamlReader.Load(markup);
            }
            catch (Exception e)
            {
                QLog.Error(e, $"{filePath}:{lineNumber}");
            }
            return null;
        }
    }

    /// <summary>
    /// Gets the first descendent of a FrameworkElement
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static UIElement? GetFirstDescendent(this FrameworkElement element)
        => element.FindChildren<UIElement>().FirstOrDefault();

    /// <param name="parent"></param>
    extension(DependencyObject parent)
    {
        /// <summary>
        /// Find's child of a FrameworkElement by name
        /// </summary>
        /// <param name="controlName"></param>
        /// <returns></returns>
        public DependencyObject? FindChildByName(string controlName)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement element && element.Name == controlName)
                    return element;

                var findResult = FindChildByName(child, controlName);
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
