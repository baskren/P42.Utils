using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Uno.Extensions
{
    public static class DeviceIdiomExtensions
    {
        public static DeviceIdiom ToUno(this Xamarin.Essentials.DeviceIdiom xIdiom)
        {
            switch(xIdiom.ToString())
            {
                case nameof(Xamarin.Essentials.DeviceIdiom.Phone): return DeviceIdiom.Phone;
                case nameof(Xamarin.Essentials.DeviceIdiom.Tablet): return DeviceIdiom.Tablet;
                case nameof(Xamarin.Essentials.DeviceIdiom.Desktop): return DeviceIdiom.Desktop;
                case nameof(Xamarin.Essentials.DeviceIdiom.TV): return DeviceIdiom.TV;
                case nameof(Xamarin.Essentials.DeviceIdiom.Watch): return DeviceIdiom.Watch;
            }
            return DeviceIdiom.Unknown;
        }

        public static Xamarin.Essentials.DeviceIdiom ToXamarin(this DeviceIdiom idiom)
        {
            switch (idiom)
            {
                case DeviceIdiom.Phone: return Xamarin.Essentials.DeviceIdiom.Phone;
                case DeviceIdiom.Tablet: return Xamarin.Essentials.DeviceIdiom.Tablet;
                case DeviceIdiom.Desktop: return Xamarin.Essentials.DeviceIdiom.Desktop;
                case DeviceIdiom.TV: return Xamarin.Essentials.DeviceIdiom.TV;
                case DeviceIdiom.Watch: return Xamarin.Essentials.DeviceIdiom.Watch;
            }
            return Xamarin.Essentials.DeviceIdiom.Unknown;
        }
    }
}
