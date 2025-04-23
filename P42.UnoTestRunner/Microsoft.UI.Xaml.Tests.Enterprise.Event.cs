using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.UI.Xaml.Tests.Enterprise;

internal class Event
{
    internal int FiredCount { get; private set; }

    private TaskCompletionSource<bool>? _tcs;

    internal async Task<bool> WaitForDefault(int timeout = 5000, CancellationToken ct = default)
    {
        var tcs = EnsureTcs();

        var timeoutTask = Task.Delay(timeout, ct);

        var winningTask = await Task.WhenAny(timeoutTask, tcs.Task);

        if (winningTask == timeoutTask)
        {
            throw new TimeoutException("Timed out waiting for event to fire");
        }

        return await tcs.Task;
    }


    public async Task<bool> WaitForNoThrow(int timeoutValueMs, bool enforceUnderDebugger = true)
    {
        // Event timeouts can be frustrating when trying to debug tests. 
        // When under the debugger we disable all timeouts.
        //if (IsDebuggerPresent() && !enforceUnderDebugger)
        //{
        //	timeoutValueMs = INFINITE;
        //}

        try
        {
            await WaitForDefault(timeoutValueMs);
            return true;
        }
        catch (TimeoutException)
        {
            return false;
        }
        //return WaitForSingleObject(m_handle, timeoutValueMs) == WAIT_OBJECT_0;
    }

    private TaskCompletionSource<bool> EnsureTcs()
    {
        var newTcs = new TaskCompletionSource<bool>();
        var tcs = Interlocked.CompareExchange(ref _tcs, newTcs, null) ?? newTcs;
        return tcs;
    }

    internal void Set()
    {
        EnsureTcs().TrySetResult(true);
        FiredCount++;
    }

    internal bool HasFired()
    {
        var tcs = _tcs;
        return tcs != null && tcs.Task.IsCompleted;
    }

    public void Reset() => Interlocked.Exchange(ref _tcs, null)?.TrySetCanceled();

    public Task WaitFor(TimeSpan timeout, CancellationToken ct = default)
    {
        return WaitForDefault((int)timeout.TotalMilliseconds, ct);
    }
}
