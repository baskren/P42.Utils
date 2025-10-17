using System.Runtime.InteropServices;

namespace P42.Utils.Uno;

public static partial class MacNative
{
    
    [LibraryImport("Native/libNativeBeep.dylib")]
    public static partial void PlatformBeep(double frequency, int duration);

}
