namespace P42.Utils.Uno;

public static partial class DeviceBeep
{
    private static bool PlatformCanBeep()
        => true;

    private static async Task PlatformBeepAsync(int freq, int duration)
    {
        var delay = Task.Delay(freq);
        var beep = Task.Run(() => WasmNative.NativeBeep(freq, duration));
        await Task.WhenAll(delay, beep);
    }
}
