using System;
using System.Reflection;

namespace P42.Utils;

public static class Environment
{

    internal static IPlatformTimer? PlatformTimer { get; set; }
    internal static Type? PlatformLongRunningTaskType { get; set; }

    /// <summary>
    /// Initialization - must be called from app's MainThread
    /// </summary>
    public static void Init()
    {
        MainThreadId = System.Environment.CurrentManagedThreadId;
    }

    /// <summary>
    /// Gets or sets the main thread identifier.
    /// </summary>
    /// <value>The main thread identifier.</value>
    public static int MainThreadId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:P42.Utils.Environment"/> is headless test (and thus the MainThreadId is questionable).
    /// </summary>
    /// <value><c>true</c> if is headless test; otherwise, <c>false</c>.</value>
    public static bool IsHeadlessTest { get; set; } = false;

    /// <summary>
    /// Gets a value indicating whether this <see cref="T:P42.Utils.Environment"/> is on main thread.
    /// </summary>
    /// <value><c>true</c> if is on main thread; otherwise, <c>false</c>.</value>
    public static bool IsOnMainThread => IsHeadlessTest || System.Environment.CurrentManagedThreadId == MainThreadId;

    //TODO: MOVE THIS FROM P42.Utils.Uno 
    public static Func<string, Assembly?, Assembly?>? EmbeddedResourceAssemblyResolver { get; set; }

    /// <summary>
    /// Gets the operating system.
    /// </summary>
    /// <returns>The operating system.</returns>
    public static string GetOperatingSystem()
    {
        var winDir = System.Environment.GetEnvironmentVariable("windir");
        if (!string.IsNullOrEmpty(winDir) && winDir.Contains('\\') && System.IO.Directory.Exists(winDir))
            return "Windows";
        
        if (System.IO.File.Exists("/proc/sys/kernel/ostype"))
        {
            var osType = System.IO.File.ReadAllText("/proc/sys/kernel/ostype");
            if (osType.StartsWith("Linux", StringComparison.OrdinalIgnoreCase))
                // Note: Android gets here too
                return "Linux";

            throw new UnsupportedPlatformException(osType);
        }
        
        if (System.IO.File.Exists("/System/Library/CoreServices/SystemVersion.plist"))
            // Note: iOS gets here too
            return "Apple";

        throw new UnsupportedPlatformException();
        
    }

    /// <summary>
    /// Platform specific loader of application paths
    /// </summary>
    public static Action? PlatformPathLoader { get; set; }


    static string? _applicationDataPath;
    /// <summary>
    /// Path to platform's app data path
    /// </summary>
    public static string ApplicationDataPath
    {
        get
        {
            if (_applicationDataPath == null)
                PlatformPathLoader?.Invoke();
            return _applicationDataPath ?? string.Empty;
        }
        set
        {
            _applicationDataPath = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

    static string? _applicationCachePath;
    /// <summary>
    /// Path to platforms application cache
    /// </summary>
    public static string ApplicationCachePath
    {
        get
        {
            if (_applicationCachePath == null)
                PlatformPathLoader?.Invoke();
            return _applicationCachePath ?? string.Empty;
        }
        set
        {
            _applicationCachePath = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

    static string? _temporaryStoratePath;
    /// <summary>
    /// Path to platform temporary storage
    /// </summary>
    public static string TemporaryStoragePath
    {
        get
        {
            if (_temporaryStoratePath == null)
                PlatformPathLoader?.Invoke();
            return _temporaryStoratePath ?? string.Empty;
        }
        set
        {
            _temporaryStoratePath = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

}

public class UnsupportedPlatformException : Exception
{
    public UnsupportedPlatformException() : base("Unsupported Platform Exception") { }

    public UnsupportedPlatformException(string osType) : base("Unsupported Platform Exception: " + osType) { }
}
