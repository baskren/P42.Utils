using System;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils;

//public delegate bool TimerCallback(object state);

public sealed class Timer : CancellationTokenSource
{
    /*
    #region Private Fields
    private static int _count;
    private readonly Func<Task, object?, Task> _function;
    private TimerCallback? _callback;
    private object? _state;
    private TimeSpan _dueTime;
    private TimeSpan _period;
    private readonly TimeSpan _infinite = new(0, 0, Timeout.Infinite);
    private readonly int _id;
    #endregion
    */
    
    #region Constructors

    /// <summary>
    /// Start Environment.PlatformTimer
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="callback"></param>
    /// <exception cref="Exception"></exception>
    public static void StartTimer(TimeSpan interval, Func<bool> callback)
    {
        if (Environment.PlatformTimer is null)
            throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
        Environment.PlatformTimer.StartTimer(interval, callback);
    }

    /// <summary>
    /// Start Environment.PlatformTimer
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="callback"></param>
    /// <exception cref="Exception"></exception>
    public static void StartTimer(TimeSpan interval, Func<Task<bool>> callback)
    {
        if (Environment.PlatformTimer is null)
            throw new Exception("Enviroment not set.  Did you initiate P42.Utils.**platform**?");
        Environment.PlatformTimer.StartTimer(interval, callback);
    }
    #endregion Constructors

/* 
    public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
    {
        this.AddToCensus();
        
        _id = _count++;
        _callback = callback;
        _state = state;
        _function = Function;
        
        Change(dueTime, period);
    }

    private async Task Function(Task t, object? s)
    {
        while (!IsCancellationRequested)
        {
            if (_period == _infinite) return;
            if (_callback is null) return;
            if (!_callback(_state)) _period = _infinite;
            if (_period == _infinite)
                return;
            await Task.Delay(_period);
        }
    }

    #endregion;

    #region Disposal
    bool _disposed;
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            _callback = null;
            _state = null;
            base.Cancel();
            this.RemoveFromCensus();
        }
        base.Dispose(disposing);
    }

    #endregion

    #region Utility
    public override string ToString()
    {
        return string.Format("[Timer " + _id + "]");
    }
    #endregion

    #region Changers
    public new void Cancel()
    {
        Change(_infinite, _infinite);
    }

    //public void Change(Int32 dueTime=Timeout.Infinite, Int32 period=Timeout.Infinite) {
    //	Change(new TimeSpan (0, 0, 0, 0, dueTime), new TimeSpan (0, 0, 0, 0, period));
    //}

    public void Change(uint dueTime, uint period)
    {
        Change(new TimeSpan(0, 0, 0, 0, (int)dueTime), new TimeSpan(0, 0, 0, 0, (int)period));
    }

    public void Change(long dueTime = Timeout.Infinite, long period = Timeout.Infinite)
    {
        Change(new TimeSpan(0, 0, 0, 0, (int)dueTime), new TimeSpan(0, 0, 0, 0, (int)period));
    }

    /// <summary>
    /// Change the Timer's due btime and period
    /// </summary>
    /// <param name="dueTime">first fire</param>
    /// <param name="period">repeated at period</param>
    public void Change(TimeSpan dueTime, TimeSpan period)
    {
        //if (_task != null)
        //	throw new NotImplementedException ("");
        _dueTime = dueTime;
        _period = period;
        if (_dueTime != _infinite)
            Task.Delay(_dueTime, Token).ContinueWith(
                _function,
                Tuple.Create(_callback, _state),
                CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                TaskScheduler.Default
            );
    }
    #endregion
    */
}
