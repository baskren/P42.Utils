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
        public Rect GetBounds(UIElement? relativeTo = null)
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




}
