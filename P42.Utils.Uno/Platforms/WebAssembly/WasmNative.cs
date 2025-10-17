#if BROWSERWASM
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;

namespace P42.Utils.Uno;

public static partial class WasmNative
{
    [JSImport("globalThis.P42_Utils_Uno_Beep")]
    internal static partial void NativeBeep(int frequency, int duration);

    [JSImport("globalThis.P42_Utils_Uno_GetPageUrl")]
    internal static partial string GetPageUrl();
    
    [JSImport("globalThis.P42_Utils_Uno_SetPageUrl")]
    internal static partial void SetPageUrl(string url);
    
    [JSImport("globalThis.P42_Utils_Uno_BrowserWasmUserAgent")]
    internal static partial string GetUserAgent();
    
    [JSImport("globalThis.P42_Utils_Uno_StorageEstimateJsonAsync")]
    internal static partial Task<string> StorageEstimateJsonAsync();
    
    [JSImport("globalThis.P42_Utils_Uno_SSetCookie")]
    private static partial void SetCookie(string cookieString);

    public static void SetCookie(string name, string value, DateTimeOffset expires = default, string path = "/")
    {
        if (expires == default)
            expires = DateTimeOffset.MaxValue;
        
        var cookieString = $"{name}={value}; expires={expires.UtcDateTime:ddd, dd MMM yyyy HH:mm:ss} GMT; path={path}";
        SetCookie(cookieString);
    }
    
    [JSImport("globalThis.P42_Utils_Uno_SGetCookies")]
    private static partial string GetCookiesString();

    public static bool TryGetCookie(string name, [MaybeNullWhen(false)] out string value)
        => GetCookies().TryGetValue(name, out value);
    
    public static Dictionary<string, string> GetCookies()
    {
        var results = new Dictionary<string, string>();
        var cookieString = GetCookiesString();
        var cookies = cookieString.Split(';');
        foreach (var cookie in cookies)
        {
            var cookieParts = cookie.Split('=');
            var cookieName = cookieParts[0].Trim();
            results[cookieName] = cookieParts.Length >= 2
                ? cookieParts[1]
                : string.Empty;
        }
        return results;
    }
}
#endif
