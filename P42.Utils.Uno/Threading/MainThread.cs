namespace P42.Utils.Uno;

/// <summary>
/// Run on MainThread, padding exceptions back to waiting thread
/// </summary>
public static class MainThread
{
    /// <summary>
    /// Is this running on the main thread?
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static bool IsMainThread
    {
        get
        {
            if (Platform.MainThreadDispatchQueue == null)
                throw new Exception("Unable to find main thread dispatch queue.  Did you call P42.Utils.Uno.Platform.Init() ?");

            return Platform.MainThread == Thread.CurrentThread;
        }
    }
    
    /// <summary>
    /// run action on MainThread
    /// </summary>
    /// <param name="action"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void Invoke(Action action)
    {
        if (IsMainThread)
        {
            action();
            return;
        }

        if (!Platform.MainThreadDispatchQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, action.Invoke))
            throw new InvalidOperationException("Unable to queue on the main thread.");
    }

    /// <summary>
    /// run action on MainThread
    /// </summary>
    /// <param name="action"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task InvokeAsync(Action action)
    {
        if (IsMainThread)
        {
            action();
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource<bool>();
        return !Platform.MainThreadDispatchQueue.TryEnqueue(
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
            }) 
            ? throw new InvalidOperationException("Unable to queue on the main thread.") 
            : tcs.Task;
    }

    
    /// <summary>
    /// run function on MainThread
    /// </summary>
    /// <param name="func"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static T Invoke<T>(Func<T> func)
    {
        if (IsMainThread)
            return func();

        var task = Task.Run(async () => await InvokeAsync(func));
        var result = task.Result;
        return result;
    }

    /// <summary>
    /// run function on MainThread
    /// </summary>
    /// <param name="func"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task<T> InvokeAsync<T>(Func<T> func)
    {
        if (IsMainThread)
            return Task.FromResult(func());

        var tcs = new TaskCompletionSource<T>();
        return !Platform.MainThreadDispatchQueue.TryEnqueue(
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
            }) 
            ? throw new InvalidOperationException("Unable to queue on the main thread.") 
            : tcs.Task;
    }

    
    
    /// <summary>
    /// run task on MainThread
    /// </summary>
    /// <param name="funcTask"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static Task InvokeAsync(Func<Task> funcTask)
    {
        if (IsMainThread)
            return funcTask();

        var tcs = new TaskCompletionSource<bool>();
        return !Platform.MainThreadDispatchQueue.TryEnqueue(
            // use of async void is ok here since tcs.SetException() handles exceptions
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async void () =>
        {
            try
            {
                await funcTask().ConfigureAwait(false);
                tcs.SetResult(true);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        }) 
        ? throw new InvalidOperationException("Unable to queue on the main thread.") 
        : tcs.Task;
    }
    
    
    /// <summary>
    /// run task on MainThread
    /// </summary>
    /// <param name="funcTask"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static T Invoke<T>(Func<Task<T>> funcTask)
    {
        var task = Task.Run(async () => await InvokeAsync(funcTask));
        var result = task.Result;
        return result;
    }

    public static Task<T> InvokeAsync<T>(Func<Task<T>> funcTask)
    {
        if (IsMainThread)
            return funcTask();

        var tcs = new TaskCompletionSource<T>();
        return !Platform.MainThreadDispatchQueue.TryEnqueue(
            // use of async void is ok here since tcs.SetException() handles exceptions
            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, async void () =>
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
        }) 
            ? throw new InvalidOperationException("Unable to queue on the main thread.") 
            : tcs.Task;
    }
}
