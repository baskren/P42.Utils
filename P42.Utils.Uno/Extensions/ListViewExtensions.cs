using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Microsoft.UI.Xaml.Input;
#if !__WASM__
using Windows.UI.Core;
#endif

namespace P42.Utils.Uno;

/// <summary>
/// Extensions for Microsoft.UI.Xaml.Controls.ListView
/// </summary>
public static class ListViewExtensions
{
    /// <summary>
    /// Calls handler before scroll event is processed by ListView.  When handler is called, set e.Handled = true to prevent ListView from scrolling
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="handler"></param>
    /// <typeparam name="TElement">ListView</typeparam>
    /// <returns>ListView</returns>
    public static TElement AddPointerWheelChangedIntercept<TElement>(this TElement listView, PointerEventHandler handler) where TElement : ListView
    {
        // In latest versions of Windows, Scrolling of a ListView, nested in a ScrollViewer, doesn't work
        // and said ListView's ScrollViewer won't fire PointerWheelChanged events.  The below allows you to 
        // get the event before it gets to the ListView's ScrollViewer.  These can be overriden (e.Handled = true) 
        // and then used for other things (like scrolling the outer ScrollView)
        if (listView.GetScrollViewer() is not { } scrollViewer)
        {
            var onLoadHandler = new AddPointerWheelChangedInterceptOnLoad(handler);
            listView.Loaded += onLoadHandler.OnLoad;
            return listView;
        }

        if (VisualTreeHelper.GetChild(scrollViewer, 0) is Border border)
            border.PointerWheelChanged += handler;

        return listView;
    }

    private class AddPointerWheelChangedInterceptOnLoad(PointerEventHandler handler)
    {
        public void OnLoad(object s, RoutedEventArgs args)
        {
            if (s is not ListView listView)
                return;
            listView.AddPointerWheelChangedIntercept(handler);
            listView.Loaded -= OnLoad;
        }
    }


    /// <summary>
    /// Get ListView's ScrollViewer
    /// </summary>
    /// <param name="depObj"></param>
    /// <returns>null if not found</returns>
    public static ScrollViewer? GetScrollViewer(this DependencyObject depObj)
    {
        if (depObj is ScrollViewer obj) return obj;

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            var result = GetScrollViewer(child);
            if (result != null) return result;
        }
        
        return null;
    }

    /// <summary>
    /// Returns offset between ListView and cell containing sourceItem 
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="sourceItem"></param>
    /// <returns>Point(0,0) on failure</returns>
    public static Point GetOffsetForItem(this ListView listView, object sourceItem)
    {
        if (GetScrollViewer(listView) is not { } viewer)
            return new Point(0, 0);

        if (listView.ContainerFromItem(sourceItem) is not UIElement element)
            return new Point(0, 0);

        var transform = element.TransformToVisual(viewer);
        var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));
        return positionInScrollViewer;
    }

    /// <summary>
    /// Scroll ListView so cell for sourceItem is at the bottom of ListView's frame
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="sourceItem"></param>
    public static async Task ScrollToBottom(this ListView listView, object sourceItem)
    {
        if (GetScrollViewer(listView) is not { } viewer)
            return;
        
        if (listView.ContainerFromItem(sourceItem) is not FrameworkElement element)
            return;
        
        
        var transform = element.TransformToVisual(viewer);
        var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));
        //System.Diagnostics.Debug.WriteLine(" ==============================================================");
        //System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom vtOffset: " + viewer.VerticalOffset);
        //System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom position: " + positionInScrollViewer.Y);
        //System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom element.H: " + element.ActualHeight);
        //System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom listView.H: " + listView.ActualHeight);


//#if !HAS_UNO
//                    var offset = Math.Max(0, viewer.VerticalOffset + positionInScrollViewer.Y + element.ActualHeight - listView.ActualHeight );
//                    viewer.ScrollToVerticalOffset(offset);
//#else
        var offset = Math.Max(0, positionInScrollViewer.Y + element.ActualHeight - listView.ActualHeight);
        viewer.ChangeView(null, offset, null);
        //#endif
        await Task.Delay(1000);
    }

		
    public static async Task ScrollToAsync(this ListView list, object item, ScrollToPosition toPosition, bool shouldAnimate = true)
    {
#if __WASM__
		if (list.ContainerFromItem(item) is Microsoft.UI.Xaml.Controls.Primitives.SelectorItem selectorItem)
        {
            var id = selectorItem.GetHtmlAttribute("id");
            //System.Diagnostics.Debug.WriteLine("BcGroupView.Edit html.id = " + id);
			global::Uno.Foundation.WebAssemblyRuntime.InvokeJS(
                $"document.getElementById('{id}').scrollIntoView({(toPosition != ScrollToPosition.End).ToString().ToLower()});");
        }

#else
        await InternalScrollToAsync(list, item, toPosition, shouldAnimate, false);
#endif
        await Task.Delay(5);
    }
		

