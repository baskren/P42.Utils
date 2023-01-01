using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI.Core;
using Windows.Foundation;

namespace P42.Utils.Uno
{
    public static class ListViewExtensions
    {
		public static ScrollViewer GetScrollViewer(this DependencyObject depObj)
		{
            if (depObj is ScrollViewer obj) return obj;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
			{
				var child = VisualTreeHelper.GetChild(depObj, i);

				var result = GetScrollViewer(child);
				if (result != null) return result;
			}
			return null;
		}

		public static Point GetOffsetForItem(this ListView listView, object item)
		{
			if (listView.ContainerFromItem(item) is UIElement element)
			{
				if (GetScrollViewer(listView) is ScrollViewer viewer)
				{
					var transform = element.TransformToVisual(viewer);
					var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));
					return positionInScrollViewer;
				}
			}
			return new Point(0, 0);
		}

		public static async Task ScrollToBottom(this ListView listView, object item)
		{
			if (GetScrollViewer(listView) is ScrollViewer viewer)
			{
				var container = listView.ContainerFromItem(item);
				if (container is FrameworkElement element)
				{
					var transform = element.TransformToVisual(viewer);
					var positionInScrollViewer = transform.TransformPoint(new Point(0, 0));
					System.Diagnostics.Debug.WriteLine(" ==============================================================");
					System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom vtOffset: " + viewer.VerticalOffset);
					System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom position: " + positionInScrollViewer.Y);
					System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom element.H: " + element.ActualHeight);
					System.Diagnostics.Debug.WriteLine("ListViewExtensions.ScrollToBottom listView.H: " + listView.ActualHeight);


//#if NET7_0_WINDOWS10_0_19041_0
//                    var offset = Math.Max(0, viewer.VerticalOffset + positionInScrollViewer.Y + element.ActualHeight - listView.ActualHeight );
//                    viewer.ScrollToVerticalOffset(offset);
//#else
					var offset = Math.Max(0, positionInScrollViewer.Y + element.ActualHeight - listView.ActualHeight);
					viewer.ChangeView(null, offset, null);
					//#endif
					await Task.Delay(1000);
				}
			}
		}

		
		public async static Task ScrollToAsync(this ListView list, object item, ScrollToPosition toPosition, bool shouldAnimate = true)
		{
#if __WASM__
			if (list.ContainerFromItem(item) is Microsoft.UI.Xaml.Controls.Primitives.SelectorItem selectorItem)
            {
                var id = selectorItem.GetHtmlAttribute("id");
                System.Diagnostics.Debug.WriteLine("BcGroupView.Edit html.id = " + id);
				global::Uno.Foundation.WebAssemblyRuntime.InvokeJS("document.getElementById('"+id+"').scrollIntoView(" +(toPosition != ScrollToPosition.End).ToString().ToLower()+ ");");
            }

#else
			await InternalScrollToAsync(list, item, toPosition, shouldAnimate, false);
#endif
			await Task.Delay(5);
		}
		

#if !__WASM__
		static bool InternalScrollToItemWithAnimation(ListView list, object item, ScrollToPosition toPosition)
		{
			if (GetScrollViewer(list) is ScrollViewer viewer)
			{
				var selectorItem = list.ContainerFromItem(item) as Microsoft.UI.Xaml.Controls.Primitives.SelectorItem;
				var transform = selectorItem?.TransformToVisual(viewer.Content as UIElement);
				var position = transform?.TransformPoint(new Windows.Foundation.Point(0, 0));
				if (!position.HasValue)
					return false;
				// scroll with animation
				var containerHeight = selectorItem.DesiredSize.Height;
				var viewportHeight = viewer.ViewportHeight;

				var offset = 0.0;
				if (toPosition == ScrollToPosition.Center)
					offset = (viewportHeight - containerHeight) / 2.0;
				else if (toPosition == ScrollToPosition.End)
					offset = viewportHeight - containerHeight;
				viewer.ChangeView(position.Value.X, position.Value.Y - offset, null);
				return true;
			}
			return false;
		}


		async static Task InternalScrollToAsync(this ListView list, object item, ScrollToPosition toPosition, bool shouldAnimate, bool previouslyFailed)
		{
			if (GetScrollViewer(list) is ScrollViewer viewer)
			{ 
				// scroll to desired item with animation
				if (shouldAnimate && InternalScrollToItemWithAnimation(list, item, toPosition))
					return;

				var viewportHeight = viewer.ViewportHeight;
                System.Diagnostics.Debug.WriteLine("ListViewExtensions.InternalScrollToAsync viewportHeight: " + viewportHeight);

				var semanticLocation = new SemanticZoomLocation { Item = item };

				// async scrolling
				await list.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
				{
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

									if (toPosition == ScrollToPosition.Center)
										semanticLocation.Bounds = new Rect(0, viewportHeight / 2 - tHeight / 2, 0, 0);
									else
										semanticLocation.Bounds = new Rect(0, viewportHeight - tHeight, 0, 0);
								}
								break;
							}

						default:
							break;
					}
				});

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

					Task.Delay(1).ContinueWith(async ct => { await InternalScrollToAsync(list, item, toPosition, shouldAnimate, true); }, TaskScheduler.FromCurrentSynchronizationContext()).WatchForError();
					//await Task.Delay(5);
					//await InternalScrollToAsync(list, item, toPosition, shouldAnimate, true); 
				}

			}
			else
			{
                async void loadedHandler(object o, RoutedEventArgs e)
                {
                    list.Loaded -= loadedHandler;
                    // Here we try to avoid an exception, see explanation at bottom
                    await list.Dispatcher.RunIdleAsync(async args => { await InternalScrollToAsync(list, item, toPosition, shouldAnimate, false); });
                }

                list.Loaded += loadedHandler;
				return;
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
}
