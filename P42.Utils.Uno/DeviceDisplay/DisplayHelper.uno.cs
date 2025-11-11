using Windows.Graphics.Display;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class DisplayHelper
{
    public static DisplayMetrics? GetDisplayMetricsForWindow(Window _)
    {
        var di = MainThread.Invoke(DisplayInformation.GetForCurrentView);
        return di.ToDisplayMetrics();
    }
}
