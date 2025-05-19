#if __WASM__
using System;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.UI.Xaml.Media.Animation;
using P42.Serilog.QuickLog;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static Dictionary<string, Dictionary<string, string>>? _browserOverview;
    private static Dictionary<string, Dictionary<string, string>>? BrowserOverview
    {
        get
        {
            if (_browserOverview != null)
                return _browserOverview;

            try
            {
                var script = """
                             require([`${config.uno_app_base}/es5.js`], c => Bowser = c);
                             const browser = Bowser.getParser(window.navigator.userAgent);
                             JSON.stringify(browser.parse());
                             """;
                var json = global::Uno.Foundation.WebAssemblyRuntime.InvokeJS(script);
                var dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, string>>>>(json);
                return _browserOverview = dict["parsedResult"];
            }
            catch (Exception ex)
            {
                QLog.Warning(ex);
            }

            return null;
        }
    }


    private static string GetManufacturer()
    {
        if (BrowserOverview is not null && 
            BrowserOverview.TryGetValue("browser", out var browser) && 
            browser.TryGetValue("name", out var name))
            return name;

        return EasDeviceInfo.SystemManufacturer;
    }

    private static string GetModel()
    {
        if (BrowserOverview is not null &&
            BrowserOverview.TryGetValue("browser", out var browser) && 
            browser.TryGetValue("version", out var version))
            return version;

        return EasDeviceInfo.SystemProductName;
    }

    private static string GetDeviceName()
        => EasDeviceInfo.FriendlyName;
    
    private static string GetDeviceId() => GetGeneratedDeviceId();

    private static bool GetIsEmulator() => false;
}
#endif
