using System.Runtime.InteropServices;

namespace P42.Utils.Uno;

public static partial class MacNative
{
    
    [LibraryImport("Platforms/Desktop/libNativeBeep.dylib")]
    internal static partial void PlatformBeep(double frequency, int duration);

}
