
namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string? _userAgent;
    private static string UserAgent => _userAgent ??= WasmNative.GetUserAgent();
    
    private static UAParser.ClientInfo? _browserInfo;
    private static UAParser.ClientInfo BrowserInfo => _browserInfo ??= UAParser.Parser.GetDefault().Parse(UserAgent);

    private static string GetManufacturer() => BrowserInfo.Device.Brand;

    private static string GetModel() => BrowserInfo.Device.Model;

    private static string GetDeviceName() 
        => string.Empty;

    private static string GetDeviceId()
    {
        const string deviceIdKey = "P42.Utils.Uno.DeviceInfo.DeviceId";
        var manager = global::Uno.Web.Http.CookieManager.GetDefault(); 
        if (manager.FindCookie(deviceIdKey) is {} cookie)
            return cookie.Value;

        var id = FallbackId();
        cookie = new global::Uno.Web.Http.Cookie(deviceIdKey, id);
        var request = new global::Uno.Web.Http.SetCookieRequest(cookie)
        {
            Expires = DateTimeOffset.MaxValue        
        };
        manager.SetCookie(request);
        return id;
    }

    private static bool GetIsEmulator() => false;
    
    public static string QueryDeviceOs() => $"Browser:[{BrowserInfo.OS.Family}]";

    public static string QueryDeviceOsVersion() => $"{BrowserInfo.OS.Major}.{BrowserInfo.OS.Minor}.{BrowserInfo.OS.Patch}";

}


