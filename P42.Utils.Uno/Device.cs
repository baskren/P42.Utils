using P42.Utils.Uno.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
#else
                return Xamarin.Essentials.DeviceInfo.Idiom.ToUno();
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
