using System.Runtime.InteropServices;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;


namespace P42.Utils.Uno;

public static partial class DeviceInfo
{


    private static EasClientDeviceInformation EasDeviceInfo => field ??= new EasClientDeviceInformation ();

    
    #region Manufacturer

    public static string Make
    {
        get
        {
            if (!string.IsNullOrEmpty (field))
                return field;

            var make = GetManufacturer();
            if (!string.IsNullOrEmpty (make))
                return field = make;

            make = EasDeviceInfo.SystemManufacturer;
            if (!string.IsNullOrEmpty (make))
                return field = make;

            return field = "Unknown";
        }
    }

    #endregion


    #region Model

    public static string Model
    {
        get
        {
            if (!string.IsNullOrEmpty(field))
                return field;
            
            var model = GetModel();
            if (!string.IsNullOrEmpty(model))
                return field = model;
            
            try
            {
                model = EasDeviceInfo.SystemProductName;
                if (!string.IsNullOrEmpty(model))
                    return field = model;
            }
            catch (Exception)
            {
                // Ignore
            }

            return field = "Unknown";
        }
    }
    
    #endregion


    #region DeviceName

    public static string DeviceName
    {
        get
        {
            if (!string.IsNullOrEmpty(field))
                return field;

            var name = GetDeviceName();
            if (!string.IsNullOrEmpty(name))
                return field = name;
            
            try
            {
#pragma warning disable Uno0001
                name = EasDeviceInfo.FriendlyName;
#pragma warning restore Uno0001
                if (!string.IsNullOrEmpty(name))
                    return field = name;
            }
            catch (Exception)
            {
                // Ignore
            }

            return field = "Unknown";

        }
    }
    #endregion


    #region DeviceId

    public static string DeviceId
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(field)) return field;

            field = GetDeviceId();
            if (IsValidId(field)) return field;

            return field = FallbackId();
        }
    } = string.Empty;

    private static string GetGeneratedDeviceId()
    {
        var item = LocalData.TagItem.For(nameof(DeviceId), typeof(DeviceInfo).FullName!, typeof(DeviceInfo).Assembly);
        if (item.TryRecallText(out var guid) && !string.IsNullOrEmpty(guid))
            return guid;

        guid = Guid.NewGuid().ToString();
        item.StoreText(guid);
        return guid;
    }

    private static bool IsValidId(string? id)
        => !string.IsNullOrWhiteSpace(id) && id.Any(c => c != '0' && c != '-');

    private static string FallbackId()
    {
        try
        {
#pragma warning disable Uno0001
            if (EasDeviceInfo.Id != Guid.Empty)
            {
                var id = EasDeviceInfo.Id.ToString();
                if (IsValidId(id)) return id;
            }
#pragma warning restore Uno0001
        }
        catch (Exception)
        {
            // Ignore
        }
        
        return GetGeneratedDeviceId();
        
    }
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

    public static string Os
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(field))
                return field;

            return field = QueryDeviceOs();
        }
    } = string.Empty;

    public static string UiPlatform
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

    public static string FallbackQueryDeviceOs()
    {
        if (OperatingSystem.IsAndroid())
            return nameof(DeviceOperatingSystem.Android);
        if (OperatingSystem.IsBrowser())
            return nameof(DeviceOperatingSystem.Browser);
        if (OperatingSystem.IsFreeBSD())
            return nameof(DeviceOperatingSystem.FreeBSD);
        if (OperatingSystem.IsIOS())
            return nameof(DeviceOperatingSystem.iOS);
        if (OperatingSystem.IsLinux())
            return nameof(DeviceOperatingSystem.Linux);
        if (OperatingSystem.IsMacCatalyst())
            return nameof(DeviceOperatingSystem.MacCatalyst);
        if (OperatingSystem.IsMacOS())
            return nameof(DeviceOperatingSystem.MacOS);
        if (OperatingSystem.IsTvOS())
            return nameof(DeviceOperatingSystem.TvOS);
        if (OperatingSystem.IsWasi())
            return nameof(DeviceOperatingSystem.Wasi);
        if (OperatingSystem.IsWatchOS())
            return nameof(DeviceOperatingSystem.WatchOS);
        if (OperatingSystem.IsWindows())
            return nameof(DeviceOperatingSystem.Windows);
        Serilog.QuickLog.QLog.Warning("Unknown OS");
        return nameof(DeviceOperatingSystem.Unknown);

    }
    #endregion


    #region OsVersion

    public static string OsVersion
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(field)) 
                return field;

            return field = QueryDeviceOsVersion();
        }
    } = string.Empty;

    public static string FallbackQueryDeviceOsVersion()
    {
        var v1 = Environment.OSVersion.Version.Major;
        var v2 = Environment.OSVersion.Version.Minor;
        var v3 = Math.Max(Environment.OSVersion.Version.Build, 0);
        var v4 = Math.Max(Environment.OSVersion.Version.Revision, 0);
        return $"{v1}.{v2}.{v3}.{v4}";
    }

    #endregion


    #region OsDescription

    public static string OsDescription
    {
        get
        {
            if (!string.IsNullOrWhiteSpace (field)) 
                return field;

            if (OperatingSystem.IsBrowser())
                return field = AnalyticsInfo.VersionInfo.DeviceFamily; 
                
            return field = RuntimeInformation.OSDescription;
        }
    } = string.Empty;

    #endregion


    /// <summary>
    /// Is this device an emulator?
    /// </summary>
    public static bool IsEmulator => GetIsEmulator();

    public static string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;

    public static string FrameworkDescription => RuntimeInformation.FrameworkDescription;

    
}
