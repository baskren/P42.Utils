#if __WINUI__ 
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace Xamarin.Essentials;

public static partial class DeviceInfo
{
    private static readonly EasClientDeviceInformation deviceInfo;
    private static string systemProductName;

    static DeviceInfo()
    {
        deviceInfo = new EasClientDeviceInformation();
        try
        {
            systemProductName = deviceInfo.SystemProductName;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unable to get system product name. {ex.Message}");
        }
    }

    private static string GetModel() => deviceInfo.SystemProductName;

    private static string GetManufacturer() => deviceInfo.SystemManufacturer;

    private static string GetDeviceName() => deviceInfo.FriendlyName;
    
}
#endif
