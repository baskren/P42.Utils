using P42.Utils.Uno.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;

namespace P42.Utils.Uno
{
    public class Device
    {
        public static DeviceIdiom Idiom
        {
            get
            {
#if __WASM__ || NETSTANDARD
                return DeviceIdiom.Web;
#elif __MACOS__
                return DeviceIdiom.Desktop;
#else
                //return Xamarin.Essentials.DeviceInfo.Idiom.ToUno();
                System.Diagnostics.Debug.WriteLine("DeviceForm: " + AnalyticsInfo.DeviceForm);
                System.Diagnostics.Debug.WriteLine("DeviceFamily: " + AnalyticsInfo.VersionInfo.DeviceFamily);
                switch (AnalyticsInfo.DeviceForm.ToLower())
                {
                    case "desktop":
                        return DeviceIdiom.Desktop;
                    case "mobile":
                    case "phone":
                        return DeviceIdiom.Phone;
                    case "tablet":
                        return DeviceIdiom.Tablet;
                    case "gameconsole":
                        return DeviceIdiom.GameConsole;
                    case "watch":
                        return DeviceIdiom.Watch;
                    case "car":
                        return DeviceIdiom.Car;
                    case "television":
                    case "tv":
                        return DeviceIdiom.TV;
                    case "virtualreality":
                    case "vr":
                        return DeviceIdiom.VR;
                    default:
                        return DeviceIdiom.Unknown;
                }
#endif
            }
        }

        public static Platform Platform
        {
            get
            {
#if NETFX_CORE
                return Platform.UWP;
#elif __ANDROID__
                return Platform.Android;
#elif __IOS__
                return Platform.iOS;
#elif NETSTANDARD
                return Platform.WASM;
#elif __MACOS__
                return Platform.MacOS;
#elif __SKIA__
                return Platform.Skia;
#else
                thrown new Execption();
#endif
            }
        }


    }
}
