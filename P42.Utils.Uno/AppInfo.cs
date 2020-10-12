using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace P42.Utils.Uno
{
    public static class AppInfo
    {
        public static string Version => Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor; // + "." + Package.Current.Id.Version.Revision;

        public static int Build => Package.Current.Id.Version.Build;

        public static string Identifier => Package.Current.Id.ResourceId;

        public static string Name => Package.Current.DisplayName;
    }
}
