using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text.Json;
using P42.Serilog.QuickLog;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;


namespace P42.Utils.Uno;

public static partial class DeviceInfo
{

    static Dictionary<string, string>? _macOsHardwareOverview;
    // ReSharper disable once UnusedMember.Local
    private static Dictionary<string,string>? MacOsHardwareOverview
    {
        #if BROWSERWASM
        [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
        #endif
        get
        {
            if (_macOsHardwareOverview is not null)
                return _macOsHardwareOverview;
            try
            {
                var json = ExecuteCommand("system_profiler -json SPHardwareDataType");
                var obj = JsonSerializer.Deserialize<Dictionary<string, object>>(json)?["spHardwareDataType"];
                if (obj is not IList { Count: > 0 } iList)
                    return _macOsHardwareOverview = new();

                json = JsonSerializer.Serialize(iList[0]);
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
    private static string? _name;
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

    private static string GetGeneratedDeviceId()
    {
        var item = LocalData.TagItem.Get(nameof(DeviceId), typeof(DeviceInfo).FullName!, typeof(DeviceInfo).Assembly);
        if (item.TryRecallText(out var guid) && !string.IsNullOrEmpty(guid))
            return guid;

        guid = Guid.NewGuid().ToString();
        item.StoreText(guid);
        return guid;
    }

    private static bool IsValidId(string? id)
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

            var form =AnalyticsInfo.DeviceForm;
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
                _ => FormFactor.Unknown
            };
            
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
            return _os = QueryOs();
#endif
        }
    }

    public static string Platform
    {
        get
        {
#if __DESKTOP__
            return $"Desktop.{Os}";
#elif __UNO_SKIA__
            return $"Skia.{Os}";
#else
            return Os;
#endif
        }
    }

    public static string QueryOs()
    {
        if (System.OperatingSystem.IsAndroid())
            return nameof(OperatingSystem.Android);
        if (System.OperatingSystem.IsBrowser())
            return nameof(OperatingSystem.Browser);
        if (System.OperatingSystem.IsFreeBSD())
            return nameof(OperatingSystem.FreeBSD);
        if (System.OperatingSystem.IsIOS())
            return nameof(OperatingSystem.iOS);
        if (System.OperatingSystem.IsLinux())
            return nameof(OperatingSystem.Linux);
        if (System.OperatingSystem.IsMacCatalyst())
            return nameof(OperatingSystem.MacCatalyst);
        if (System.OperatingSystem.IsMacOS())
            return nameof(OperatingSystem.MacOS);
        if (System.OperatingSystem.IsTvOS())
            return nameof(OperatingSystem.TvOS);
        if (System.OperatingSystem.IsWasi())
            return nameof(OperatingSystem.Wasi);
        if (System.OperatingSystem.IsWatchOS())
            return nameof(OperatingSystem.WatchOS);
        if (System.OperatingSystem.IsWindows())
            return nameof(OperatingSystem.Windows);
        QLog.Warning("Unknown OS");
        return nameof(OperatingSystem.Unknown);

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
            var v1 = Environment.OSVersion.Version.Major;
            var v2 = Environment.OSVersion.Version.Minor;
            var v3 = Math.Max(Environment.OSVersion.Version.Build, 0);
            var v4 = Math.Max(Environment.OSVersion.Version.Revision, 0);
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
            return _osDescription = AnalyticsInfo.VersionInfo.DeviceFamilyVersion; 
#else
            return _osDescription = RuntimeInformation.OSDescription;
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



    private static string ExecuteCommand(string command)
    {
        // Create a new process
        using var process = new System.Diagnostics.Process();
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
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode == 0)
            return output;

        QLog.Warning($"ExecuteCommand({command}), Error {error}");
        return string.Empty;

    }

}
