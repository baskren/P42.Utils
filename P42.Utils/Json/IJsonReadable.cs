using System.Text.Json;

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
    void PropertiesFrom(Utf8JsonReader reader);
}

/// <summary>
/// Interface for IJsonReadable
/// </summary>
// ReSharper disable once UnusedType.Global
public interface IJsonReadable<T> : IJsonReadable, ICopiable<T>;
