namespace P42.Utils.Uno;

public static partial class DeviceBeep
{
    private static bool PlatformCanBeep()
        => DesktopNative.CanBeep();
    private static Task PlatformBeepAsync(int frequency, int duration)
        => DesktopNative.BeepAsync(frequency, duration);
}
