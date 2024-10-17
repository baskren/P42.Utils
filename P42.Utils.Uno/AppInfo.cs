using System;
using Windows.ApplicationModel;

namespace P42.Utils.Uno;

public static class AppInfo
{
    /// <summary>
    /// TODO: Some of this information is available via assembly attributes.  It may be possible to implement a custom
    /// assembly attribute that takes the UNO build properties and sets these automatically.  In the mean time, these
    /// are going to be obsolited
    /// 
    /// </summary>
    
    [Obsolete]
    public static string Version => Package.Current.Id.Version.Major + "." + Package.Current.Id.Version.Minor; // + "." + Package.Current.Id.Version.Revision;

    [Obsolete]
    public static int Build => Package.Current.Id.Version.Build;

    [Obsolete]
    public static string Identifier => Package.Current.Id.ResourceId;

    [Obsolete]
    public static string Name => Package.Current.DisplayName;
}
