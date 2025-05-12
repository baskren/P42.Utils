using Windows.ApplicationModel;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class AppInfo
{
    /// <summary>
    /// App Identifier
    /// </summary>
    public static string Identifier 
        => Package.Current.Id.Name; // BrowserWasm: returns DisplayName, not app identifier

#if BROWSERWASM
    public static string Name => Package.Current.Id.Name;
#else
    /// <summary>
    /// App Name
    /// </summary>
    public static string Name => Package.Current.DisplayName;
#endif

    /// <summary>
    /// App Version number
    /// </summary>
    public static string Version => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}"; // Doesn't work with BrowserWasm, iOS

    /// <summary>
    /// App Build Number
    /// </summary>
    public static int Build => Package.Current.Id.Version.Revision;  // Doesn't work with BrowserWasm, iOS


}
