using System.Text.Json.Serialization;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    // ReSharper disable InconsistentNaming
    [JsonSerializable(typeof(Dictionary<string, List<Dictionary<string, string>>>))]
    internal partial class Dictionary_string_List_Dictionary_string_string_SerializerContext : JsonSerializerContext;

    private static readonly Serializer Serializer = new ();
    
    static DeviceInfo()
    {
        Serializer.Add(typeof(Dictionary<string, List<Dictionary<string, string>>>), Dictionary_string_List_Dictionary_string_string_SerializerContext.Default);
    }
    
    
    private static Dictionary<string, string>? _macOsHardwareOverview;
    // ReSharper disable once UnusedMember.Local
    private static Dictionary<string,string> MacOsHardwareOverview
    {
        get
        {
            if (_macOsHardwareOverview is not null)
                return _macOsHardwareOverview;
            try
            {
                var json = Platform.ExecuteShellCommand("system_profiler -json SPHardwareDataType");
                if (!Serializer.TryDeserialize<Dictionary<string, List<Dictionary<string, string>>>>(json, out var dict)
                    || dict["spHardwareDataType"] is not { Count: > 0 } list)
                    return _macOsHardwareOverview = new Dictionary<string, string>();

                return _macOsHardwareOverview = list[0];
            }
            catch (Exception ex)
            {
                QLog.Warning(ex);
            }
            return _macOsHardwareOverview = new Dictionary<string, string>();
        }
    }

    
    private static string GetManufacturer()
    {
        if (OperatingSystem.IsMacOS())
            return "Apple inc.";

        if (OperatingSystem.IsWindows())
        {
            try
            {
                // ReSharper disable once StringLiteralTypo
                var data = Platform.ExecuteShellCommand("wmic computersystem get manufacturer");
                return data.Replace("\r", "").Replace("\n", "").Replace("Manufacturer", "").Trim();
            }
            catch (Exception ex)
            {
                // ReSharper disable once StringLiteralTypo
                QLog.Warning(ex, "wmic computersystem get manufacturer");
            }
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                return Platform.ExecuteShellCommand("cat /sys/class/dmi/id/sys_vendor");
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "cat /sys/class/dmi/id/sys_vendor");
            }
        }

        return string.Empty;
    }
     

    private static string GetModel()
    {
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            var items = new List<string>();
            if (MacOsHardwareOverview.TryGetValue("machine_name", out var machineName))
                items.Add(machineName);

            if (MacOsHardwareOverview.TryGetValue("machine_model", out var machineModel))
                items.Add(machineModel);

            if (MacOsHardwareOverview.TryGetValue("machine_model", out var modelNumber))
                items.Add(modelNumber);

            return items.Count == 0
                ? EasDeviceInfo.SystemProductName
                : string.Join(":", items);
        }


        if (OperatingSystem.IsWindows())
        {
            try
            {
                var data = Platform.ExecuteShellCommand("wmic computersystem get model");
                return data.Replace("\r", "").Replace("\n", "").Replace("Model", "").Trim();
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "wmic computersystem get model");
            }
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                return Platform.ExecuteShellCommand("cat /sys/class/dmi/id/product_name");
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "cat /sys/class/dmi/id/product_name");
            }
        }


        return string.Empty;
    }


    private static string GetDeviceName()
    {
        // ☑ Desktop.MacOS
        // ☐ Desktop.Linux
        // ☑ Desktop.Windows 

        if (OperatingSystem.IsMacOS())
        {
            try
            {
                return Platform.ExecuteShellCommand("scutil --get ComputerName");
            }
            catch (Exception e)
            {
                QLog.Warning(e, "scutil --get ComputerName");
            }

        }

        if (OperatingSystem.IsWindows())
        {
            try
            {
                var data = Platform.ExecuteShellCommand("wmic computersystem get name");
                return data.Replace("\r", "").Replace("\n", "").Replace("Name", "").Trim();
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "mic computersystem get name");
            }
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                return Platform.ExecuteShellCommand("uname -n");
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "uname -n");
            }
        }

        return string.Empty;
    }


    private static string GetDeviceId()
    {
        if (OperatingSystem.IsMacOS() || OperatingSystem.IsMacCatalyst())
        {
            if (MacOsHardwareOverview.TryGetValue("platform_UUID", out var uuid) && IsValidId(uuid))
                return uuid;

            if (MacOsHardwareOverview.TryGetValue("provisioning_UDID", out var udid) && IsValidId(udid))
                return udid;

            if (MacOsHardwareOverview.TryGetValue("serial_number", out var serialNumber) && IsValidId(serialNumber))
                return serialNumber;

        }

        if (OperatingSystem.IsWindows())
        {
            try
            {
                var data = Platform.ExecuteShellCommand("wmic csproduct get uuid");
                data = data.Replace("\r", "").Replace("\n", "").Replace("UUID", "").Trim();
                if (IsValidId(data))
                    return data;
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "mic computersystem get uuid");
            }
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                // This requires "sudo"
                //var data = Platform.ExecuteCommand("cat /sys/class/dmi/id/product_uuid");
                var data = Platform.ExecuteShellCommand("cat /etc/machine-id");
                data = data.Replace("\r", "").Replace("\n", "").Replace("UUID", "").Trim();
                if (IsValidId(data))
                    return data;
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, "cat /etc/machine-id");
            }
        }

        return string.Empty; 

    }



    private static bool GetIsEmulator() => false;

    public static string QueryDeviceOs()
        => FallbackQueryDeviceOs();

    public static string QueryDeviceOsVersion()
        => FallbackQueryDeviceOsVersion();
    
}