#if !__WASM__
    private static bool InternalScrollToItemWithAnimation(ListView list, object item, ScrollToPosition toPosition)
    {
        if (GetScrollViewer(list) is not { } viewer)
            return false;

        //var selectorItem = list.ContainerFromItem(item) as Microsoft.UI.Xaml.Controls.Primitives.SelectorItem;
        if (list.ContainerFromItem(item) is not Microsoft.UI.Xaml.Controls.Primitives.SelectorItem selectorItem)
            return false;
        
        var transform = selectorItem.TransformToVisual(viewer.Content as UIElement);
        var position = transform.TransformPoint(new Point(0, 0));
        // scroll with animation
        var containerHeight = selectorItem.DesiredSize.Height;
        var viewportHeight = viewer.ViewportHeight;

        var offset = toPosition switch
        {
            ScrollToPosition.Center => (viewportHeight - containerHeight) / 2.0,
            ScrollToPosition.End => viewportHeight - containerHeight,
            _ => 0.0
        };
        viewer.ChangeView(position.X, position.Y - offset, null);
        return true;
    }


    private static async Task InternalScrollToAsync(this ListView list, object item, ScrollToPosition toPosition, bool shouldAnimate, bool previouslyFailed)
    {
        if (GetScrollViewer(list) is { } viewer)
        { 
            // scroll to desired item with animation
            if (shouldAnimate && InternalScrollToItemWithAnimation(list, item, toPosition))
                return;

            var viewportHeight = viewer.ViewportHeight;
            //System.Diagnostics.Debug.WriteLine("ListViewExtensions.InternalScrollToAsync viewportHeight: " + viewportHeight);

            var semanticLocation = new SemanticZoomLocation { Item = item };

            // async scrolling
            //await list.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            //{
            switch (toPosition)
            {
                case ScrollToPosition.Start:
                {

                    list.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);
                    return;
                }

                case ScrollToPosition.MakeVisible:
                {
                    list.ScrollIntoView(item, ScrollIntoViewAlignment.Default);
                    return;
                }

                case ScrollToPosition.End:
                case ScrollToPosition.Center:
                {
                    /*
                    var content = (FrameworkElement)list.ItemTemplate.LoadContent();
                    content.DataContext = item;
                    content.Measure(new Windows.Foundation.Size(viewer.ActualWidth, double.PositiveInfinity));
                    */
                    if (list.ContainerFromItem(item) is FrameworkElement element)
                    {
                        var tHeight = element.DesiredSize.Height;

                        semanticLocation.Bounds = toPosition == ScrollToPosition.Center 
                            ? new Rect(0, viewportHeight / 2 - tHeight / 2, 0, 0) 
                            : new Rect(0, viewportHeight - tHeight, 0, 0);
                    }
                    break;
                }
            }
            //});

            // Waiting for loaded doesn't seem to be enough anymore; the ScrollViewer does not appear until after Loaded.
            // Even if the ScrollViewer is present, an invoke at low priority fails (E_FAIL) presumably because the items are
            // still loading. An invoke at idle sometimes work, but isn't reliable enough, so we'll just have to commit
            // treason and use a blanket catch for the E_FAIL and try again.
            try
            {
                list.MakeVisible(semanticLocation);
            }
            catch (Exception)
            {
                if (previouslyFailed)
                    return;

                await Task.Delay(10);
                Task.Delay(1).ContinueWith(async _ => { await InternalScrollToAsync(list, item, toPosition, shouldAnimate, true); }, TaskScheduler.FromCurrentSynchronizationContext()).WatchForError();
                //await Task.Delay(5);
                //await InternalScrollToAsync(list, item, toPosition, shouldAnimate, true); 
            }

        }
        else
        {
            list.Loaded += LoadedHandler;
            return;
            
            async void LoadedHandler(object o, RoutedEventArgs e)
            {
                list.Loaded -= LoadedHandler;
                // Here we try to avoid an exception, see explanation at bottom
                await list.Dispatcher.RunIdleAsync(AgileCallback);
                return;

                async void AgileCallback(IdleDispatchedHandlerArgs _)
                {
                    await InternalScrollToAsync(list, item, toPosition, shouldAnimate, false);
                }

            }

        }

    }

    /*
    public static ScrollViewer GetScrollViewer(DependencyObject depObj)
    {
        var obj = depObj as ScrollViewer;
        if (obj != null) return obj;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            var child = VisualTreeHelper.GetChild(depObj, i);

            var result = GetScrollViewer(child);
            if (result != null) return result;
        }
        return null;
    }
    */
#endif
}
