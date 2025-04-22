#if WinAppSdk
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


    [DllImport("kernel32.dll")]
    static extern bool Beep(int frequency, int duration);


    private static async Task PlatformBeepAsync(int frequency, int duration)
    {
        Beep(frequency, duration);
        await Task.CompletedTask;
    }

}
#endif
