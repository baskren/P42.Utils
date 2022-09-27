using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace P42.Utils.Uno
{
    /// <summary>
    /// NO LONGER PUBLICALLY VISIBLE - USE XAMARIN.ESSENTIALS.MAINTHREAD instead
    /// </summary>
    class MainThread
    {
        public static bool IsMainThread
        {
            get
            {
                // if there is no main window, then this is either a service
                // or the UI is not yet constructed, so the main thread is the
                // current thread
                try
                {
                    if (CoreApplication.MainView?.CoreWindow == null)
                        return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to validate MainView creation. {ex.Message}");
                    return true;
                }

                return CoreApplication.MainView.CoreWindow.Dispatcher?.HasThreadAccess ?? false;
            }
        }

        public static void BeginInvokeOnMainThread(Action action)
        {
            if (action is null)
                return;

            if (IsMainThread)
            {
                action.Invoke();
                return;
            }

            var dispatcher = CoreApplication.MainView?.CoreWindow?.Dispatcher;
            if (dispatcher == null)
                throw new InvalidOperationException("Unable to find main thread.");

            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action.Invoke()).WatchForError();
        }



        public static T BeginInvokeOnMainThread<T>(Func<T> action)
        {
            if (action is null)
                return default(T);

            if (P42.Utils.Uno.MainThread.IsMainThread)
                return action.Invoke();

            var task = Task.Run(async () => await BeginInvokeOnMainThreadAsync(() => action.Invoke()));
            task.Wait();
            return task.Result;
        }

        public static async Task<T> BeginInvokeOnMainThreadAsync<T>(Func<T> action)
        {
            if (action is null)
                return default(T);

            if (P42.Utils.Uno.MainThread.IsMainThread)
                return action.Invoke();

            var coreDispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView?.CoreWindow?.Dispatcher;
            if (coreDispatcher == null)
            {
                throw new InvalidOperationException("Unable to find main thread.");
            }

            var tcs = new TaskCompletionSource<T>();
            coreDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                var result = action.Invoke();
                tcs.SetResult(result);
            }).WatchForError();

            await tcs.Task;

            return tcs.Task.Result;
        }

    }
}
