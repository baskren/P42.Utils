
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
        if (WasmNative.TryGetCookie(deviceIdKey, out var id))
            return id;

        id = FallbackId();
        WasmNative.SetCookie(deviceIdKey, id);
        return id;
    }

    private static bool GetIsEmulator() => false;
    
    public static string QueryDeviceOs() => $"Browser:[{BrowserInfo.OS.Family}]";

    public static string QueryDeviceOsVersion() => $"{BrowserInfo.OS.Major}.{BrowserInfo.OS.Minor}.{BrowserInfo.OS.Patch}";

}


