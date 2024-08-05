using Newtonsoft.Json;
namespace P42.Utils
{
    public interface IJsonWriteable
    {
        void WriteValue(JsonWriter writer, bool justValues=false);
    }
}
