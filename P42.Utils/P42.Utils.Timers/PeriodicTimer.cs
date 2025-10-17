using System;
using System.Threading.Tasks;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public sealed class PeriodicTimer
{
    /// <summary>
    /// Starts a recurring timer. Callbacks slower than period will cause unintended invocations.  Slow and inconsistent callbacks may cause race conditions. Similar to Xamarin.Forms.Timer.
    /// </summary>
    /// <param name="period">The time between invocation of callback</param>
    /// <param name="callback">returns false when you wish timer to stop.</param>
    public static void StartTimer(TimeSpan period, Func<bool> callback, CancellationToken token = default)
    {
        if (period <= TimeSpan.Zero)
            return;

        _ = new PeriodicTimer(period, callback, token);
    }

    /// <summary>
    /// Starts a recurring timer. Callbacks slower than period will cause unintended invocations.  Slow and inconsistent callbacks may cause race conditions. Similar to Xamarin.Forms.Timer.
    /// </summary>
    /// <param name="period">The time between invocation of callback</param>
    /// <param name="callback">returns false when you wish timer to stop.</param>
    public static void StartTimer(TimeSpan period, Func<Task<bool>> callback, CancellationToken token = default)
    {
        if (period <= TimeSpan.Zero)
            return;

        _ = new PeriodicTimer(period, callback, token);
    }

    private PeriodicTimer(TimeSpan span, Func<bool>? func, CancellationToken token = default)
    {
        if (func == null)
            return;
        if (span <= TimeSpan.Zero)
            return;

        var timer = new System.Timers.Timer();
        timer.Elapsed += (_, _) =>
        {
            if (!token.IsCancellationRequested && func.Invoke())
                return;
            timer.Stop();
            func = null;
        };
        timer.Interval = span.TotalMilliseconds;
        timer.Start();
    }

    private PeriodicTimer(TimeSpan span, Func<Task<bool>>? func, CancellationToken token = default)
    {
        if (func == null)
            return;
        if (span <= TimeSpan.Zero)
            return;
        
        var timer = new System.Timers.Timer();
        timer.Elapsed += async (_, _) =>
        {
            if (!token.IsCancellationRequested && await func.Invoke())
                return;
            timer.Stop();
            func = null;
        };
        timer.Interval = span.TotalMilliseconds;
        timer.Start();
    }
    
    
}
