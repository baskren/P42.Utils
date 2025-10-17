using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

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
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool FreeConsole();

#else
    private static bool AllocConsole() => false;

    private static bool FreeConsole() => false;
#endif

}
