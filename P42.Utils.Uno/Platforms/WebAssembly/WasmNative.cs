#if BROWSERWASM
using System.Runtime.InteropServices.JavaScript;

namespace P42.Utils.Uno;

public static partial class WasmNative
{
    [JSImport("globalThis.P42_Utils_Uno_Beep")]
    internal static partial void NativeBeep(int frequency, int duration);

}
#endif
