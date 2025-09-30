using Newtonsoft.Json;
namespace P42.Utils;

/// <summary>
/// Interface for IJsonReadable
/// </summary>
public interface IJsonReadable
{
    /// <summary>
    /// Applies properties values found in JsonReader
    /// </summary>
    /// <param name="reader"></param>
    void PropertiesFrom(JsonReader reader);
}

/// <summary>
/// Interface for IJsonReadable
/// </summary>
public interface IJsonReadable<T> : IJsonReadable, ICopiable<T>
{
}
