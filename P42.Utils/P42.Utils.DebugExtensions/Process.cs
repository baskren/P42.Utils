using Microsoft.VisualBasic.CompilerServices;

namespace P42.Utils;

public static class Process
{
    internal static IProcess? PlatformProcess;

    /// <summary>
    /// How much memory is consumed by this process / application
    /// </summary>
    /// <exception cref="IncompleteInitialization"></exception>
    public static ulong Memory => PlatformProcess?.Memory() ?? throw new IncompleteInitialization();

}
