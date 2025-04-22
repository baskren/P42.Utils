using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;
//using AVFoundation;

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
#if BROWSERWASM
        await DeviceBeep.PlatformBeepAsync(frequency, duration);
#else
        Init();
        var tcs = new TaskCompletionSource<bool>();
        Queue?.Enqueue((frequency, duration, tcs));
        await tcs.Task;
#endif
    }


    static ConcurrentQueue<(int Frequency, int Duration, TaskCompletionSource<bool> Tcs)>? Queue;

    static void Init()
    {
        if (!PlatformCanBeep())
            return;

        if (Queue == null)
        {
            Queue = new();


            // Start a consumer task
            Task consumer = Task.Run(async () =>
            {
                TaskCompletionSource<bool>? tcs = null;
                while (true)
                {
                    try
                    {
                        if (Queue.TryDequeue(out var item))
                        {
                            tcs = item.Tcs;
                            await DeviceBeep.PlatformBeepAsync(item.Frequency, item.Duration);
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
            });
        }
    }


#if __IOS__ || __MACCATALYST__ || __ANDROID__ || WINDOWS || BROWSERWASM
#else
    static bool PlatformCanBeep() => false;

    static Task PlatformBeepAsync(int freq, int duration) => Task.CompletedTask;
#endif
}
