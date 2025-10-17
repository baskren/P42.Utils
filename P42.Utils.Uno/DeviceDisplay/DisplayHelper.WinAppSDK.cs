using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace P42.Utils.Uno;

public static partial class DisplayHelper
{
    public static DisplayMetrics? GetDisplayMetricsForWindow(Window window)
    {
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var monitor = MonitorFromWindow(hwnd, 2); // MonitorFromWindow flags: 2 = MONITOR_DEFAULTTONEAREST

        var mi = new MONITORINFO();
        mi.cbSize = Marshal.SizeOf(mi);


        if (!GetMonitorInfo(monitor, ref mi))
            return null;

        var width = mi.rcMonitor.right - mi.rcMonitor.left;
        var height = mi.rcMonitor.bottom - mi.rcMonitor.top;
        var orientation = width >= height ? DisplayOrientation.Landscape : DisplayOrientation.Portrait;

        var dpi = GetDpiForWindow(hwnd);
        var density = dpi / 96.0;

        return new DisplayMetrics(width, height, density, orientation, DisplayRotation.Unknown);


    }

    public static uint GetCurrentDpi(Window window)
    {
        // Get the window handle (HWND) for the WinUI 3 window.
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        return GetDpiForWindow(hwnd);
    }

    public static (uint dpiX, uint dpiY) GetDpiForCurrentMonitor(Window window)
    {
        const int MDT_EFFECTIVE_DPI = 0;
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        var monitor = MonitorFromWindow(hwnd, 2); // MonitorFromWindow flags: 2 = MONITOR_DEFAULTTONEAREST
        GetDpiForMonitor(monitor, MDT_EFFECTIVE_DPI, out var dpiX, out var dpiY);
        return (dpiX, dpiY);
    }

    public static int GetScreenWidth() => GetSystemMetrics(0); // SM_CXSCREEN
    public static int GetScreenHeight() => GetSystemMetrics(1); // SM_CYSCREEN


    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    public static string GetPrimaryMonitorOrientation()
    {
        var screenWidth = GetSystemMetrics(SM_CXSCREEN);
        var screenHeight = GetSystemMetrics(SM_CYSCREEN);

        return screenWidth >= screenHeight ? "Landscape" : "Portrait";
    }

    
    [LibraryImport("User32.dll")]
    private static partial uint GetDpiForWindow(IntPtr hwnd);
    
    [LibraryImport("Shcore.dll")]
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static partial int GetDpiForMonitor(IntPtr hmonitor, int dpiType, out uint dpiX, out uint dpiY);

    [LibraryImport("User32.dll")]
    private static partial IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    [LibraryImport("User32.dll")]
    private static partial int GetSystemMetrics(int nIndex);

    [LibraryImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    // ReSharper disable once UnusedMember.Local
    private static partial bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    [LibraryImport("User32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData);

    [StructLayout(LayoutKind.Sequential)]
    private struct MONITORINFO
    {
        public int cbSize;
        public RECT rcMonitor;
        public RECT rcWork;
        public uint dwFlags;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }



}
