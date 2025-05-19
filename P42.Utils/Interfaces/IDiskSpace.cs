using System.Threading.Tasks;

namespace P42.Utils.Interfaces;

/// <summary>
/// Interface for IDiskSpace
/// </summary>
public interface IDiskSpace
{
    /// <summary>
    /// Available space
    /// </summary>
    Task<ulong> FreeAsync();

    /// <summary>
    /// Total Space
    /// </summary>
    Task<ulong> SizeAsync();

    /// <summary>
    /// Space used
    /// </summary>
    Task<ulong> UsedAsync();
}
