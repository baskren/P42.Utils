using System.Diagnostics.CodeAnalysis;

namespace P42.Utils.Uno;

public static partial class DeviceBeep
{
    #if !__WASM__ && !__ANDROID__ && !__IOS__ && !DESKTOP && !WINDOWS 
        private static bool PlatformCanBeep()
            => false;
    
        // ReSharper disable once UnusedParameter.Local
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        private static async Task PlatformBeepAsync(int freq, int duration)
            => throw new PlatformNotSupportedException();
    #endif

}
