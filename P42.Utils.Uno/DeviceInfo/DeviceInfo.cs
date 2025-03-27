using System;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    // TODO: Do we need to differentiate between Desktop.MacOS, Desktop.Windows, Desktop.Linux, etc.?

    /// <summary>
    /// What platform is the device running on (iOS, Android, Windows, WASM, Desktop, etc.)
    /// </summary>
    public static string Platform => Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Replace($".{DeviceForm}", "");
    
    /// <summary>
    /// Device Manufacturer
    /// </summary>
    public static string Manufacturer => GetManufacturer();

    /// <summary>
    /// Device model
    /// </summary>
    public static string Model => GetModel();
    
    /// <summary>
    /// Form factor (Desktop, Tablet, Phone, etc.)
    /// </summary>
    public static string DeviceForm => Windows.System.Profile.AnalyticsInfo.DeviceForm;

    /// <summary>
    /// Common name for OsVersion
    /// </summary>
    public static Version OsVersion => Windows.System.Profile.AnalyticsInfo.VersionInfo.ParseDeviceFamilyVersion();

    /// <summary>
    /// Is this device an emulator?
    /// </summary>
    public static bool IsEmulator => GetIsEmulator();

    /// <summary>
    /// Device name
    /// </summary>
    public static string Name => GetDeviceName();

    
}
