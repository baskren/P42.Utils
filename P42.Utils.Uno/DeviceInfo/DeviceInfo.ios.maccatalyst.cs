#if __IOS__ || __MACCATALYST__
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using P42.Serilog.QuickLog;
using UIKit;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string GetManufacturer() =>
        (new string?[]
        {
            EasDeviceInfo.SystemManufacturer,
            "Apple inc"
        })
        .FirstNotNullOrWhiteSpace();


    private static string GetModel()
    {
#if MACCATALYST
        if (MacOsHardwareOverview is null)
            return string.Empty;

        var items = new List<string>();
        if (MacOsHardwareOverview.TryGetValue("machine_name", out var machineName))
            items.Add(machineName);

        if (MacOsHardwareOverview.TryGetValue("machine_model", out var machineModel))
            items.Add(machineModel);

        if (MacOsHardwareOverview.TryGetValue("model_number", out var modelNumber))
            items.Add(modelNumber);

        return string.Join(":", items);
#endif

        return (new string?[]
            {
                GetSystemLibraryProperty("hw.model"),
                EasDeviceInfo.SystemProductName,
                UIDevice.CurrentDevice.Model,
            }).FirstNotNullOrWhiteSpace();

    }

    private static string GetDeviceName()
    {
#if MACCATALYST
            try
            {
                return ExecuteCommand("scutil --get ComputerName");
            }
            catch (Exception e)
            {
                QLog.Warning(e, "scutil --get ComputerName");
            }
#endif

        return UIDevice.CurrentDevice.Name;
    }

    private static string GetDeviceId()
    {
        var id = UIKit.UIDevice.CurrentDevice.IdentifierForVendor?.AsString();
        if (IsValidId(id))
            return id!;

#if MACCATALYST
        if (MacOsHardwareOverview is not null)
        {
            if (MacOsHardwareOverview.TryGetValue("platform_UUID", out var uuid) && IsValidId(uuid))
                return uuid;

            if (MacOsHardwareOverview.TryGetValue("provisioning_UDID", out var udid) && IsValidId(udid))
                return udid;

            if (MacOsHardwareOverview.TryGetValue("serial_number", out var serialNumber) && IsValidId(serialNumber))
                return serialNumber;

        }
#endif

        return string.Empty;
    }


    private static bool GetIsEmulator()
    #if __MACCATALYST__
        => false;
    #else
        => ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR;
    #endif
    

    private static string? GetSystemLibraryProperty(string property)
    {
        try
        {
            var lengthPtr = Marshal.AllocHGlobal(sizeof(int));
            SysctlByName(property, IntPtr.Zero, lengthPtr, IntPtr.Zero, 0);

            var propertyLength = Marshal.ReadInt32(lengthPtr);

            if (propertyLength == 0)
                return null;

            var valuePtr = Marshal.AllocHGlobal(propertyLength);
            SysctlByName(property, valuePtr, lengthPtr, IntPtr.Zero, 0);

            var returnValue = Marshal.PtrToStringAnsi(valuePtr);

            Marshal.FreeHGlobal(lengthPtr);
            Marshal.FreeHGlobal(valuePtr);

            return returnValue;
        }
        catch (Exception)
        {
        }
        return null;
    }

    [DllImport(ObjCRuntime.Constants.SystemLibrary, EntryPoint = "sysctlbyname")]
    private static extern int SysctlByName([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);


}
#endif
