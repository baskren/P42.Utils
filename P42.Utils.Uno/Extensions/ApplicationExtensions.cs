using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;
public static  class ApplicationExtensions
{
    public static bool CreateConsoleWindow(this Application app)
    {
        return AllocConsole();
    }

#if WINDOWS
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();
#else
    private static bool AllocConsole() => false;
#endif
}
