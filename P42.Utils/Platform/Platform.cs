using System.Reflection;
using Microsoft.VisualBasic.CompilerServices;

namespace P42.Utils;

/// <summary>
/// Platform specific initialization
/// </summary>
public static class Platform
{

    //[Obsolete("NO LONGER NEEDED", true)]
    //internal static IPlatformTimer? PlatformTimer { get; set; } 
    

    internal static readonly Assembly Assembly = typeof(Platform).Assembly;
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
    
    internal static Action PlatformPathLoader
    {
        get => field ?? throw new IncompleteInitialization();
        set;
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

    /// <summary>
    /// Where is app data stored?
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    public static string ApplicationLocalFolderPath
    {
        get
        {
            if (field == null)
                PlatformPathLoader.Invoke();
            return field ?? throw new IncompleteInitialization();
        }
        set
        {
            field = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

    /// <summary>
    /// Where is app data cached?
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    public static string ApplicationLocalCacheFolderPath
    {
        get
        {
            if (field == null)
                PlatformPathLoader.Invoke();
            return field ?? throw new IncompleteInitialization();
        }
        set
        {
            field = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

    /// <summary>
    /// Where is temp storage?
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    public static string ApplicationTemporaryFolderPath
    {
        get
        {
            if (field == null)
                PlatformPathLoader.Invoke();
            return field ?? throw new IncompleteInitialization();
        }
        set
        {
            field = value;
            DirectoryExtensions.GetOrCreateDirectory(value);
        }
    }

}

