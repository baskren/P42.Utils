namespace P42.Utils.Interfaces;

/// <summary>
/// Interface for IDiskSpace
/// </summary>
public interface IDiskSpace
{
    /// <summary>
    /// Available space
    /// </summary>
    ulong Free { get; }

    /// <summary>
    /// Total Space
    /// </summary>
    ulong Size { get; }

    /// <summary>
    /// Space used
    /// </summary>
    ulong Used { get; }
}
