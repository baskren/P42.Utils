using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

using _Impl = Microsoft.UI.Dispatching.DispatcherQueue;
using _Handler = Microsoft.UI.Dispatching.DispatcherQueueHandler;
using _Priority = Microsoft.UI.Dispatching.DispatcherQueuePriority;



namespace P42.UnoTestRunner;

public partial class UnitTestDispatcherCompat
{
    public enum Priority { Low = -1, Normal = 0, High = 1 }

    private static _Priority RemapPriority(Priority priority) => priority switch
    {
        // [uwp] Windows.UI.Core.CoreDispatcherPriority::Idle doesn't have a counterpart, and is thus ignored.

        Priority.Low => _Priority.Low,
        Priority.Normal => _Priority.Normal,
        Priority.High => _Priority.High,

        _ => throw new ArgumentOutOfRangeException($"Invalid value: ({priority:d}){priority}"),
    };
}

public partial class UnitTestDispatcherCompat
{
    private readonly _Impl _impl;

    private readonly Windows.UI.Core.CoreDispatcher? _dispatcher;
    public UnitTestDispatcherCompat(_Impl impl, Windows.UI.Core.CoreDispatcher? dispatcher = null)
    {
        this._impl = impl;
        this._dispatcher = dispatcher;
    }

    public Windows.Foundation.IAsyncAction RunIdleAsync(Windows.UI.Core.IdleDispatchedHandler agileCallback)
    {
        // For now, Uno WinUI Dispatcher is non-null.
        // In the future, this will break and it will be null.
        if (_dispatcher is not null)
        {
            return _dispatcher.RunIdleAsync(agileCallback);
        }

        // This code path is for Windows WinUI, and "potentially" Uno WinUI in future when the breaking change is taken.
        // This is a wrong implementation. It doesn't really wait for "Idle".
        return RunAsync(UnitTestDispatcherCompat.Priority.Low, () => { }).AsAsyncAction();
    }

    public static UnitTestDispatcherCompat From(UIElement x) =>
        new UnitTestDispatcherCompat(x.DispatcherQueue, x.Dispatcher);

    public static UnitTestDispatcherCompat From(Window x) =>
        new UnitTestDispatcherCompat(x.DispatcherQueue, x.Dispatcher);

    public static UnitTestDispatcherCompat Instance
    {
        get
        {
#if WINAPPSDK
		    return new UnitTestDispatcherCompat(_Impl.GetForCurrentThread());
#else
            var dispatchQueueType = typeof(_Impl);
            var mainGetter = dispatchQueueType.GetMethod("get_Main", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var mainValue = mainGetter.Invoke(null, null) as _Impl;
            return new UnitTestDispatcherCompat(mainValue);
#endif
        }
    }


    public bool HasThreadAccess => _impl.HasThreadAccess;

    public void Invoke(_Handler handler) => Invoke(default, handler);
    public void Invoke(Priority priority, _Handler handler)
    {
        _impl.TryEnqueue(RemapPriority(priority), handler);
    }
    public void InvokeWithBypass(_Handler handler) => InvokeWithBypass(default, handler);
    public void InvokeWithBypass(Priority priority, _Handler handler)
    {
        if (_impl.HasThreadAccess)
            handler();
        else
            Invoke(priority, handler);

    }

    public Task RunAsync(_Handler handler) => RunAsync(default, handler);
    public Task RunAsync(Priority priority, _Handler handler)
    {
        var tcs = new TaskCompletionSource<object>();

        _impl.TryEnqueue(RemapPriority(priority), () =>
        {
            try
            {
                handler();
                tcs.TrySetResult(default!);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        });

        return tcs.Task;
    }
    public Task RunAsyncWithBypass(_Handler handler) => RunAsyncWithBypass(default, handler);
    public Task RunAsyncWithBypass(Priority priority, _Handler handler)
    {
        if (_impl.HasThreadAccess)
        {
            handler();
            return Task.CompletedTask;
        }
        else
            return RunAsync(priority, handler);

    }
}
