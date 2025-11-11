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
    private static string GetManufacturer() => string.Empty;

    private static string GetModel()
    {
        try
        {
            var data = Shell.ExecuteCommand("wmic" ,"computersystem get model", out var output, out var error);
            return output.Replace("\r", "").Replace("\n", "").Replace("Model", "").Trim();
        }
        catch (Exception ex)
        {
            QLog.Warning(ex, "wmic computersystem get model");
        }

        return string.Empty;
    }

    private static string GetDeviceName()
    {
        try
        {
            var data = Shell.ExecuteCommand("wmic", "computersystem get name", out var output, out var error);
            return output.Replace("\r", "").Replace("\n", "").Replace("Name", "").Trim();
        }
        catch (Exception ex)
        {
            QLog.Warning(ex, "wmic computersystem get name");
        }

        return string.Empty;
    }

    private static string GetDeviceId()
    {
        try
        {
            var data = Shell.ExecuteCommand("wmic", "csproduct get uuid", out var output, out var error);
            output = output.Replace("\r", "").Replace("\n", "").Replace("UUID", "").Trim();
            if (IsValidId(output)) 
                return output;

        }
        catch (Exception ex)
        {
            QLog.Warning(ex, "mic computersystem get uuid");
        }

        return string.Empty;
    }

    private static bool GetIsEmulator() => false;
    
    public static string QueryDeviceOs()
        => nameof(OperatingSystem.Windows);

    public static string QueryDeviceOsVersion()
        => FallbackQueryDeviceOsVersion();
    
}
