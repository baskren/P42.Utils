
namespace P42.Utils.Uno;

public static class Platform
{
    /// <summary>
    /// Floor value for FontSize, used by TextBlockExtensions.FloorFontSize
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public static double MinFontSize { get; set; } = 10.0;

    #region Application

    /// <summary>
    /// Reference to current application
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Application Application
    {
        get => field ?? throw new NotInitializedException();
        private set;
    }

    #endregion

    #region Window / Frame

    [Obsolete("Use MainWindow instead", true)]
    public static Window Window => MainWindow;

    /// <summary>
    /// Reference to main window
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static Window MainWindow
    {
        get => field ?? throw new NotInitializedException();
        private set;
    }

    public static Frame Frame 
        => MainWindow.Content as Frame
           ?? throw new NotInitializedException();
    #endregion
    
    
    #region Font Families
    /// <summary>
    /// Math font family
    /// </summary>
    public static FontFamily MathFontFamily => field ??= new FontFamily("ms-appx:///P42.Utils.Uno.Platform/Assets/Fonts/STIXGeneral.ttf#STIXGeneral");

    
    /// <summary>
    /// Segoe UI font family
    /// </summary>
    public static FontFamily SansSerifFontFamily => field ??= new FontFamily("Segoe UI#Regular");

    // Font that has built-in  "variants" for subscript and superscript
    // Variants don't work on UNO
    public static FontFamily VariantsFontFamily => field ??= new FontFamily("Calibri");
    //public static FontFamily VariantsFontFamily => _variantsFontFamily ??= new FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/ScriptVariants.ttf#ScriptVariants");


    public static FontFamily SerifFontFamily
    {
        get
        {
            if (field != null)
                return field;
            
            if (OperatingSystem.IsLinux())
                return field =  new FontFamily("Noto Serif");
            if (OperatingSystem.IsBrowser())
                return field =  new FontFamily("serif");
            
            return field =  new FontFamily("Times New Roman");
        }
    }
        

    /// <summary>
    /// Monospace font family
    /// </summary>
    // ReSharper disable once StringLiteralTypo
    public static FontFamily MonoSpaceFontFamily => field ??= new FontFamily("ms-appx:///P42.Utils.Uno.Platform/Assets/Fonts/FiraCode-VariableFont_wght.ttf#Fira Code");
    #endregion

    static bool _hasBeenInit;
    public static void Init(Application application, Window window)
    {
        P42.Utils.Platform.Init();

        if (_hasBeenInit)
            return;
        _hasBeenInit = true;

        Application = application;
        MainWindow = window;

        Utils.Platform.PlatformPathLoader = PlatformPathLoader;
        DiskSpace.PlatformDiskSpace = new DeviceDisk();

        MainThread.Init();
        NotifiableObject.BaseNotifiablePropertyObject.MainThreadAction = MainThread.Invoke;

        // DO WE NEED TO DO A RESET?
        if (!ResetRequested())
            return;

        Console.WriteLine("P42.Utils.Uno.Platform.Init RESET START");
        Task.Run(ResetAppStorage).Wait();
        Console.WriteLine("P42.Utils.Uno.Platform.Init RESET COMPLETE");

        #if BROWSERWASM
        AssemblyExtensions.WasmAssemblyDateTimeDelegate = WasmNative.GetWasmAsmDateTimeAsync;
        #endif
    }

    private static void PlatformPathLoader()
    {
        try
        {
            Utils.Platform.ApplicationLocalFolderPath = ApplicationData.Current.LocalFolder.Path;
            Utils.Platform.ApplicationLocalCacheFolderPath = ApplicationData.Current.LocalCacheFolder.Path;
            Utils.Platform.ApplicationTemporaryFolderPath = ApplicationData.Current.TemporaryFolder.Path;
        }
        catch (Exception)
        {
            try
            {
                var asm = AssemblyExtensions.GetApplicationAssembly();
                var assemblyName = asm.Name();
                // Unpackaged WinUI3 App
                Utils.Platform.ApplicationLocalFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), assemblyName);
                Directory.CreateDirectory(Utils.Platform.ApplicationLocalFolderPath);
                Utils.Platform.ApplicationLocalCacheFolderPath = Path.Combine(Utils.Platform.ApplicationLocalFolderPath, "Cache");
                Directory.CreateDirectory(Utils.Platform.ApplicationLocalCacheFolderPath);
                Utils.Platform.ApplicationTemporaryFolderPath = Path.Combine(Path.GetTempPath(), assemblyName);
                Directory.CreateDirectory(Utils.Platform.ApplicationTemporaryFolderPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    private static bool ResetRequested()
    {
#if BROWSERWASM

        // app path needs to be appended with the following, without quotes: "?ResetAppStorage="
        var fullUrlText = WasmNative.GetPageUrl();
        var fullUrl = new Uri(fullUrlText);
        var args = global::Uno.Extensions.UriExtensions.GetParameters(fullUrl);
        if (args.Keys.FirstOrDefault(k => k.Equals(nameof(ResetAppStorage), StringComparison.OrdinalIgnoreCase)) is not { } resetKey)
            return false; 
        var value = args[resetKey];
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
        await ApplicationData.Current.LocalFolder.DeleteChildrenAsync();
        await ApplicationData.Current.LocalCacheFolder.DeleteChildrenAsync();
        await ApplicationData.Current.TemporaryFolder.DeleteChildrenAsync();

#if __IOS__ || __MACCATALYST__
        Foundation.NSUserDefaults.StandardUserDefaults.SetBool(false, nameof(ResetAppStorage));
#elif BROWSERWASM
        var fullUrlText = WasmNative.GetPageUrl();
        var fullUrl = new Uri(fullUrlText);
        var args = global::Uno.Extensions.UriExtensions.GetParameters(fullUrl);
        if (args.Keys.FirstOrDefault(k => k.Equals(nameof(ResetAppStorage), StringComparison.OrdinalIgnoreCase)) is { } resetKey)
            args.Keys.Remove(resetKey);
        var builder = new UriBuilder(fullUrl);
        var query = System.Web.HttpUtility.ParseQueryString(builder.Query);
        query.Remove(nameof(ResetAppStorage));
        builder.Query = query.ToString();
        var updatedUrl = builder.ToString();
        WasmNative.SetPageUrl(updatedUrl);
#endif
    }


#if __IOS__
    [JetBrains.Annotations.PublicAPI]
    public class SettingsObserver
    {
        
        public static void StartListening()
        {
            Foundation.NSNotificationCenter.DefaultCenter.AddObserver(
                Foundation.NSUserDefaults.DidChangeNotification,
                (_) =>
                {
                    if (Foundation.NSUserDefaults.StandardUserDefaults.BoolForKey(nameof(ResetAppStorage)))
                        ResetAppStorage().Wait();
                });
        }
    }
#endif
    
}

public class NotInitializedException() : Exception("P42.Utils.Uno not initialized via P42.Utils.Uno.Platform.Init()");
