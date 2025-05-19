using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using P42.Serilog.QuickLog;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    // TODO: Do we need to differentiate between Desktop.MacOS, Desktop.Windows, Desktop.Linux, etc.?
    //private static string systemProductName;

    static Dictionary<string, string>? _macOsHardwareOverview;
    private static Dictionary<string,string>? MacOsHardwareOverview
    {
        get
        {
            if (_macOsHardwareOverview is not null)
                return _macOsHardwareOverview;
            try
            {
                var json = ExecuteCommand("system_profiler -json SPHardwareDataType");
                var obj = JsonSerializer.Deserialize<Dictionary<string, object>>(json)["spHardwareDataType"] as IList;
                json = JsonSerializer.Serialize(obj[0]);
                return _macOsHardwareOverview = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            catch (Exception ex)
            {
                QLog.Warning(ex);
            }
            return _macOsHardwareOverview = new ();
        }
    }

    static EasClientDeviceInformation? _easDeviceInfo;
    private static EasClientDeviceInformation EasDeviceInfo => _easDeviceInfo ??= new EasClientDeviceInformation ();


    /// <summary>
    /// What platform is the device running on (iOS, Android, Windows, WASM, Desktop, etc.)
    /// </summary>
    //public static string Platform => Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily.Replace($".{DeviceForm}", "");
    //public static string Platform

    #region Manufacturer
    /// <summary>
    /// Device Manufacturer
    /// </summary>
    static string? _make;
    public static string Make => _make ??= GetManufacturer();

    #endregion


    #region Model
    /// <summary>
    /// Device model
    /// </summary>
    static string? _model;
    public static string Model => _model ??= GetModel();
    
    #endregion


    #region DeviceName
    public static string? _name;
    public static string DeviceName => _name ??= GetDeviceName();
    #endregion


    #region DeviceId

    static string _id = string.Empty;
    public static string DeviceId
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_id)) return _id;

            _id = GetDeviceId();
            if (IsValidId(_id)) return _id;

            _id = EasDeviceInfo.Id.ToString();
            if (IsValidId(_id)) return _id;

            return _id = GetGeneratedDeviceId();
        }
    }

    static string GetGeneratedDeviceId()
    {
        var item = LocalData.TagItem.Get(nameof(DeviceId), typeof(DeviceInfo).FullName!, typeof(DeviceInfo).Assembly);
        if (!item.TryRecallText(out var guid))
        {
            guid = Guid.NewGuid().ToString();
            item.StoreText(guid);
        }
        return guid;
    }

    static bool IsValidId(string? id)
        => !string.IsNullOrWhiteSpace(id) && id.Any(c => c != '0' && c != '-');

    #endregion


    #region DeviceForm
    static FormFactor _deviceForm = FormFactor.NotSet;
    /// <summary>
    /// Form factor (Desktop, Tablet, Phone, etc.)
    /// </summary>
    public static FormFactor DeviceForm
    {
        get
        {
#if MACCATALYST
            return FormFactor.Desktop;
#else
            // ✔️ Browser.iOS : Safari (desktop on iPad Pro, tablet on iPad Mini, mobile on iPhone)
            // ✔️ Browser.MacOS : Safari
            // ✔️ Browser.MacOS : Chrome
            // ✔️ Browser.MacOS : Edge
            // ✔️ Browser.MacOS : Firefox
            // ✔️ Browser.MacOS : Brave
            // ✔️ Browser.Windows : Chrome
            // ✔️ Browser.Windows : Edge
            // ✔️ Browser.Windows : Firefox

            // ✔️ Desktop.MacOS
            // ☐ Desktop.Linux
            // ✔️ Desktop.Windows 

            // ✔️ Android
            // ✔️ Browser
            // ☐ FreeBSD
            // ✔️ Desktop
            // ✔️ iOS
            // ☐ Linux
            // ✖️ MacCatalyst (tablet when iPad Pro says desktop)
            // ☐ MacOS
            // ☐ TvOS
            // ☐ Wasi
            // ☐ Linux
            // ✔️ Windows

            if (_deviceForm != FormFactor.NotSet)
                return _deviceForm;

            var form = Windows.System.Profile.AnalyticsInfo.DeviceForm;
            if (Enum.TryParse<FormFactor>(form, true, out var factor))
                return _deviceForm = factor;

            return _deviceForm = form switch
            {
                "Xbox" => FormFactor.GameConsole,
                "Holographic" 
                or "HoloLens" => FormFactor.VirtualReality,
                "SurfaceHub" => FormFactor.DigitalWhiteboard,
                "Tv" => FormFactor.Television,
                "Phone" => FormFactor.Mobile,
                "Universal" 
                or "Notebook" 
                or "Convertible" 
                or "Detachable" 
                or "All-in-One" 
                or "Stick PC" 
                or "Puck" => FormFactor.Desktop,
                _ => FormFactor.Unknown,
            };
            

#endif
        }
    }
    #endregion


    #region Os
    /// <summary>
    /// Common name for OsVersion
    /// </summary>
    //public static Version OsVersion => Windows.System.Profile.AnalyticsInfo.VersionInfo.ParseDeviceFamilyVersion();
    static string _os = string.Empty;
    public static string Os
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_os))
                return _os;

