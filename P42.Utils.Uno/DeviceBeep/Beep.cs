using System.Collections.Concurrent;
using P42.Serilog.QuickLog;


namespace P42.Utils.Uno;

public static partial class DeviceBeep
{

    public static bool CanBeep
        => PlatformCanBeep();

    /// <summary>
    /// Beep 
    /// </summary>
    /// <param name="frequency"></param>
    /// <param name="duration"></param>
    public static async Task PlayAsync(int frequency = 1500, int duration = 300)
    {
//#if BROWSERWASM
//        await DeviceBeep.PlatformBeepAsync(frequency, duration);
//#else

        if (!PlatformCanBeep())
            return;

        Init();

        var tcs = new TaskCompletionSource<bool>();
        _queue?.Enqueue((frequency, duration, tcs));
        await tcs.Task;
//#endif
    }


//#if !BROWSERWASM
    private static ConcurrentQueue<(int Frequency, int Duration, TaskCompletionSource<bool> Tcs)>? _queue;

    private static void Init()
    {
        if (!PlatformCanBeep())
            return;

        if (_queue != null)
            return;

        _queue = new();


        // Start a consumer task
        Task.Run(async () =>
        {
            TaskCompletionSource<bool>? tcs = null;
            while (true)
            {
                try
                {
                    if (_queue.TryDequeue(out var item))
                    {
                        tcs = item.Tcs;
                        await PlatformBeepAsync(item.Frequency, item.Duration);
                        tcs.TrySetResult(true);
                    }
                    else
                    {
                        tcs = null;
                        Thread.Sleep(50); // Avoid busy-waiting
                    }
                }
                catch (Exception ex)
                {
                    QLog.Error(ex);
                    tcs?.TrySetException(ex);
                    tcs = null;
                }
            }
            // ReSharper disable once FunctionNeverReturns
        });
    }
//#endif


}
