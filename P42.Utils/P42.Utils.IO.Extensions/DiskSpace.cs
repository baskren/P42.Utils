using System.Threading.Tasks;
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
    public static Task<ulong> FreeAsync() => PlatformDiskSpace?.FreeAsync() ?? throw new IncompleteInitialization();

    /// <summary>
    /// Total disk space
    /// </summary>
    public static Task<ulong> SizeAsync() => PlatformDiskSpace?.SizeAsync() ?? throw new IncompleteInitialization();

    /// <summary>
    /// Used disk space
    /// </summary>
    public static Task<ulong> UsedAsync => PlatformDiskSpace?.UsedAsync() ?? throw new IncompleteInitialization();

}
