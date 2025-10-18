#if BROWSERWASM
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace P42.Utils.Uno;

internal static partial class WasmNative
{

    [JSImport("globalThis.P42_Utils_Uno_GetPageUrl")]
    internal static partial string GetPageUrl();
    
    [JSImport("globalThis.P42_Utils_Uno_SetPageUrl")]
    internal static partial void SetPageUrl(string url);
    
}
#endif
