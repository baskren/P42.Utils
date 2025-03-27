using System;
using System.Threading.Tasks;
using P42.Utils.Uno;

namespace P42.Utils.Uno;

/// <summary>
/// Run on background thread, passing exceptions back to calling waiting thread
/// </summary>
public static class BackgroundThread
{
    /// <summary>
    /// Runs action on BackgroundThread
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static Task InvokeAsync(Action action)
    {
        if (!MainThread.IsMainThread)
        {
            action();
            return Task.CompletedTask;
        }
        
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        Task.Run(delegate {
            try
            {
                action();
                tcs.TrySetResult(result: true);
            }
            catch (Exception exception)
            {
                tcs.TrySetException(exception);
            }
        });
        return tcs.Task;
    }

    /// <summary>
    /// Runs function on BackgroundThread
    /// </summary>
    /// <param name="func"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<T> InvokeAsync<T>(Func<T> func)
    {
        if (!MainThread.IsMainThread)
            return Task.FromResult(func());

        TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
        Task.Run(delegate {
            try
            {
                T result = func();
                tcs.TrySetResult(result);
            }
            catch (Exception exception)
            {
                tcs.TrySetException(exception);
            }
        });
        return tcs.Task;
    }

    /// <summary>
    /// Runs Task function (async lambda) on background thread
    /// </summary>
    /// <param name="funcTask"></param>
    /// <returns></returns>
    public static Task InvokeAsync(Func<Task> funcTask)
    {
        if (!MainThread.IsMainThread)
            return funcTask();

        var tcs = new TaskCompletionSource<bool>();
        Task.Run(async delegate {
            try
            {
                await funcTask().ConfigureAwait(continueOnCapturedContext: false);
                tcs.SetResult(true);
            }
            catch (Exception exception)
            {
                tcs.SetException(exception);
            }
        });
        return tcs.Task;
    }

    /// <summary>
    /// Runs Task function (async lambda) on background thread
    /// </summary>
    /// <param name="funcTask"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Task<T> InvokeAsync<T> (Func<Task<T>> funcTask)
    {
        if (!MainThread.IsMainThread)
            return funcTask();

        var tcs = new TaskCompletionSource<T>();
        Task.Run(async delegate {
            try
            {
                var result = await funcTask().ConfigureAwait(continueOnCapturedContext: false);
                tcs.SetResult(result);
            }
            catch (Exception exception)
            {
                tcs.SetException(exception);
            }
        });
        return tcs.Task;
    }


    
}
