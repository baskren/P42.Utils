using System;
using Newtonsoft.Json;
namespace PCL.Utils
{
	public interface IJsonWriteable
	{
		void WriteValue(JsonWriter writer);
	}
}
