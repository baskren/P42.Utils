namespace P42.Utils.Uno;

public static class DesktopNative
{
    public static Task BeepAsync(int frequency, int duration)
    {
        if (OperatingSystem.IsMacOS())
            MacNative.PlatformBeep(frequency, duration);
        else if (OperatingSystem.IsWindows())
            Console.Beep(frequency, duration);
        else if (OperatingSystem.IsLinux())
            return LinuxNative.BeepAsync(frequency, duration);
        else
            throw new PlatformNotSupportedException("Platform not supported");
        
        return Task.CompletedTask;
    }

    public static bool CanBeep()
    {
        return OperatingSystem.IsMacOS() || OperatingSystem.IsWindows() || OperatingSystem.IsLinux();
    }
}
