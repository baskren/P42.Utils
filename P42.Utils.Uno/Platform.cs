
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

    public static void Init(Application application, Window window)
    {
        Application = application;
        MainWindow = window;
        // Environment.Init();
        // Environment.PlatformTimer = new Timer();
        Environment.PlatformPathLoader = PlatformPathLoader;
        DiskSpace.PlatformDiskSpace = new DeviceDisk();
        P42.Utils.Process.PlatformProcess = new P42.Utils.Uno.Process();
        MainThread = Thread.CurrentThread;
        MainThreadDispatchQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        NotifiableObject.BaseNotifiablePropertyObject.MainThreadAction = P42.Utils.Uno.MainThread.Invoke;
    }

    private static void PlatformPathLoader()
    {
        Environment.ApplicationLocalFolderPath = ApplicationData.Current.LocalFolder.Path;
        Environment.ApplicationLocalCacheFolderPath = ApplicationData.Current.LocalCacheFolder.Path;
        Environment.ApplicationTemporaryFolderPath = ApplicationData.Current.TemporaryFolder.Path;
    }

}

public class NotInitializedException : Exception 
{
    public NotInitializedException() : base("P42.Utils.Uno not initialized via P42.Utils.Uno.Platform.Init()") { }
}
