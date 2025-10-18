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

            const string errorTitle = "Failed to Get Mac OS Hardware Overview";
            try
            {
                if (Shell.ExecuteCommand("system_profiler", "-json SPHardwareDataType", out var json, out var error) != 0
                    || !Serializer.TryDeserialize<Dictionary<string, List<Dictionary<string, string>>>>(json, out var dict)
                    || dict["spHardwareDataType"] is not { Count: > 0 } list)
                {
                    QLog.Error(error, errorTitle);
                    return _macOsHardwareOverview = new Dictionary<string, string>();
                }
                return _macOsHardwareOverview = list[0];
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, errorTitle);
            }
            return _macOsHardwareOverview = new Dictionary<string, string>();
        }
    }

    
    private static string GetManufacturer()
    {
        if (OperatingSystem.IsMacOS())
            return "Apple inc.";

        const string errorTitle = "Cannot get device manufacturer";

        if (OperatingSystem.IsWindows())
        {
            try
            {
                if (Shell.ExecuteCommand("wmic", "computersystem get manufacturer", out var output, out var error) == 0)
                    return output.Replace("\r", "").Replace("\n", "").Replace("Manufacturer", "").Trim();
                QLog.Warning(error, errorTitle);
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, errorTitle);
            }
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                if (Shell.ExecuteCommand("cat", "/sys/class/dmi/id/sys_vendor", out var output, out var error) == 0)
                    return output;
                QLog.Warning(error, errorTitle);
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, errorTitle);
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

        const string errorTitle = "Cannot get device model";

        if (OperatingSystem.IsWindows())
        {
            try
            {
                if (Shell.ExecuteCommand("wmic", "computersystem get model", out var output, out var error) == 0)
                    return output.Replace("\r", "").Replace("\n", "").Replace("Model", "").Trim();
                QLog.Warning(error, errorTitle);
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, errorTitle);
            }
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                if (Shell.ExecuteCommand("cat", "/sys/class/dmi/id/product_name", out var output, out var error) == 0)
                    return output;
                QLog.Warning(error, errorTitle);
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, errorTitle);
            }
        }
        
        return string.Empty;
    }


    private static string GetDeviceName()
    {
        // ☑ Desktop.MacOS
        // ☐ Desktop.Linux
        // ☑ Desktop.Windows 

        const string errorTitle = "Cannot get device name";
        
        if (OperatingSystem.IsMacOS())
        {
            try
            {
                if (Shell.ExecuteCommand("scutil", "--get ComputerName", out var output, out var error ) == 0)
                    return output;
                QLog.Warning(error, errorTitle);
            }
            catch (Exception e)
            {
                QLog.Warning(e, errorTitle);
            }

        }

        if (OperatingSystem.IsWindows())
        {
            try
            {
                if (Shell.ExecuteCommand("wmic","computersystem get name", out var output, out var error) == 0)
                    return output.Replace("\r", "").Replace("\n", "").Replace("Name", "").Trim();
                QLog.Warning(error, errorTitle);
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, errorTitle);
            }
        }

        if (OperatingSystem.IsLinux())
        {
            try
            {
                if (Shell.ExecuteCommand("uname", "-n", out var output, out var error ) == 0)
                    return output;
                QLog.Warning(error, errorTitle);
            }
            catch (Exception ex)
            {
                QLog.Warning(ex, errorTitle);
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

        const string errorTitle = "Cannot get machine id";
        var cmd = "wmic";
        var args = "csproduct get uuid";

        if (OperatingSystem.IsLinux())
        {
            cmd = "cat";
            args = "/etc/machine-id";
        }
        try
        {
            if (Shell.ExecuteCommand(cmd, args, out var output, out var error) != 0)
            {
                QLog.Warning(error, errorTitle);
                return string.Empty;
            }
            output = output.Replace("\r", "").Replace("\n", "").Replace("UUID", "").Trim();
            if (IsValidId(output))
                return output;
        }
        catch (Exception ex)
        {
            QLog.Warning(ex, errorTitle);
        }
        
        return string.Empty; 
    }



    private static bool GetIsEmulator() => false;

    public static string QueryDeviceOs()
        => FallbackQueryDeviceOs();

    public static string QueryDeviceOsVersion()
        => FallbackQueryDeviceOsVersion();
    
}

