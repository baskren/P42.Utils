namespace P42.Utils;

/// <summary>
/// Collapsable interface
/// </summary>
public interface ICollapsable
{
    /// <summary>
    /// Is element collapsed
    /// </summary>
    bool IsCollapsed { get; }
    
    /// <summary>
    /// Is element a template?
    /// </summary>
    bool IsTemplate { get; }
}
