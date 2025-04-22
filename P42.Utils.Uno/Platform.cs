
using System.Web;

namespace P42.Utils.Uno;

public static class Platform
{
    /// <summary>
    /// Floor value for FontSize, used by TextBlockExtensions.FloorFontSize
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public static double MinFontSize { get; set; } = 10.0;

    #region Application
    private static Application? _application;
    /// <summary>
    /// Reference to current application
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Application Application
    {
        get => _application ?? throw new NotInitializedException();
        private set => _application = value;
    }
    #endregion

    #region Window / Frame
    private static Window? _mainWindow;

    [Obsolete("Use MainWindow instead", true)]
    public static Window Window => throw new NotImplementedException();

    /// <summary>
    /// Reference to main window
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Window MainWindow
    {
        get => _mainWindow ?? throw new NotInitializedException();
        private set => _mainWindow = value; 
    }

    public static Frame Frame 
        => MainWindow.Content as Frame
           ?? throw new NotInitializedException();
    #endregion
    
    #region MainThread / MainThreadDispatchQueue
    static Thread? _mainThread;
    /// <summary>
    /// Applications UI Thread
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Thread MainThread
    {
        get => _mainThread ?? throw new NotInitializedException(); 
        private set => _mainThread = value;
    }

    private static Microsoft.UI.Dispatching.DispatcherQueue? _mainThreadDispatchQueue;
    /// <summary>
    /// DispatchQueue for the main thread
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Microsoft.UI.Dispatching.DispatcherQueue MainThreadDispatchQueue
    {
        get => _mainThreadDispatchQueue ?? throw new NotInitializedException(); 
        private set => _mainThreadDispatchQueue = value;
    }
    #endregion

    #region AssembliesToInclude
    private static List<System.Reflection.Assembly>? _assembliesToInclude;
    [Obsolete("Is this still needed?", true)]
    public static List<System.Reflection.Assembly> AssembliesToInclude => _assembliesToInclude ??= [typeof(P42.Utils.DebugExtensions).Assembly];
    #endregion

    #region Assembly
    [Obsolete("Is this still needed?", true)]
    public static System.Reflection.Assembly Assembly => throw new NotImplementedException();
    #endregion

    #region Font Families
    private static Microsoft.UI.Xaml.Media.FontFamily? _mathFontFamily;
    /// <summary>
    /// Math font family
    /// </summary>
    public static Microsoft.UI.Xaml.Media.FontFamily MathFontFamily => _mathFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/STIXGeneral.ttf#STIXGeneral");

    
    private static Microsoft.UI.Xaml.Media.FontFamily? _sansSerifFontFamily;
    /// <summary>
    /// Segoe UI font family
    /// </summary>
    public static Microsoft.UI.Xaml.Media.FontFamily SansSerifFontFamily => _sansSerifFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI#Regular");


    static Microsoft.UI.Xaml.Media.FontFamily? _serifFontFamily;
    public static Microsoft.UI.Xaml.Media.FontFamily SerifFontFamily => _serifFontFamily ??=
        #if __IOS__ || __MACCATALYST__ || __DESKTOP__ || __MACOS__
        new Microsoft.UI.Xaml.Media.FontFamily("Times New Roman");
        #elif WINDOWS
        new Microsoft.UI.Xaml.Media.FontFamily("Cambria");
        #else
        new Microsoft.UI.Xaml.Media.FontFamily("serif");
        #endif
        

    private static Microsoft.UI.Xaml.Media.FontFamily? _monoSpaceFontFamily;
    /// <summary>
    /// Monospace font family
    /// </summary>
    // ReSharper disable once StringLiteralTypo
    public static Microsoft.UI.Xaml.Media.FontFamily MonoSpaceFontFamily => _monoSpaceFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/FiraCode-VariableFont_wght.ttf#Fira Code");
    #endregion

