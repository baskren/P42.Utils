
using System;
using System.Threading.Tasks;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public sealed class IntervalTimer 
{

    #region Constructors

    /// <summary>
    /// Starts a recurring timer.  Interval is not the same as period - callback processing time will impact period.
    /// </summary>
    /// <param name="interval">The time between end of one invocation of the callback and the next</param>
    /// <param name="callback">The action to run when the timer elapses.</param>
    /// <exception cref="Exception">Thrown when P42.Utils is not initialized properly</exception>
    public static void StartTimer(TimeSpan interval, Func<bool> callback)
    {
        if (interval <= TimeSpan.Zero)
            return;

        _ = new IntervalTimer(interval, callback);
    }


    /// <summary>
    /// Starts a recurring timer.  Interval is not the same as period - callback processing time will impact period.
    /// </summary>
    /// <param name="interval">The time between end of one invocation of the callback and the next</param>
    /// <param name="callback">The action to run when the timer elapses.</param>
    /// <exception cref="Exception">Thrown when P42.Utils is not initialized properly</exception>
    public static void StartTimer(TimeSpan interval, Func<Task<bool>> callback)
    {
        if (interval <= TimeSpan.Zero)
            return;

        _ = new IntervalTimer(interval, callback);
    }

    #endregion


    #region Private Fields
    private readonly Action? _tick;
    #endregion

    
    #region Constructors
    private IntervalTimer(TimeSpan period, Func<bool>? callback)
    {
        Action<Task> process = _ =>
        {
            if (callback!.Invoke())
                _tick!.Invoke();
            else
            {
                callback = null;
                GC.Collect();
            }
        };
        
        _tick = () =>
        {
            Task.Delay(period).ContinueWith(
                process, 
                default,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Current
            );
        };
        
        _tick.Invoke();
    }

    private IntervalTimer(TimeSpan period, Func<Task<bool>>? callback)
    {
        if (callback == null)
            return;
        if (period <= TimeSpan.Zero)
            return;
        
        Func<Task, Task> process = async _ =>
        {
            if (await callback.Invoke())
                _tick!.Invoke();
            else
            {
                callback = null;
                GC.Collect();
            }
        };
        
        _tick = () =>
        {
            Task.Delay(period).ContinueWith(
                process, 
                default,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Current
            );
        };
        
        _tick.Invoke();
    }

    
    #endregion;

}
