using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.Serialization;

namespace PCL.Utils
{
	public static class JsonExtensions
	{
		public static bool IsSimple(this Type type)
		{
			var info = type.GetTypeInfo();
			return info.IsPrimitive || info.IsEnum || type.Equals(typeof(string)) || type.Equals(typeof(decimal));
		}

		public static List<T> ParseListIJsonReadableProperty<T>(this JsonReader reader) where T: IJsonReadable
		{
			var result = new List<T>();
			T value;
			reader.Read();
			if (reader.TokenType != JsonToken.StartArray)
				throw new InvalidDataContractException("For the time being, all lists are assumed to be within square brackets");
			
			reader.Read();
			while (reader.TokenType != JsonToken.EndArray)
			{
				value = Activator.CreateInstance<T>();
				value.ValueFrom(reader);
				result.Add(value);
				reader.Read();
			}
			return result;
		}

		public static List<T> ParseListSimpleProperty<T>(this JsonReader reader) 
		{
			var result = new List<T>();
			T value;
			reader.Read();
			if (reader.TokenType != JsonToken.StartArray)
				throw new InvalidDataContractException("For the time being, all lists are assumed to be within square brackets");
			reader.Read();
			while (reader.TokenType != JsonToken.EndArray)
			{
				value = (T)reader.Value;
				result.Add(value);
				reader.Read();
			}
			return result;
		}

		public static string ParseStringProperty(this JsonReader reader)
		{
			reader.Read();
			return reader.Value as string;
		}

		public static int ParseIntProperty(this JsonReader reader)
		{
			reader.Read();
			return (int)((long)reader.Value);
		}

		public static bool ParseBoolProperty(this JsonReader reader)
		{
			reader.Read();
			return (bool)reader.Value;
		}

		public static double ParseDoubleProperty(this JsonReader reader)
		{
			double result;
			reader.Read();
			switch (reader.TokenType)
			{
				case JsonToken.Boolean:
					var boolVal = (bool)reader.Value;
					result = boolVal ? 1 : 0;
					break;
				case JsonToken.Bytes:
					var bytesVal = (byte[])reader.Value;
					result = BitConverter.ToDouble(bytesVal, 0);
					break;
				case JsonToken.Float:
					var doubleVal = (double)reader.Value;
					result = doubleVal;
					break;
				case JsonToken.Integer:
					var intVal = (double)((long)reader.Value);
					result = intVal;
					break;
				case JsonToken.String:
					var stringVal = (string)reader.Value;
					if (!double.TryParse(stringVal, out doubleVal))
						throw new InvalidDataContractException("Could not parse what JSON thinks is a double.");
					result = doubleVal;
					break;
				default:
					throw new InvalidDataContractException("Found JSON invalid value token [" + reader.Value + "] of type [" + reader.TokenType + "]");
			}
			return result;
		}

	}


}
