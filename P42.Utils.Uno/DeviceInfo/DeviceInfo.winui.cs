#if WINDOWS 
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media.Animation;
using P42.Serilog.QuickLog;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string GetManufacturer() => EasDeviceInfo.SystemManufacturer;

    private static string GetModel()
    {
        try
        {
            var data = ExecuteCommand("wmic computersystem get model");
            return data.Replace("\r", "").Replace("\n", "").Replace("Model", "").Trim();
        }
        catch (Exception ex)
        {
            QLog.Warning(ex, "wmic computersystem get model");
        }

        return EasDeviceInfo.SystemProductName;
    }

    private static string GetDeviceName()
    {
        try
        {
            var data = ExecuteCommand("wmic computersystem get name");
            return data.Replace("\r", "").Replace("\n", "").Replace("Name", "").Trim();
        }
        catch (Exception ex)
        {
            QLog.Warning(ex, "wmic computersystem get name");
        }

        return EasDeviceInfo.FriendlyName;
    }

    private static string GetDeviceId()
    {
        try
        {
            var data = ExecuteCommand("wmic csproduct get uuid");
            data = data.Replace("\r", "").Replace("\n", "").Replace("UUID", "").Trim();
            if (IsValidId(data)) 
                return data;

        }
        catch (Exception ex)
        {
            QLog.Warning(ex, "mic computersystem get uuid");
        }

        return EasDeviceInfo.Id.ToString();
    }

    private static bool GetIsEmulator() => false;
    

}
#endif
