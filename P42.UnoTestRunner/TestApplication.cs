using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Data;


namespace P42.UnoTestRunner;

[Bindable]
public partial class TestApplication : Application
{
    static TestApplication? _instance;
    public static TestApplication Instance => _instance ?? throw new Exception("TestApplication.Instance called before being set");

    static Window? _mainWindow;
    static public Window MainWindow
    {
        get => _mainWindow ?? throw new Exception("TestApplication.TestWindow called before being set.  Did you override OnLaunched and then forget to call base.OnLaunched() or forget to remove declaration of 'MainWindow' from App.xaml.cs ?");
        set
        {
            if (_mainWindow is not null)
                throw new Exception("TestApplication.Instance is a singleton and cannot be reset");
            _mainWindow = value;
        }
    }

    static Thread? _mainThread;
    static public Thread MainThread
    {
        get => _mainThread ?? throw new Exception("TestApplication.MainThread called before being set.");
        set
        {
            if (_mainThread is not null)
                throw new Exception("TestApplication.MainThread is a singleton and cannot be reset");
            _mainThread = value;
        }
    }

    static DispatcherQueue? _mainThreadDispatchQueue;
    static public DispatcherQueue MainThreadDispatchQueue
    {
        get => _mainThreadDispatchQueue ?? throw new Exception("TestApplication.MainThreadDispatchQueue not set");
        set
        {
            if (_mainThreadDispatchQueue is not null)
                throw new Exception("TestApplication.MainThreadDispatchQueue cannot be reset");
            _mainThreadDispatchQueue = value;
        }
    }

    public TestApplication()
    {
        MainThread = Thread.CurrentThread;
        MainThreadDispatchQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        _instance = this;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new Window();

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

        // MainWindow.SetWindowIcon();  Uno.Resizetizer not accessable from library project
        // Ensure the current window is active
        MainWindow.Activate();
    }

    /// <summary>
    /// Invoked when Navigation to a certain page fails
    /// </summary>
    /// <param name="sender">The Frame which failed navigation</param>
    /// <param name="e">Details about the navigation failure</param>
    protected virtual void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new InvalidOperationException($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
    }


}
