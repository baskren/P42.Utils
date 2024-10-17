using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.UI.Dispatching;

namespace P42.Utils.Uno;

public static class Platform
{
    [Obsolete("Use P42.Utils.Uno.TextBlockExtensions.MinFontSize instead.", true)]
    public static double MinFontSize { get; set; }

    public static Microsoft.UI.Xaml.Application? Application { get; private set; }

    public static Microsoft.UI.Xaml.Window? Window { get; private set; }

    public static Thread? MainThread { get; private set; }

    public static DispatcherQueue? MainThreadDispatchQueue { get; private set; }

    private static List<Assembly>? _assembliesToInclude;
    public static List<Assembly> AssembliesToInclude => _assembliesToInclude ??= [typeof(P42.Utils.DebugExtensions).Assembly];


    private static Assembly? _assembly;
    public static Assembly Assembly => _assembly ??= typeof(Platform).Assembly;

    private static Microsoft.UI.Xaml.Media.FontFamily? _mathFontFamily;
    public static Microsoft.UI.Xaml.Media.FontFamily MathFontFamily => _mathFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily(AssetExtensions.AssetPath("ms-appx:///P42.Utils.Uno/Assets/Fonts/STIXGeneral.ttf#STIXGeneral"));

    private static Microsoft.UI.Xaml.Media.FontFamily? _sansSerifFontFamily;
    public static Microsoft.UI.Xaml.Media.FontFamily SansSerifFontFamily => _sansSerifFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI#Regular");

    private static Microsoft.UI.Xaml.Media.FontFamily? _monoSpaceFontFamily;
    public static Microsoft.UI.Xaml.Media.FontFamily MonoSpaceFontFamily => _monoSpaceFontFamily ??= new Microsoft.UI.Xaml.Media.FontFamily(AssetExtensions.AssetPath("ms-appx:///P42.Utils.Uno/Assets/Fonts/FiraCode-VariableFont_wght.ttf#Fira Code"));


    /// <summary>
    /// Initializes P42.Utils.Uno
    /// </summary>
    /// <param name="application"></param>
    /// <param name="window"></param>
    public static void Init(Microsoft.UI.Xaml.Application application, Microsoft.UI.Xaml.Window window)
    {
        Application = application;
        Window = window;
        Environment.Init();
        Environment.PlatformTimer = new Timer();
        Environment.PlatformPathLoader = PlatformPathLoader;
        P42.Utils.DiskSpace.PlatformDiskSpace = new DiskSpace();
        P42.Utils.Process.PlatformProcess = new Process();
        Environment.EmbeddedResourceAssemblyResolver = EmbeddedResourceExtensions.FindAssemblyForResourceId;
        MainThread = Thread.CurrentThread;
        MainThreadDispatchQueue = DispatcherQueue.GetForCurrentThread();

        NotifiableObject.BaseNotifiablePropertyObject.MainThreadAction = P42.Utils.Uno.MainThread.InvokeOnMainThread;
    }

    private static void PlatformPathLoader()
    {
        Environment.ApplicationDataPath = Windows.Storage.ApplicationData.Current.LocalFolder.Path;
        Environment.ApplicationCachePath = Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path;
        Environment.TemporaryStoragePath = Windows.Storage.ApplicationData.Current.TemporaryFolder.Path;
    }

    
}
