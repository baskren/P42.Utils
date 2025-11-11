using Microsoft.VisualBasic.CompilerServices;
using P42.Utils.Interfaces;

namespace P42.Utils;

/// <summary>
/// Query Disk Space
/// </summary>
public static class DiskSpace
{
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
    internal static IDiskSpace? PlatformDiskSpace;
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value

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
