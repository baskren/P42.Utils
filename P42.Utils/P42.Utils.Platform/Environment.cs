using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace P42.Utils;

/// <summary>
/// Platform specific initialization
/// </summary>
public static class Environment
{

    //[Obsolete("NO LONGER NEEDED", true)]
    //internal static IPlatformTimer? PlatformTimer { get; set; } 
    

    internal static readonly Assembly Assembly = typeof(Environment).Assembly;
    /*
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

    
    /// <summary>
    /// EmbeddedResourceResolver 
    /// </summary>
    [Obsolete("Use P42.Uno/AppResources/EmbeddedResourceExtensions.FindAssembly instead.", true)]
    public static Func<string, Assembly, Assembly>? EmbeddedResourceAssemblyResolver { get; }
   

    /// <summary>
    /// Initialization
    /// </summary>
    public static void Init()
    {
        MainThreadId = System.Environment.CurrentManagedThreadId;
    }

    */

    /// <summary>
    /// Gets the operating system.
    /// </summary>
    /// <returns>The operating system.</returns>
    [Obsolete("Use .DeviceFamily, .DeviceFamilyVersion, and .DeviceForm in Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily instead.", true)]
    public static string GetOperatingSystem()
        => throw new NotImplementedException();

    private static Action? _platformPathLoader; 
    internal static Action PlatformPathLoader
    {
        get => _platformPathLoader ?? throw new IncompleteInitialization();
        set => _platformPathLoader = value;
    }

    /*
    static string _documentsPath;
    public static string DocumentsPath
    {
        get
        {
            if (_documentsPath == null)
                PlatformPathLoader?.Invoke();
            return _documentsPath;
        }
        set
        {
            _documentsPath = value;
        }
    }
    */

    private static string? _applicationDataPath;
    /// <summary>
    /// Where is app data stored?
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    public static string ApplicationLocalFolderPath
    {
        get
        {
            if (_applicationDataPath == null)
                PlatformPathLoader.Invoke();
            return _applicationDataPath ?? throw new IncompleteInitialization();
        }
        set
        {
            _applicationDataPath = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

    private static string? _applicationCachePath;
    /// <summary>
    /// Where is app data cached?
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    public static string ApplicationLocalCacheFolderPath
    {
        get
        {
            if (_applicationCachePath == null)
                PlatformPathLoader.Invoke();
            return _applicationCachePath ?? throw new IncompleteInitialization();
        }
        set
        {
            _applicationCachePath = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

    private static string? _temporaryStoragePath;
    /// <summary>
    /// Where is temp storage?
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    public static string ApplicationTemporaryFolderPath
    {
        get
        {
            if (_temporaryStoragePath == null)
                PlatformPathLoader.Invoke();
            return _temporaryStoragePath ?? throw new IncompleteInitialization();
        }
        set
        {
            _temporaryStoragePath = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

}

