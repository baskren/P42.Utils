using Microsoft.VisualBasic.CompilerServices;
using P42.Utils.Interfaces;

namespace P42.Utils;

/// <summary>
/// Query Disk Space
/// </summary>
public static class DiskSpace
{
    internal static IDiskSpace? PlatformDiskSpace;

    /// <summary>
    /// Free disk space
    /// </summary>
    public static ulong Free => PlatformDiskSpace?.Free ?? throw new IncompleteInitialization();

    /// <summary>
    /// Total disk space
    /// </summary>
    public static ulong Size => PlatformDiskSpace?.Size ?? throw new IncompleteInitialization();

    /// <summary>
    /// Used disk space
    /// </summary>
    public static ulong Used => PlatformDiskSpace?.Used ?? throw new IncompleteInitialization();

}
