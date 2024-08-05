using System;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils.Uno
{
    static partial class MainThread
    {
        public static bool IsMainThread
        {
            get
            {
                if (Platform.MainThreadDispatchQueue == null)
                    throw new Exception("Unable to find main thread.  Did you call Xamarin.Essentials.Platform.Init() ?");

                return Platform.MainThread == Thread.CurrentThread;
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

            if (!Platform.MainThreadDispatchQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => action.Invoke()))
                throw new InvalidOperationException("Unable to queue on the main thread.");
        }

        public static void InvokeOnMainThread(Action action)
        {
            if (action is null)
                return;

            if (IsMainThread)
            {
                action();
                return;
            }

            if (!Platform.MainThreadDispatchQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => action.Invoke()))
                throw new InvalidOperationException("Unable to queue on the main thread.");
        }

        public static Task InvokeOnMainThreadAsync(Action action)
        {
            if (action is null)
                return Task.CompletedTask;

            if (IsMainThread)
            {
                action();
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<bool>();
            if (!Platform.MainThreadDispatchQueue.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        try
                        {
                            action();
                            tcs.TrySetResult(true);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    }))
                throw new InvalidOperationException("Unable to queue on the main thread.");

            return tcs.Task;
        }

        public static T InvokeOnMainThread<T>(Func<T> func)
        {
            if (func is null)
                return default(T);

            if (IsMainThread)
                return func();

            var task = Task.Run(async () => await InvokeOnMainThreadAsync(func));
            var result = task.Result;
            return result;
        }

        public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
        {
            if (func is null)
                return Task.FromResult(default(T));

            if (IsMainThread)
                return Task.FromResult(func());

            var tcs = new TaskCompletionSource<T>();
            if (!Platform.MainThreadDispatchQueue.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () =>
                    {
                        try
                        {
                            var result = func();
                            tcs.TrySetResult(result);
                        }
                        catch (Exception ex)
                        {
                            tcs.TrySetException(ex);
                        }
                    }))
                throw new InvalidOperationException("Unable to queue on the main thread.");

            return tcs.Task;
        }

        public static Task InvokeOnMainThreadAsync(Func<Task> funcTask)
        {
            if (funcTask is null)
                return Task.CompletedTask;

            if (IsMainThread)
                return funcTask();

            var tcs = new TaskCompletionSource<object>();
            if (!Platform.MainThreadDispatchQueue.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                    {
                        try
                        {
                            await funcTask().ConfigureAwait(false);
                            tcs.SetResult(null);
                        }
                        catch (Exception e)
                        {
                            tcs.SetException(e);
                        }
                    }))
                throw new InvalidOperationException("Unable to queue on the main thread.");

            return tcs.Task;
        }

        public static T InvokeOnMainThread<T>(Func<Task<T>> funcTask)
        {
            if (funcTask is null)
                return default;

            var task = Task.Run(async () => await InvokeOnMainThreadAsync(funcTask));
            var result = task.Result;
            return result;
        }

        public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
        {
            if (funcTask is null)
                return Task.FromResult(default(T));

            if (IsMainThread)
                return funcTask();

            var tcs = new TaskCompletionSource<T>();
            if (!Platform.MainThreadDispatchQueue.TryEnqueue(
                Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async () =>
                    {
                        try
                        {
                            var ret = await funcTask().ConfigureAwait(false);
                            tcs.SetResult(ret);
                        }
                        catch (Exception e)
                        {
                            tcs.SetException(e);
                        }
                    }))
                throw new InvalidOperationException("Unable to queue on the main thread.");

            return tcs.Task;
        }
    }
}
