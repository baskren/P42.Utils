using System;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using AsyncAwaitBestPractices;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class DeviceDisplay
{
    #region Private Fields and Properties

    private static WeakEventManager<DisplayInfoChangedEventArgs>? _mainDisplayInfoChangedEventManager; 
    private static WeakEventManager<DisplayInfoChangedEventArgs> MainDisplayInfoChangedEventManager => _mainDisplayInfoChangedEventManager ??= new(); 

    private static DisplayMetrics _currentMetrics;

    private static System.Threading.Lock? _lock;
    private static System.Threading.Lock Locker => _lock ??= new System.Threading.Lock();
    
    private static Windows.System.Display.DisplayRequest? _displayRequest;

    private static DisplayMetrics? _defaultMetrics;
    private static DisplayMetrics DefaultMetrics => _defaultMetrics ??= new DisplayMetrics(
                                                                            width: 1920,
                                                                            height: 1080,
                                                                            density: 1,
                                                                            orientation: DisplayOrientation.Portrait,
                                                                            rotation: DisplayRotation.Rotation0);
    #endregion
    
    
    /// <summary>
    /// Force screen from sleeping
    /// </summary>
    public static bool KeepScreenOn
    {
        get
        {
            lock (Locker)
            {
                return _displayRequest is not null;
            }
        }
        set
        {
            lock (Locker)
            {
                if (value)
                {
                    if (_displayRequest is not null)
                        return;

                    _displayRequest = new Windows.System.Display.DisplayRequest();
                    _displayRequest.RequestActive();
                }
                else
                {
                    if (_displayRequest is null)
                        return;

                    _displayRequest.RequestRelease();
                    _displayRequest = null;
                }
            }
        }
    }

    private static int _subscribersCount;
    // ReSharper disable once EventNeverSubscribedTo.Global
    /// <summary>
    /// Update when the main display info changes
    /// </summary>
    public static event EventHandler<DisplayInfoChangedEventArgs> MainDisplayInfoChanged
    {
        add
        {
            lock (Locker)
            {
                MainDisplayInfoChangedEventManager.AddEventHandler(value);
                if (_subscribersCount++ > 0)
                    return;
                SetCurrent(GetMainDisplayInfo());
                StartScreenMetricsListeners();
            }            
        }
        remove
        {
            lock (Locker)
            {
                MainDisplayInfoChangedEventManager.RemoveEventHandler(value);
                if (--_subscribersCount > 0)
                    return;
                StopScreenMetricsListeners();
            }            
        }
    }

    /// <summary>
    /// Is the MainDisplayInfo to be trusted?
    /// </summary>
    public static bool IsMainDisplayInfoTrusted { get; private set; }

    public static DisplayMetrics MainDisplayInfo
        => IsMainDisplayInfoTrusted ? _currentMetrics : DefaultMetrics;
    
    public static DisplayMetrics QueryMainDisplayInfo()
        => GetMainDisplayInfo();

    /// <summary>
    /// Get metrics for main display
    /// </summary>
    /// <param name="di"></param>
    /// <returns></returns>
    private static DisplayMetrics GetMainDisplayInfo(DisplayInformation? di = null)
    {
        try
        {
#if WINDOWS
            if (di is not null)
            {
                var result = di.ToDisplayMetrics();
                IsMainDisplayInfoTrusted = true;
                return result;
            }
            if (DisplayHelper.GetDisplayMetricsForWindow(Platform.MainWindow) is { } metrics)
            {

                IsMainDisplayInfoTrusted = true;
                return metrics;
            }
#else
            di ??= MainThread.Invoke(DisplayInformation.GetForCurrentView);
            var result = di.ToDisplayMetrics();
            IsMainDisplayInfoTrusted = true;
            return result;
#endif

        }
        catch (Exception)
        {
        }
        IsMainDisplayInfoTrusted = false;
        return DefaultMetrics;

    }


    #region supporting methods

    private static bool _listeningToMetrics;
    private static void StartScreenMetricsListeners()
    {
        lock (Locker)
        {
            if (_listeningToMetrics)
                return;
            _listeningToMetrics = true;
            MainThread.Invoke(() =>
            {
                try
                {
                    var di = DisplayInformation.GetForCurrentView();

                    di.DpiChanged += OnDisplayInformationChanged;
                    di.OrientationChanged += OnDisplayInformationChanged;
                }
                catch (Exception e)
                {
                    _listeningToMetrics = false;
                    QLog.Error(e);
                }
            });
        }
    }

    private static void StopScreenMetricsListeners()
    {
        lock (Locker)
        {
            if (!_listeningToMetrics)
                return;
            _listeningToMetrics = false;
            MainThread.Invoke(() =>
            {
                try
                {
                    var di = DisplayInformation.GetForCurrentView();

                    di.DpiChanged -= OnDisplayInformationChanged;
                    di.OrientationChanged -= OnDisplayInformationChanged;
                }
                catch (Exception e)
                {
                    _listeningToMetrics = true;
                    QLog.Error(e);
                }
            });
        }
    }

    private static void OnDisplayInformationChanged(DisplayInformation? di, object? args)
    {
        var metrics = GetMainDisplayInfo(di);
        OnMainDisplayInfoChanged(metrics);
    }

    private static void OnMainDisplayInfoChanged(DisplayMetrics metrics)
        => OnMainDisplayInfoChanged(new DisplayInfoChangedEventArgs(metrics)).SafeFireAndForget(ex => QLog.Error(ex));

    private static async Task OnMainDisplayInfoChanged(DisplayInfoChangedEventArgs e)
    {
        if (_currentMetrics.Equals(e.DisplayMetrics))
            return;

        SetCurrent(e.DisplayMetrics);
        await Task.Delay(500); // this is to allow AppWindow.SafeMargin() to be accurate
        MainDisplayInfoChangedEventManager.RaiseEvent(e, nameof(MainDisplayInfoChanged));
    }

    private static void SetCurrent(DisplayMetrics metrics)
    => _currentMetrics = metrics.Copy();

    #endregion

}

public class DisplayInfoChangedEventArgs(DisplayMetrics displayMetrics) : EventArgs
{
    public DisplayMetrics DisplayMetrics { get; } = displayMetrics;
}
