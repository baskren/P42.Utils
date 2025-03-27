using System;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils;

[Obsolete("If you are using this, please let the author know so it can be preserved")]
public class AsyncAwaitForSet<T>
{
    [Obsolete("If you are using this, please let the author know so it can be preserved")]
    public AsyncAwaitForSet(Func<T, bool> onSetAction, Action? onStartAction = null)
    {
        _onSetAction = onSetAction;
        onStartAction?.Invoke();
    }

    #region Async Constructor / Presenter / Result
    private volatile TaskCompletionSource<T> _tcs = new();

    private readonly Func<T, bool>? _onSetAction;

    private AsyncAwaitForSet<T>? _currentInstance;
    [Obsolete("If you are using this, please let the author know so it can be preserved")]
    public AsyncAwaitForSet<T>? CurrentInstance => _currentInstance; 

    [Obsolete("If you are using this, please let the author know so it can be preserved")]
    public Task<T> Result()
    {
        return _tcs.Task;
    }


    [Obsolete("If you are using this, please let the author know so it can be preserved")]
    public void Set(T result)
    {
        if (_onSetAction != null && !_onSetAction.Invoke(result))
            return;
        _tcs.TrySetResult(result);
        _currentInstance = null;
    }

    [Obsolete("If you are using this, please let the author know so it can be preserved")]
    public void Reset()
    {
        while (true)
        {
            var tcs = _tcs;
            if (!tcs.Task.IsCompleted ||
                Interlocked.CompareExchange(ref _tcs, new TaskCompletionSource<T>(), tcs) == tcs)
                return;
        }
    }
    #endregion

}
