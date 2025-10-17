using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace P42.Utils;

public partial class Serializer : Dictionary<Type, JsonSerializerContext>
{
    // ReSharper disable InconsistentNaming
    [JsonSerializable(typeof(Dictionary<string, string>))]
    public partial class Dictionary_string_string_SerializerContext : JsonSerializerContext;
    
    [JsonSerializable(typeof(Dictionary<string, Dictionary<string, string>>))]
    public partial class Dictionary_string_Dictionary_string_string_SerializerContext : JsonSerializerContext;
    
    [JsonSerializable(typeof(Dictionary<string, Dictionary<string, Dictionary<string, string>>>))]
    public partial class Dictionary_string_Dictionary_string_Dictionary_string_string_SerializerContext : JsonSerializerContext;

    
    public static Serializer Default { get; } = new ();

    // ReSharper disable once UnusedMethodReturnValue.Global
    public bool CheckForSerializationContext<T>(bool showWarnings)
    {
        if (ContainsKey(typeof(T)))
            return true;

        if (!showWarnings)
            return false;

        var warning =
            $"P42.Utils.Serializer.Default does not have a pre-set SerializationContext for type [{typeof(T)}]. Adding one will improve application performance.";
        Console.WriteLine(warning);
        System.Diagnostics.Debug.WriteLine(warning);
        Serilog.QuickLog.QLog.Warning(warning);
        return false;
    }


    public Serializer()
    {
        Add(typeof(Dictionary<string, string>), Dictionary_string_string_SerializerContext.Default);
        Add(typeof(Dictionary<string, Dictionary<string, string>>), Dictionary_string_Dictionary_string_string_SerializerContext.Default);
        Add(typeof(Dictionary<string, Dictionary<string, Dictionary<string, string>>>), Dictionary_string_Dictionary_string_Dictionary_string_string_SerializerContext.Default);
    }
    
    public T? Deserialize<T>(string json) //where T : class
    {
        if (!TryGetValue(typeof(T), out var context))
            throw new UnregisteredJsonSerializerContextException(typeof(T));
        
        var result = System.Text.Json.JsonSerializer.Deserialize(json, typeof(T), context);
        if (result is T typeResult)
            return typeResult;
        
        return default;
    }
    
    

    public bool TryDeserialize<T>(string json, [MaybeNullWhen(false)] out T result) 
    {
        #if DEBUG
        CheckForSerializationContext<T>(true);
        #endif
        
        try
        {
            result = Deserialize<T>(json);
            return result != null;
        }
        catch (Exception)
        {
            result = default;
            return false;
        }
    }

    public class UnregisteredJsonSerializerContextException(Type type)
        : Exception($"No registered SerializerContext for {type}");
    
    //public class SerializedAsNullException(Type type, string json) 
    //    : Exception($"JsonSerializer for {type} does not support null result.  JSON = {json}");
    
    
}

