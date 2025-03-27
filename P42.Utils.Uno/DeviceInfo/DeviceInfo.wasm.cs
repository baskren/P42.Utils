#if __WASM__
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static readonly EasClientDeviceInformation deviceInfo;
    private static string _systemProductName = string.Empty;

    static DeviceInfo()
    {
        deviceInfo = new EasClientDeviceInformation();
        try
        {
            _systemProductName = deviceInfo.SystemProductName;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get system product name. {ex.Message}");
        }
    }

    private static string GetModel() => deviceInfo.SystemProductName;

    private static string GetManufacturer() => deviceInfo.SystemManufacturer;

    private static string GetDeviceName() => deviceInfo.FriendlyName;
    
    private static bool GetIsEmulator() => false;
}
#endif
