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
