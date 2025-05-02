#if WinAppSdk || DESKTOP
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{
    private static bool PlatformCanBeep() => true;

    private static async Task PlatformBeepAsync(int frequency, int duration)
    {
        Console.Beep(frequency, duration);
        await Task.CompletedTask;
    }

}
#endif
