
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
    public static Window Window => MainWindow;

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
    
    #region Font Families
    private static FontFamily? _mathFontFamily;
    /// <summary>
    /// Math font family
    /// </summary>
    public static FontFamily MathFontFamily => _mathFontFamily ??= new FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/STIXGeneral.ttf#STIXGeneral");

    
    private static FontFamily? _sansSerifFontFamily;
    /// <summary>
    /// Segoe UI font family
    /// </summary>
    public static FontFamily SansSerifFontFamily => _sansSerifFontFamily ??= new FontFamily("Segoe UI#Regular");

    private static FontFamily? _variantsFontFamily;
    // Font that has built-in  "variants" for subscript and superscript
    // Variants don't work on UNO
    public static FontFamily VariantsFontFamily => _variantsFontFamily ??= new FontFamily("Calibri");
    //public static FontFamily VariantsFontFamily => _variantsFontFamily ??= new FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/ScriptVariants.ttf#ScriptVariants");


    static FontFamily? _serifFontFamily;
    public static FontFamily SerifFontFamily
    {
        get
        {
            if (_serifFontFamily != null)
                return _serifFontFamily;
            
            if (OperatingSystem.IsLinux())
                return _serifFontFamily =  new FontFamily("Noto Serif");
            if (OperatingSystem.IsBrowser())
                return _serifFontFamily =  new FontFamily("serif");
            
            return _serifFontFamily =  new FontFamily("Times New Roman");
        }
    }
        

    private static FontFamily? _monoSpaceFontFamily;
    /// <summary>
    /// Monospace font family
    /// </summary>
    // ReSharper disable once StringLiteralTypo
    public static FontFamily MonoSpaceFontFamily => _monoSpaceFontFamily ??= new FontFamily("ms-appx:///P42.Utils.Uno/Assets/Fonts/FiraCode-VariableFont_wght.ttf#Fira Code");
    #endregion

    static bool _hasBeenInit;
    public static void Init(Application application, Window window)
    {
        if (_hasBeenInit)
            return;
        _hasBeenInit = true;

        Application = application;
        MainWindow = window;

        Utils.Platform.PlatformPathLoader = PlatformPathLoader;
        DiskSpace.PlatformDiskSpace = new DeviceDisk();
        P42.Utils.Process.PlatformProcess = new Process();
        MainThread = Thread.CurrentThread;
        MainThreadDispatchQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        NotifiableObject.BaseNotifiablePropertyObject.MainThreadAction = P42.Utils.Uno.MainThread.Invoke;

        // DO WE NEED TO DO A RESET?
        if (!ResetRequested())
            return;

        Console.WriteLine("P42.Utils.Uno.Platform.Init RESET START");
        Task.Run(ResetAppStorage).Wait();
        Console.WriteLine("P42.Utils.Uno.Platform.Init RESET COMPLETE");
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

    // ReSharper disable once UnusedMember.Local
    public static string ExecuteShellCommand(string command)
    {
        // Create a new process
        using var process = new System.Diagnostics.Process();
        // Configure the process
        if (OperatingSystem.IsWindows())
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c \"{command}\"";
        }
        else
        {
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c \"{command}\"";
        }
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        // Start the process
        process.Start();

        // Capture output
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode == 0)
            return output;

        Serilog.QuickLog.QLog.Warning($"ExecuteCommand({command}), Error {error}");
        return string.Empty;

    }

    
}

public class NotInitializedException() : Exception("P42.Utils.Uno not initialized via P42.Utils.Uno.Platform.Init()");
