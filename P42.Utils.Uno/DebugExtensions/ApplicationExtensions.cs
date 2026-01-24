namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class ApplicationExtensions
{
    /// <summary>
    /// Create a window that shows console output (only works in WinAppSdk)
    /// </summary>
    /// <returns></returns>
    public static bool OpenConsoleWindow()
        => AllocConsole();

    /// <summary>
    /// Close a window that shows console output (only works in WinAppSdk)
    /// </summary>
    /// <returns></returns>
    public static bool CloseConsoleWindow()
        => FreeConsole();



#if WINDOWS
    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

#else
    private static bool AllocConsole() => false;

    private static bool FreeConsole() => false;
#endif

}
