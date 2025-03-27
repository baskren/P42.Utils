using Newtonsoft.Json;
namespace P42.Utils;

/// <summary>
/// Interface for IJsonWriteable
/// </summary>
public interface IJsonWriteable
{
    /// <summary>
    /// WriteValue to writer
    /// </summary>
    /// <param name="writer">JsonWriter</param>
    /// <param name="justValues"></param>
    void WriteValue(JsonWriter writer, bool justValues=false);
}
