using Windows.ApplicationModel;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class AppInfo
{
    /// <summary>
    /// App Identifier
    /// </summary>
    public static string Identifier => Package.Current.Id.ResourceId;

    /// <summary>
    /// App Name
    /// </summary>
    public static string Name => Package.Current.DisplayName;
    
    /// <summary>
    /// App Version number
    /// </summary>
    public static string Version => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}"; // + "." + Package.Current.Id.Version.Revision;

    /// <summary>
    /// App Build Number
    /// </summary>
    public static int Build => Package.Current.Id.Version.Build;

}
