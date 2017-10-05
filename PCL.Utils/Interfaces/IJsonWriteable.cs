using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace PCL.Utils
{
    public interface IJsonWriteable
    {
        void WriteValue(JsonWriter writer);
    }
}
