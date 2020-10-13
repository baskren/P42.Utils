using P42.Utils.Uno.Extensions;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace P42.Utils.Uno
{
    public class Device
    {
        class MainThreadTimer
        {
            readonly DispatcherTimer dispatcherTimer;
            readonly Func<bool> Func;

            public MainThreadTimer(TimeSpan span, Func<bool> func)
            {
                Func = func;
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Interval = span;
                dispatcherTimer.Start();
            }

            void DispatcherTimer_Tick(object sender, object e)
            {
                if (!Func.Invoke())
                {
                    dispatcherTimer.Stop();
                }
            }
        }

        class AsyncMainThreadTimer
        {
            readonly DispatcherTimer dispatcherTimer;
            readonly Func<Task<bool>> task;

            public AsyncMainThreadTimer(TimeSpan span, Func<Task<bool>> task)
            {
                this.task = task;
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Tick += DispatcherTimer_Tick;
                dispatcherTimer.Interval = span;
                dispatcherTimer.Start();
            }

            async void DispatcherTimer_Tick(object sender, object e)
            {
                if (!await task.Invoke())
                {
                    dispatcherTimer.Stop();
                }
            }
        }

        public static void StartTimer(TimeSpan timeSpan, Func<bool> func)
            => new MainThreadTimer(timeSpan, func);

        public static void StartTimer(TimeSpan timeSpan, Func<Task<bool>> task)
            => new AsyncMainThreadTimer(timeSpan, task);


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
            var dispatcher = CoreApplication.MainView?.CoreWindow?.Dispatcher;

            if (dispatcher == null)
                throw new InvalidOperationException("Unable to find main thread.");
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).WatchForError();
        }


        public static DeviceIdiom Idiom
        {
            get
            {
#if __WASM__ || NETSTANDARD
                return DeviceIdiom.Web;
#else
                return Xamarin.Essentials.DeviceInfo.Idiom.ToUno();
#endif
            }
        }
    }
}
