using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.UI.Dispatching;

namespace P42.Utils.Uno;

public static class Platform
{
    /// <summary>
    /// Floor value for FontSize, used by TextBlockExtensions.FloorFontSize
    /// </summary>
    public static double MinFontSize { get; set; } = 10.0;

    #region Application
    private static Microsoft.UI.Xaml.Application? _application;
    /// <summary>
    /// Reference to current application
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Microsoft.UI.Xaml.Application Application
    {
        get => _application ?? throw new Exception("P42.Utils.Uno.Platform.Application not set");
        private set => _application = value;
    }
    #endregion

    #region Window / Frame
    private static Microsoft.UI.Xaml.Window? _mainWindow;

    [Obsolete("Use MainWindow instead", true)]
    public static Microsoft.UI.Xaml.Window Window => throw new NotImplementedException();

    /// <summary>
    /// Reference to main window
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Microsoft.UI.Xaml.Window MainWindow
    {
        get => _mainWindow ?? throw new Exception("P42.Utils.Uno.Platform.Window not set");
        private set => _mainWindow = value; 
    }

    public static Microsoft.UI.Xaml.Controls.Frame Frame 
        => MainWindow.Content as Microsoft.UI.Xaml.Controls.Frame
           ?? throw new Exception("P42.Utils.Uno.Platform.MainWindow.Current not a Frame");
    #endregion
    
    #region MainThread / MainThreadDispatchQueue
    static Thread? _mainThread;
    /// <summary>
    /// Applications UI Thread
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Thread MainThread
    {
        get => _mainThread ?? throw new Exception("P42.Utils.Uno.Platform.MainThread not set"); 
        private set => _mainThread = value;
    }

    private static DispatcherQueue? _mainThreadDispatchQueue;
    /// <summary>
    /// DispatchQueue for the main thread
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static DispatcherQueue MainThreadDispatchQueue
    {
        get => _mainThreadDispatchQueue ?? throw new Exception("Platform.MainThreadDispatchQueue not set"); 
        private set => _mainThreadDispatchQueue = value;
    }
    #endregion

    #region AssembliesToInclude
    private static List<Assembly>? _assembliesToInclude;
    [Obsolete("Is this still needed?", true)]
    public static List<Assembly> AssembliesToInclude => _assembliesToInclude ??= [typeof(P42.Utils.DebugExtensions).Assembly];
    #endregion

    #region Assembly
    private static Assembly? _assembly;
    [Obsolete("Is this still needed?", true)]
    public static Assembly Assembly => _assembly ??= typeof(Platform).Assembly;
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
    public static Microsoft.UI.Xaml.Media.FontFamily MonoSpaceFontFamily => _monoSpaceFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/FiraCode-VariableFont_wght.ttf#Fira Code");
    #endregion

    public static void Init(Microsoft.UI.Xaml.Application application, Microsoft.UI.Xaml.Window window)
    {
        Application = application;
#pragma warning disable CS0618 // Type or member is obsolete
        MainWindow = window;
#pragma warning restore CS0618 // Type or member is obsolete
        Environment.Init();
        // Environment.PlatformTimer = new Timer();
        Environment.PlatformPathLoader = PlatformPathLoader;
        P42.Utils.DiskSpace.PlatformDiskSpace = new DeviceDisk();
        P42.Utils.Process.PlatformProcess = new Process();
        MainThread = Thread.CurrentThread;
        MainThreadDispatchQueue = DispatcherQueue.GetForCurrentThread();

        NotifiableObject.BaseNotifiablePropertyObject.MainThreadAction = P42.Utils.Uno.MainThread.Invoke;
    }

    private static void PlatformPathLoader()
    {
        Environment.ApplicationLocalFolderPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        Environment.ApplicationLocalCacheFolderPath = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
        Environment.ApplicationTemporaryFolderPath = Windows.Storage.ApplicationData.Current.TemporaryFolder.Path;
    }

}
