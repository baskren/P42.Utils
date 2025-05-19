#if DESKTOP
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

    private static string GetManufacturer()
    {
        if (System.OperatingSystem.IsMacOS())
            return "Apple inc.";

        if (System.OperatingSystem.IsWindows())
        {
            try
            {
                var data = ExecuteCommand("wmic computersystem get manufacturer");
                return data.Replace("\r", "").Replace("\n", "").Replace("Manufacturer", "").Trim();
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "wmic computersystem get manufacturer");
            }
        }

        if (System.OperatingSystem.IsLinux())
        {
            try
            {
                return ExecuteCommand("cat /sys/class/dmi/id/sys_vendor");
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "cat /sys/class/dmi/id/sys_vendor");
            }
        }

        return EasDeviceInfo.SystemManufacturer;
    }
     

    private static string GetModel()
    {
        if (System.OperatingSystem.IsMacOS() || System.OperatingSystem.IsMacCatalyst())
        {
            if (MacOsHardwareOverview is null)
                return EasDeviceInfo.SystemProductName;

            var items = new List<string>();
            if (MacOsHardwareOverview.TryGetValue("machine_name", out var machineName))
                items.Add(machineName);

            if (MacOsHardwareOverview.TryGetValue("machine_model", out var machineModel))
                items.Add(machineModel);

            if (MacOsHardwareOverview.TryGetValue("machine_model", out var modelNumber))
                items.Add(modelNumber);

            return (items.Count == 0)
                ? EasDeviceInfo.SystemProductName
                : string.Join(":", items);
        }


        if (System.OperatingSystem.IsWindows())
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
        }

        if (System.OperatingSystem.IsLinux())
        {
            try
            {
                return ExecuteCommand("cat /sys/class/dmi/id/product_name");
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "cat /sys/class/dmi/id/product_name");
            }
        }


        return EasDeviceInfo.SystemProductName;
    }


    private static string GetDeviceName()
    {
        // ☑ Desktop.MacOS
        // ☐ Desktop.Linux
        // ☑ Desktop.Windows 

        if (System.OperatingSystem.IsMacOS())
        {
            try
            {
                return ExecuteCommand("scutil --get ComputerName");
            }
            catch (Exception e)
            {
                QLog.Warning(e, "scutil --get ComputerName");
            }

        }

        if (System.OperatingSystem.IsWindows())
        {
            try
            {
                var data = ExecuteCommand("wmic computersystem get name");
                return data.Replace("\r", "").Replace("\n", "").Replace("Name", "").Trim();
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "mic computersystem get name");
            }
        }

        if (System.OperatingSystem.IsLinux())
        {
            try
            {
                return ExecuteCommand("uname -n");
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "uname -n");
            }
        }

        return EasDeviceInfo.FriendlyName;
    }


    private static string GetDeviceId()
    {
        if (MacOsHardwareOverview is not null &&
            (System.OperatingSystem.IsMacOS() || System.OperatingSystem.IsMacCatalyst()))
        {
            if (MacOsHardwareOverview.TryGetValue("platform_UUID", out var uuid) && IsValidId(uuid))
                return uuid;

            if (MacOsHardwareOverview.TryGetValue("provisioning_UDID", out var udid) && IsValidId(udid))
                return udid;

            if (MacOsHardwareOverview.TryGetValue("serial_number", out var serialNumber) && IsValidId(serialNumber))
                return serialNumber;

        }

        if (System.OperatingSystem.IsWindows())
        {
            try
            {
                var data = ExecuteCommand("wmic csproduct get uuid");
                return data.Replace("\r", "").Replace("\n", "").Replace("UUID", "").Trim();
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "mic computersystem get uuid");
            }
        }

        if (System.OperatingSystem.IsLinux())
        {
            try
            {
                return ExecuteCommand("cat /sys/class/dmi/id/product_uuid");
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "cat /sys/class/dmi/id/product_uuid");
            }
        }

        return string.Empty;

    }



    private static bool GetIsEmulator() => false;
}
#endif