    static bool _hasBeenInit;
    public static void Init(Application application, Window window)
    {
        Console.WriteLine($"P42.Utils.Uno.Platform.Init A : hasBeenInit[{_hasBeenInit}]");
        if (_hasBeenInit)
            return;
        _hasBeenInit = true;

        Application = application;
        MainWindow = window;

        Environment.PlatformPathLoader = PlatformPathLoader;
        DiskSpace.PlatformDiskSpace = new DeviceDisk();
        P42.Utils.Process.PlatformProcess = new P42.Utils.Uno.Process();
        MainThread = Thread.CurrentThread;
        MainThreadDispatchQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        NotifiableObject.BaseNotifiablePropertyObject.MainThreadAction = P42.Utils.Uno.MainThread.Invoke;

        // DO WE NEED TO DO A RESET?
        if (ResetRequested())
        {
            Console.WriteLine($"P42.Utils.Uno.Platform.Init B");
            Task.Run(ResetAppStorage).Wait();
            Console.WriteLine($"P42.Utils.Uno.Platform.Init C");
        }
    }

    private static void PlatformPathLoader()
    {
        Environment.ApplicationLocalFolderPath = ApplicationData.Current.LocalFolder.Path;
        Environment.ApplicationLocalCacheFolderPath = ApplicationData.Current.LocalCacheFolder.Path;
        Environment.ApplicationTemporaryFolderPath = ApplicationData.Current.TemporaryFolder.Path;
    }

    private static bool ResetRequested()
    {
#if BROWSERWASM

        // app path needs to be appended with the following, without quotes: "?ResetAppStorage="
        Console.WriteLine($"P42.Utils.Uno.Platform.ResetRequested A");

        var fullUrlText = global::Uno.Foundation.WebAssemblyRuntime.InvokeJS("window.location.href;");
        var fullUrl = new Uri(fullUrlText);
        var args = global::Uno.Extensions.UriExtensions.GetParameters(fullUrl);
        Console.WriteLine($"P42.Utils.Uno.Platform.ResetRequested B");
        if (args.Keys.FirstOrDefault(k => k.Equals(nameof(ResetAppStorage), StringComparison.OrdinalIgnoreCase)) is not { } resetKey)
            return false; 
        var value = args[resetKey];
        Console.WriteLine($"P42.Utils.Uno.Platform.ResetRequested C");
        return string.IsNullOrEmpty(value) || value.Equals("true", StringComparison.OrdinalIgnoreCase);

#elif __IOS__

        SettingsObserver.StartListening();        
        return Foundation.NSUserDefaults.StandardUserDefaults.BoolForKey(nameof(ResetAppStorage));
#else
        return false;
#endif
    }

    public static async Task ResetAppStorage()
    {
        Console.WriteLine($"P42.Utils.Uno.Platform.ResetAppStorage A");
        await ApplicationData.Current.LocalFolder.DeleteChildrenAsync();
        await ApplicationData.Current.LocalCacheFolder.DeleteChildrenAsync();
        await ApplicationData.Current.TemporaryFolder.DeleteChildrenAsync();
        Console.WriteLine($"P42.Utils.Uno.Platform.ResetAppStorage B");

#if __IOS__ || __MACCATALYST__
        Foundation.NSUserDefaults.StandardUserDefaults.SetBool(false, nameof(ResetAppStorage));
#elif BROWSERWASM
        var fullUrlText = global::Uno.Foundation.WebAssemblyRuntime.InvokeJS("window.location.href;");
        var fullUrl = new Uri(fullUrlText);
        var args = global::Uno.Extensions.UriExtensions.GetParameters(fullUrl);
        if (args.Keys.FirstOrDefault(k => k.Equals(nameof(ResetAppStorage), StringComparison.OrdinalIgnoreCase)) is { } resetKey)
            args.Keys.Remove(resetKey);
        var builder = new UriBuilder(fullUrl);
        var query = HttpUtility.ParseQueryString(builder.Query);
        query.Remove(nameof(ResetAppStorage));
        builder.Query = query.ToString();
        var updatedUrl = builder.ToString();
        global::Uno.Foundation.WebAssemblyRuntime.InvokeJS($"window.location.href={updatedUrl};");
#endif
    }


#if __IOS__
    public class SettingsObserver
    {
        
        public static void StartListening()
        {
            Foundation.NSNotificationCenter.DefaultCenter.AddObserver(
                Foundation.NSUserDefaults.DidChangeNotification,
                (notification) =>
                {
                    if (Foundation.NSUserDefaults.StandardUserDefaults.BoolForKey(nameof(ResetAppStorage)))
                        ResetAppStorage().Wait();
                });
        }
    }
#endif

}

public class NotInitializedException : Exception 
{
    public NotInitializedException() : base("P42.Utils.Uno not initialized via P42.Utils.Uno.Platform.Init()") { }
}
