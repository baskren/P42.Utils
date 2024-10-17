using Windows.System.Profile;

namespace P42.Utils.Uno;

/// <summary>
/// Device methods
/// </summary>
public static class Device
{
    public static DeviceIdiom Idiom
    {
        get
        {
#if __WASM__
               return DeviceIdiom.Web;
#elif __MACOS__
                return DeviceIdiom.Desktop;
#else
            //System.Diagnostics.Debug.WriteLine("DeviceForm: " + AnalyticsInfo.DeviceForm);
            //System.Diagnostics.Debug.WriteLine("DeviceFamily: " + AnalyticsInfo.VersionInfo.DeviceFamily);
            return AnalyticsInfo.DeviceForm.ToLower() switch
            {
                "desktop" => DeviceIdiom.Desktop,
                "mobile" or "phone" => DeviceIdiom.Phone,
                "tablet" => DeviceIdiom.Tablet,
                "gameconsole" => DeviceIdiom.GameConsole,
                "watch" => DeviceIdiom.Watch,
                "car" => DeviceIdiom.Car,
                "television" or "tv" => DeviceIdiom.TV,
                "virtualreality" or "vr" => DeviceIdiom.VR,
                _ => DeviceIdiom.Unknown
            };
#endif
        }
    }


}
