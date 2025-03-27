using System;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils;

public delegate bool TimerCallback(object state);

[Obsolete("Please use IntervalTimer or PeriodicTimer instead.", true)]
public sealed class Timer
{

    /// <summary>
    /// Starts a recurring timer
    /// </summary>
    /// <param name="interval">The interval between invocations of the callback</param>
    /// <param name="callback">The action to run when the timer elapses.</param>
    /// <exception cref="Exception">Thrown when P42.Utils is not initialized properly</exception>
    [Obsolete("Please use IntervalTimer or PeriodicTimer instead.", true)]
    public static void StartTimer(TimeSpan interval, Func<bool> callback)
    {
    }

    /// <summary>
    /// Starts a recurring timer
    /// </summary>
    /// <param name="interval">The interval between invocations of the callback</param>
    /// <param name="callback">The action to run when the timer elapses.</param>
    /// <exception cref="Exception">Thrown when P42.Utils is not initialized properly</exception>
    [Obsolete("Please use IntervalTimer or PeriodicTimer instead.", true)]
    public static void StartTimer(TimeSpan interval, Func<Task<bool>> callback)
    {
    }


}
