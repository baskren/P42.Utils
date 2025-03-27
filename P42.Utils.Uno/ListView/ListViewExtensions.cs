using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;

namespace P42.Utils.Uno;

public static class ListViewExtensions
{
    
    /// <summary>
    /// Work-around for when ListView is nested in a ScrollViewer and, as a result, PointerWheelChanged events are not being fired.
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="handler"></param>
    /// <typeparam name="TElement"></typeparam>
    /// <returns>targeted ListView</returns>
    public static TElement AddPointerWheelChangedIntercept<TElement>(this TElement listView, PointerEventHandler handler) where TElement : ListView
    {
        // In latest versions of Windows, Scrolling of a ListView, nested in a ScrollViewer, doesn't work
        // and said ListView's ScrollViewer won't fire PointerWheelChanged events.  The below allows you to 
        // get the event before it gets to the ListView's ScrollViewer.  These can be overriden (e.Handled = true) 
        // and then used for other things (like scrolling the outer ScrollView)


        if (listView.GetScrollViewer() is not { } scrollViewer)
        {
            listView.Loaded += (s, _) =>
            {
                if (s is ListView lv)
                    lv.AddPointerWheelChangedIntercept(handler);
            };
            return listView;
        }

        if (VisualTreeHelper.GetChild(scrollViewer, 0) is Border border)
            border.PointerWheelChanged += handler;

        return listView;
    }


    /// <summary>
    /// Get ScrollViewer within DependencyObject (typically a ListView)
    /// </summary>
    /// <param name="depObj"></param>
    /// <returns></returns>
    public static ScrollViewer? GetScrollViewer(this DependencyObject depObj)
    {
        if (depObj is ScrollViewer obj) return obj;

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
            if (VisualTreeHelper.GetChild(depObj, i) is not { } child)
                continue;

            if (GetScrollViewer(child) is { } result)
                return result;
            
        }
        return null;
    }

    /// <summary>
    /// Gets the Offset of an item within a ListView
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static Point GetOffsetForItem(this ListView listView, object item)
    {
        if (listView.ContainerFromItem(item) is not UIElement element)
            throw new Exception("Item is not visible in ListView");

        if (GetScrollViewer(listView) is not { } viewer)
            throw new Exception("ScrollViewer is not found in ListView");

        var transform = element.TransformToVisual(viewer);
        var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));
        return positionInScrollViewer;
    }

    /// <summary>
    /// Tries to get offset for item
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="item"></param>
    /// <param name="offset"></param>
    /// <returns>false when cannot get offset</returns>
    public static bool TryGetOffsetOfItem(this ListView listView, object item, out Point offset)
    {
        try
        {
            offset = GetOffsetForItem(listView, item);
            return true;
        }
        catch (Exception)
        {
            offset = default;
            return false;
        }
    }

    /// <summary>
    /// Scrolls ListView to item to bottom of current ListView's bounds
    /// </summary>
    /// <param name="listView"></param>
    /// <param name="item"></param>
    public static async Task ScrollToBottom(this ListView listView, object item)
    {
        if (GetScrollViewer(listView) is { } viewer)
        {
            var container = listView.ContainerFromItem(item);
            if (container is FrameworkElement element)
            {
                var transform = element.TransformToVisual(viewer);
                var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));
                var offset = Math.Max(0, positionInScrollViewer.Y + element.ActualHeight - listView.ActualHeight);
                viewer.ChangeView(null, offset, null);
                await Task.Delay(1000);
            }
        }
    }

    /// <summary>
    /// Scrolls ListView to item
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <param name="toPosition"></param>
    /// <param name="shouldAnimate"></param>
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
        await Task.Delay(500);
    }
		

#if !__WASM__
    private static bool TryInternalScrollToItemWithAnimation(ListView list, object item, ScrollToPosition toPosition)
	{
        if (GetScrollViewer(list) is not { } viewer)
            return false;

        if (list.ContainerFromItem(item) is not SelectorItem selectorItem)
            return false;
        
        var transform = selectorItem.TransformToVisual(viewer.Content as UIElement);
        var position = transform.TransformPoint(new Point(0, 0));

        // scroll with animation
        var containerHeight = selectorItem.DesiredSize.Height;
        var viewportHeight = viewer.ViewportHeight;

        var offset = 0.0;
        if (toPosition == ScrollToPosition.Center)
            offset = (viewportHeight - containerHeight) / 2.0;
        else if (toPosition == ScrollToPosition.End)
            offset = viewportHeight - containerHeight;
        viewer.ChangeView(position.X, position.Y - offset, null);
        return true;
    }


    private static async Task InternalScrollToAsync(this ListView list, object item, ScrollToPosition toPosition, bool shouldAnimate, bool previouslyFailed)
	{
		if (GetScrollViewer(list) is { } viewer)
		{ 
			// scroll to desired item with animation
			if (shouldAnimate && TryInternalScrollToItemWithAnimation(list, item, toPosition))
				return;

			var viewportHeight = viewer.ViewportHeight;

			var semanticLocation = new SemanticZoomLocation { Item = item };

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
						if (list.ContainerFromItem(item) is FrameworkElement element)
                        {
                            var tHeight = element.DesiredSize.Height;
                            semanticLocation.Bounds = toPosition == ScrollToPosition.Center 
                                ? new Rect(0, viewportHeight / 2 - tHeight / 2, 0, 0) 
                                : new Rect(0, viewportHeight - tHeight, 0, 0);
                        }
						break;
					}

				default:
					throw new ArgumentOutOfRangeException(nameof(toPosition), toPosition, null);
			}

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
				Task.Delay(1).ContinueWith(async _ => 
                    { await InternalScrollToAsync(list, item, toPosition, shouldAnimate, true); }
                    , TaskScheduler.FromCurrentSynchronizationContext()).WatchForError();
			}

		}
		else
		{
            async void LoadedHandler(object o, RoutedEventArgs e)
            {
                list.Loaded -= LoadedHandler;
                // Here we try to avoid an exception, see explanation at bottom
                await list.Dispatcher.RunIdleAsync(async _ => { await InternalScrollToAsync(list, item, toPosition, shouldAnimate, false); });
            }

            list.Loaded += LoadedHandler;
		}

	}

    
#endif
    
    
}
