using System.Text.Json.Serialization;
// ReSharper disable InconsistentNaming

namespace P42.Utils;

[JsonSerializable(typeof(ObservableConcurrentDictionary<string, string>))]
public partial class ObservableConcurrentDictionary_string_string_SerializerContext : JsonSerializerContext;

