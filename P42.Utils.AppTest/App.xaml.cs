using System;
using Microsoft.Extensions.Logging;
using Uno.Resizetizer;

namespace P42.Utils.AppTest;

public partial class App : global::P42.UnoTestRunner.TestApplication
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        //P42.UnoTestRunner.TestApplication.MainThread = Thread.CurrentThread;
        //P42.UnoTestRunner.TestApplication.MainThreadDispatchQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        InitializeExceptionHandling();
        //_instance = this;
        this.InitializeComponent();
    }

    //static App? _instance;
    //public static App Instance => _instance ?? throw new Exception("TestApplication.Instance called before being set");

    public UnoTestRunner.ConsoleOutputRedirector ConsoleOutputRedirector = new UnoTestRunner.ConsoleOutputRedirector();

    
    Window? _mainWindow;
    public Window MainWindow
    {
        get => _mainWindow ?? throw new Exception("TestApplication.TestWindow called before being set.  Did you override OnLaunched and then forget to call base.OnLaunched() or forget to remove declaration of 'MainWindow' from App.xaml.cs ?");
        set
        {
            if (_mainWindow is not null)
                throw new Exception("TestApplication.Instance is a singleton and cannot be reset");
            _mainWindow = value;
        }
    }

    /*
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        P42.UnoTestRunner.TestApplication.MainWindow = MainWindow = new Window();
#if DEBUG
        MainWindow.UseStudio();
#endif


        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (MainWindow.Content is not Frame rootFrame)
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new Frame();

            // Place the frame in the current Window
            MainWindow.Content = rootFrame;

            rootFrame.NavigationFailed += OnNavigationFailed;
        }

        if (rootFrame.Content == null)
        {
            // When the navigation stack isn't restored navigate to the first page,
            // configuring the new page by passing required information as a navigation
            // parameter
            rootFrame.Navigate(typeof(P42.UnoTestRunner.TestControlPage), args.Arguments);
        }

        MainWindow.SetWindowIcon();
        // Ensure the current window is active
        MainWindow.Activate();
    }
    */

    /*
    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }
    */

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    public static void InitializeLogging()
    {
#if DEBUG
        // Logging is disabled by default for release builds, as it incurs a significant
        // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
        // is a concern for your application, keep this disabled. If you're running on the web or
        // desktop targets, you can use URL or command line parameters to enable it.
        //
        // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__ || __MACCATALYST__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#else
            builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);

            // Generic Xaml events
            // builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

            // Layouter specific messages
            // builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

            // builder.AddFilter("Windows.Storage", LogLevel.Debug );

            // Binding related messages
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

            // Binder memory references tracking
            // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

            // DevServer and HotReload related
            // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

            // Debug JS interop
            // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }

    private void InitializeExceptionHandling()
    {
        System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        //System.AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
        Microsoft.UI.Xaml.Application.Current.UnhandledException += CurrentApplication_UnhandledException;

        // https://learn.microsoft.com/en-us/windows/uwp/launch-resume/app-lifecycle

    }

    private void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
    {
        Console.WriteLine($"FIRST CHANCE APPLICATION EXCEPTION: {e.Exception}");
    }

    private void CurrentApplication_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Console.WriteLine($"UNHANDLED APPLICATION EXCEPTION: {e.Exception}");
    }

    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        Console.WriteLine($"UNHANDLED CURRENT DOMAIN EXCEPTION: {e.ExceptionObject}");
    }
}
