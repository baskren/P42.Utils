using System.Diagnostics.CodeAnalysis;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{
    private static bool PlatformCanBeep()
        => false;

    // ReSharper disable once UnusedParameter.Local
    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private static async Task PlatformBeepAsync(int freq, int duration)
        => throw new PlatformNotSupportedException();

}
