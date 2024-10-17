using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils;

public static class ThreadExtensions
{
    /// <summary>
    /// Default exception handler for Fire and Forget Tasks 
    /// </summary>
    public static Action<Thread, Exception> DefaultExceptionHandler { get; set; } = Serilog.QuickLog.QLogExtensions.LogException;

        
    /// <summary>
    /// Fire and Forget a Task
    /// </summary>
    /// <param name="task">Task</param>
    /// <param name="onException">optional Handler</param>
    /// <param name="callingMethodName">optional caller</param>
    [SuppressMessage("ReSharper", "VariableHidesOuterVariable", Justification = "Pass params explicitly to async local function or it will allocate to pass them")]
    public static void Forget(this Task task, Action<Thread, Exception>? onException = null,  [CallerFilePath] string? sourceFilePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        ArgumentNullException.ThrowIfNull(task);

        // note: this code is inspired by a tweet from Ben Adams: https://twitter.com/ben_a_adams/status/1045060828700037125
        // Only care about tasks that may fault (not completed) or are faulted,
        // so fast-path for SuccessfullyCompleted and Canceled tasks.
        if (!task.IsCanceled && (!task.IsCompleted || task.IsFaulted))
        {
            // use "_" (Discard operation) to remove the warning IDE0058: Because this call is not awaited, execution of the
            // current method continues before the call is completed - https://docs.microsoft.com/en-us/dotnet/csharp/discards#a-standalone-discard
            _ = ForgetAwaited(task, onException, DebugExtensions.ParentCodeWaypoint(callerPath: sourceFilePath, lineNumber:lineNumber));
        }
    }

    // Allocate the async/await state machine only when needed for performance reasons.
    // More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/?WT.mc_id=DT-MVP-5003978
    // Pass params explicitly to async local function otherwise it will allocate to pass them
    private static async Task ForgetAwaited(Task task, Action<Thread, Exception>? onException, string callerWaypoint)
    {
        try
        {
            await task;
        }
        catch (TaskCanceledException)
        {
            // log a message if we were given a logger to use
            System.Diagnostics.Debug.WriteLine($"Fire and forget task was canceled for calling method: {callerWaypoint}");
        }
        catch (Exception e)
        {
            // log a message if we were given a logger to use
            var msg =
                $"Fire and forget task failed for calling method: {callerWaypoint} [{e.Message}][{e.StackTrace}]";
            System.Diagnostics.Debug.WriteLine(msg);
            Console.WriteLine(msg);

            onException ??= DefaultExceptionHandler;
            onException.Invoke(Thread.CurrentThread, new FireAndForgetException(callerWaypoint, e));
        }
    }

}

public class FireAndForgetException(string callerWaypoint, Exception innerException)
    : Exception($"Fire and forget task, from calling waypoint [{callerWaypoint}], failed.", innerException)
{
    /// <summary>
    /// Method that called the FireAndForget task
    /// </summary>
    public string CallerWaypoint { get; private set; } = callerWaypoint;
}
