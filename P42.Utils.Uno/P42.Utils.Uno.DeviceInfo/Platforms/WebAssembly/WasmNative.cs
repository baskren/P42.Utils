#if BROWSERWASM
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace P42.Utils.Uno;

public static partial class WasmNative
{
    [JSImport("globalThis.P42_Utils_Uno_BrowserWasmUserAgent")]
    internal static partial string GetUserAgent();
    
    
}
#endif
