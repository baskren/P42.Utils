using System;
using System.Threading.Tasks;
using UIKit;

namespace P42.Utils.iOS
{
    public class LongRunningTask : ILongRunningTask
    {

        nint taskId = -1;
        //TaskCompletionSource<bool> tcs;

        public async Task<T> RunAsync<T>(Func<T> function, Func<T> expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<T>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
            {
                expirationHandler?.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(default);
            });
            InnerRun(function, tcs);
            var result = await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
            return result;
        }

        public async Task<T> RunAsync<T>(Func<Task<T>> function, Func<T> expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<T>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(() =>
            {
                expirationHandler?.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(default);
            });
            InnerRun(function, tcs);
            var result = await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
            return result;
        }

        public async Task<T> RunAsync<T>(Func<T> function, Func<Task<T>> expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<T>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(async () =>
            {
                T r = default;
                if (expirationHandler != null)
                    r = await expirationHandler.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(r);
            });
            InnerRun(function, tcs);
            var result = await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
            return result;
        }

        public async Task<T> RunAsync<T>(Func<Task<T>> function, Func<Task<T>> expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<T>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(async () =>
            {
                T r = default;
                if (expirationHandler != null)
                    r = await expirationHandler.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(r);
            });
            InnerRun(function, tcs);
            var result = await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
            return result;
        }


        public async Task RunAsync(Action action, Action expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask( () =>
            {
                expirationHandler?.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(false);
            });
            InnerRun(action, tcs);
            await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }


        public async Task RunAsync(Func<Task> action, Action expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask( () =>
            {
                expirationHandler?.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(false);
            });
            InnerRun(action, tcs);
            await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }


        public async Task RunAsync(Action action, Func<Task> expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(async () =>
            {
                if (expirationHandler != null)
                    await expirationHandler.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(false);
            });
            InnerRun(action, tcs);
            await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }


        public async Task RunAsync(Func<Task> action, Func<Task> expirationHandler = null)
        {
            var tcs = new TaskCompletionSource<bool>();
            taskId = UIApplication.SharedApplication.BeginBackgroundTask(async () =>
            {
                if (expirationHandler != null)
                    await expirationHandler.Invoke();
                UIApplication.SharedApplication.EndBackgroundTask(taskId);
                tcs.SetResult(false);
            });
            InnerRun(action, tcs);
            await tcs.Task;
            UIApplication.SharedApplication.EndBackgroundTask(taskId);
        }

        void InnerRun<T>(Func<T> func, TaskCompletionSource<T> tcs)
        {
            if (func != null)
            {
                T result = func.Invoke();
                tcs.SetResult(result);
                return;
            }
            tcs.SetResult(default);
        }

        void InnerRun<T>(Func<Task<T>> func, TaskCompletionSource<T> tcs)
        {
            if (func != null)
            {
                Task.Run(async () =>
                {
                    T result = await func.Invoke();
                    tcs.SetResult(result);
                });
                return;
            }
            tcs.SetResult(default);
        }


        void InnerRun(Func<Task> action, TaskCompletionSource<bool> tcs)
        {
            action?.Invoke().RunSynchronously();
            tcs.SetResult(true);
        }

        void InnerRun(Action action, TaskCompletionSource<bool> tcs)
        {
            action?.Invoke();
            tcs.SetResult(true);
        }
    }
}
