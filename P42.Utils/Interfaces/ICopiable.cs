namespace P42.Utils;

/// <summary>
/// Interface for Copiable
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICopiable<in T>
{
    /// <summary>
    /// Set this instances properties from other
    /// </summary>
    /// <param name="other"></param>
    void PropertiesFrom(T other);
}
