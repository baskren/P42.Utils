using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace P42.Utils.Uno;

/// <summary>
/// Perform actions/functions on MainThread
/// </summary>
public static class MainThread
{
    /// <summary>
    /// Are we on the app's main thread?
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static bool IsMainThread
    {
        get
        {
            if (Platform.MainThreadDispatchQueue == null)
                throw new Exception("Unable to find main thread.  Did you call Xamarin.Essentials.Platform.Init() ?");

            return Platform.MainThread == Thread.CurrentThread;
        }
    }

    /// <summary>
    /// Invoke action on main thread
    /// </summary>
    /// <param name="action"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void InvokeOnMainThread(Action action)
    {
        if (Platform.MainThreadDispatchQueue is null)
            throw new InvalidOperationException("MainThreadDispatchQueue not set.  Did you call P42.Utils.Uno.Platform.Init()?");
        
        if (IsMainThread)
        {
            action.Invoke();
            return;
        }

        if (!Platform.MainThreadDispatchQueue.TryEnqueue(DispatcherQueuePriority.Normal, action.Invoke))
            throw new InvalidOperationException("Unable to queue on the main thread.");
    }

    /// <summary>
    /// Asynchronously invoke action on main thread
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task InvokeOnMainThreadAsync(Action action)
    {
        if (Platform.MainThreadDispatchQueue is null)
            throw new InvalidOperationException("MainThreadDispatchQueue not set.  Did you call P42.Utils.Uno.Platform.Init()?");
        
        if (IsMainThread)
        {
            action();
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource<bool>();
        if (!Platform.MainThreadDispatchQueue.TryEnqueue(DispatcherQueuePriority.Normal, Callback))
            throw new InvalidOperationException("Unable to queue on the main thread.");

        return tcs.Task;

        void Callback()
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
        }
    }

    /// <summary>
    /// Invoke function on main thread
    /// </summary>
    /// <param name="func"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T InvokeOnMainThread<T>(Func<T> func)
    {
        if (IsMainThread)
            return func();

        var task = Task.Run(async () => await InvokeOnMainThreadAsync(func));
        var result = task.Result;
        return result;
    }

    /// <summary>
    /// Asynchronously invoke function on main thread
    /// </summary>
    /// <param name="func"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
    {
        if (Platform.MainThreadDispatchQueue is null)
            throw new InvalidOperationException("MainThreadDispatchQueue not set.  Did you call P42.Utils.Uno.Platform.Init()?");
        
        if (IsMainThread)
            return Task.FromResult(func());

        var tcs = new TaskCompletionSource<T>();
        if (!Platform.MainThreadDispatchQueue.TryEnqueue(DispatcherQueuePriority.Normal, Callback))
            throw new InvalidOperationException("Unable to queue on the main thread.");

        return tcs.Task;

        void Callback()
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
        }
    }

    /// <summary>
    /// Asynchronously invoke async function on main thread
    /// </summary>
    /// <param name="funcTask"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task InvokeOnMainThreadAsync(Func<Task> funcTask)
    {
        if (Platform.MainThreadDispatchQueue is null)
            throw new InvalidOperationException("MainThreadDispatchQueue not set.  Did you call P42.Utils.Uno.Platform.Init()?");
        
        if (IsMainThread)
            return funcTask();

        var tcs = new TaskCompletionSource<object?>();

        if (!Platform.MainThreadDispatchQueue.TryEnqueue(DispatcherQueuePriority.Normal, Callback))
            throw new InvalidOperationException("Unable to queue on the main thread.");

        return tcs.Task;

        async void Callback()
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
        }
    }
    
    /// <summary>
    /// Invoke async function on main thread
    /// </summary>
    /// <param name="funcTask"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T InvokeOnMainThread<T>(Func<Task<T>> funcTask)
    {
        var task = Task.Run(async () => await InvokeOnMainThreadAsync(funcTask));
        var result = task.Result;
        return result;
    }
    

    /// <summary>
    /// Async invoke async function on main thread
    /// </summary>
    /// <param name="funcTask"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task<T> InvokeOnMainThreadAsync<T>(Func<Task<T>> funcTask)
    {
        if (Platform.MainThreadDispatchQueue is null)
            throw new InvalidOperationException("MainThreadDispatchQueue not set.  Did you call P42.Utils.Uno.Platform.Init()?");
        
        if (IsMainThread)
            return funcTask();

        var tcs = new TaskCompletionSource<T>();
        if (!Platform.MainThreadDispatchQueue.TryEnqueue(DispatcherQueuePriority.Normal, Callback))
            throw new InvalidOperationException("Unable to queue on the main thread.");

        return tcs.Task;

        async void Callback()
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
        }
    }

    public static Task InvokeOnMainThreadAsync(Task funcTask, CancellationToken cancellationToken = default)
    {
        if (Platform.MainThreadDispatchQueue is null)
            throw new InvalidOperationException("MainThreadDispatchQueue not set.  Did you call P42.Utils.Uno.Platform.Init()?");
        
        if (IsMainThread)
            return funcTask;

        var tcs = new TaskCompletionSource<bool>();
        if (!Platform.MainThreadDispatchQueue.TryEnqueue(DispatcherQueuePriority.Normal, Callback))
            throw new InvalidOperationException("Unable to queue on the main thread.");

        return tcs.Task;

        async void Callback()
        {
            try
            {
                await funcTask.WaitAsync(cancellationToken).ConfigureAwait(false);
                tcs.SetResult(true);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        }
    }
    
    public static Task<T> InvokeOnMainThreadAsync<T>(Task<T> funcTask, CancellationToken cancellationToken = default)
    {
        if (Platform.MainThreadDispatchQueue is null)
            throw new InvalidOperationException("MainThreadDispatchQueue not set.  Did you call P42.Utils.Uno.Platform.Init()?");
        
        if (IsMainThread)
            return funcTask;

        var tcs = new TaskCompletionSource<T>();
        if (!Platform.MainThreadDispatchQueue.TryEnqueue(DispatcherQueuePriority.Normal, Callback))
            throw new InvalidOperationException("Unable to queue on the main thread.");

        return tcs.Task;

        async void Callback()
        {
            try
            {
                var ret = await funcTask.WaitAsync(cancellationToken).ConfigureAwait(false);
                tcs.SetResult(ret);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        }
    }
    
}
