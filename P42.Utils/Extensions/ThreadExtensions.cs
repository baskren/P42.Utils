using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace P42.Utils
{
    public static class ThreadExtensions
    {
        [SuppressMessage("ReSharper", "VariableHidesOuterVariable", Justification = "Pass params explicitly to async local function or it will allocate to pass them")]
        public static void Forget(this Task task, Action<Exception, Dictionary<string,string>> exceptionAction, [CallerMemberName] string callingMethodName = "", [CallerFilePath] string callerPath = "")
        {
            if (task == null) throw new ArgumentNullException(nameof(task));

            // note: this code is inspired by a tweet from Ben Adams: https://twitter.com/ben_a_adams/status/1045060828700037125
            // Only care about tasks that may fault (not completed) or are faulted,
            // so fast-path for SuccessfullyCompleted and Canceled tasks.
            if (!task.IsCanceled && (!task.IsCompleted || task.IsFaulted))
            {
                // use "_" (Discard operation) to remove the warning IDE0058: Because this call is not awaited, execution of the
                // current method continues before the call is completed - https://docs.microsoft.com/en-us/dotnet/csharp/discards#a-standalone-discard
                _ = ForgetAwaited(task, exceptionAction, callingMethodName, callerPath);
            }
        }

        // Allocate the async/await state machine only when needed for performance reasons.
        // More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/?WT.mc_id=DT-MVP-5003978
        // Pass params explicitly to async local function or it will allocate to pass them
        static async Task ForgetAwaited(Task task, Action<Exception, Dictionary<string, string>> exceptionAction, string callingMethodName = "", [CallerFilePath] string callerPath = "")
        {
            try
            {
                await task;
            }
            catch (TaskCanceledException)
            {
                // log a message if we were given a logger to use
                //Serilog.Log.Error(tce, $"Fire and forget task was canceled for calling method: {callingMethodName}");
                System.Diagnostics.Debug.WriteLine($"Fire and forget task was canceled for calling method: {callingMethodName}");
            }
            catch (Exception e)
            {
                // log a message if we were given a logger to use
                //Serilog.Log.Error(e, $"Fire and forget task failed for calling method: {callingMethodName}");
                //using (var p = Forms9Patch.Toast.Create("EXCEPTION : " + callingMethodName, e.Message)) { p.CancelOnPageOverlayTouch = p.CancelOnBackButtonClick = false; }

                System.Diagnostics.Debug.WriteLine($"Fire and forget task failed for calling method: {callingMethodName} [{e.Message}][{e.StackTrace}]");
                System.Console.WriteLine($"Fire and forget task failed for calling method: {callingMethodName} [{e.Message}][{e.StackTrace}]");

                if (exceptionAction is null)
                    Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() => throw e);
                else
                    exceptionAction?.Invoke(e, new Dictionary<string, string>
                    {
                        { "CallerName", callingMethodName },
                        { "CallerPath", callerPath }
                    });

            }
        }

    }
}