#if BROWSERWASM
            _os = "Browser";

            if (BrowserOverview is null)
                return _os;

            if (BrowserOverview.TryGetValue("os", out var os) && os.TryGetValue("name", out var osName))
                _os += $":[{osName}]";

            if (BrowserOverview.TryGetValue("os", out var browser) && browser.TryGetValue("name", out var browserName))
                _os += $" {browserName}";

            if (BrowserOverview.TryGetValue("engine", out var engine) && engine.TryGetValue("name", out var engineName))
                _os += $" ({engineName})";

            return _os;
#else

#if DESKTOP
            _os = "Desktop:";
#endif
            return _os += QueryOs();
#endif
        }
    }

    static string QueryOs()
    {
        if (System.OperatingSystem.IsAndroid())
            return _os += OperatingSystem.Android.ToString();
        if (System.OperatingSystem.IsBrowser())
            return _os += OperatingSystem.Browser.ToString();
        if (System.OperatingSystem.IsFreeBSD())
            return _os += OperatingSystem.FreeBSD.ToString();
        if (System.OperatingSystem.IsIOS())
            return _os += OperatingSystem.iOS.ToString();
        if (System.OperatingSystem.IsLinux())
            return _os += OperatingSystem.Linux.ToString();
        if (System.OperatingSystem.IsMacCatalyst())
            return _os += OperatingSystem.MacCatalyst.ToString();
        if (System.OperatingSystem.IsMacOS())
            return _os += OperatingSystem.MacOS.ToString();
        if (System.OperatingSystem.IsTvOS())
            return _os += OperatingSystem.TvOS.ToString();
        if (System.OperatingSystem.IsWasi())
            return _os += OperatingSystem.Wasi.ToString();
        if (System.OperatingSystem.IsWatchOS())
            return (_os = OperatingSystem.WatchOS.ToString());
        if (System.OperatingSystem.IsWindows())
            return _os += OperatingSystem.Windows.ToString();
        QLog.Warning("Unknown OS");
        return _os += OperatingSystem.Unknown.ToString();

    }
    #endregion


    #region OsVersion
    static string _osVersion = string.Empty;
    public static string OsVersion
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_osVersion)) return _osVersion;

#if BROWSERWASM
            if (BrowserOverview is null)
                return "n/a";

            if (BrowserOverview.TryGetValue("os", out var os) && os.TryGetValue("version", out var osVersion))
                _osVersion += $"[{osVersion}]";

            if (BrowserOverview.TryGetValue("os", out var browser) && browser.TryGetValue("version", out var browserVersion))
                _osVersion += $" {browserVersion}";

            if (BrowserOverview.TryGetValue("engine", out var engine) && engine.TryGetValue("version", out var engineVersion))
                _osVersion += $" ({engineVersion})";

            return _osVersion;

#elif ANDROID
            return _osVersion = AnalyticsInfo.VersionInfo.ParseDeviceFamilyVersion().ToString();
#else
            var v1 = System.Environment.OSVersion.Version.Major;
            var v2 = System.Environment.OSVersion.Version.Minor;
            var v3 = Math.Max(System.Environment.OSVersion.Version.Build, 0);
            var v4 = Math.Max(System.Environment.OSVersion.Version.Revision, 0);
            return _osVersion = $"{v1}.{v2}.{v3}.{v4}";

#endif
        }
    }
    #endregion


    #region OsDescription
    static string _osDescription = string.Empty;
    public static string OsDescription
    {
        get
        {
            if (!string.IsNullOrWhiteSpace (_osDescription)) return _osDescription;

#if BROWSERWASM
            return _osDescription = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
;
#else
            return _osDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
#endif
        }
    }
    #endregion


    /// <summary>
    /// Is this device an emulator?
    /// </summary>
    public static bool IsEmulator => GetIsEmulator();

    public static string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;

    public static string FrameworkDescription => RuntimeInformation.FrameworkDescription;



    static string ExecuteCommand(string command)
    {
        // Create a new process
        using (var process = new System.Diagnostics.Process())
        {
            // Configure the process
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Start the process
            process.Start();

            // Capture output
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                QLog.Warning($"ExecuteCommand({command}), Error {error}");
                return string.Empty;
            }

            return output;
        }
    }

}
