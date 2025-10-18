#if BROWSERWASM
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace P42.Utils.Uno;

internal static partial class WasmNative
{
    [JSImport("globalThis.P42_Utils_Uno_StorageEstimateJsonAsync")]
    internal static partial Task<string> StorageEstimateJsonAsync();
    
}
#endif
