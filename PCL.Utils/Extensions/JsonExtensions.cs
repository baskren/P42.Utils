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

		public static bool IsIJsonReadable(this Type type)
		{
			return typeof(IJsonReadable).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
		}

		public static List<T> ParseListIJsonReadableProperty<T>(this JsonReader reader) where T : IJsonReadable
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

		public static List<T> ParseListProperty<T>(this JsonReader reader)
		{
			bool isIJsonReadable = typeof(T).IsIJsonReadable();
			if (!typeof(T).IsSimple())
				throw new InvalidDataContractException("ParseListProperty only works with IJsonReadable or IsSimple types");
			bool isInt = typeof(T) == typeof(int);
			bool isEnum = typeof(T).GetTypeInfo().IsEnum;
			var result = new List<T>();
			T value;
			reader.Read();
			if (reader.TokenType != JsonToken.StartArray)
				throw new InvalidDataContractException("For the time being, all lists are assumed to be within square brackets");
			reader.Read();
			while (reader.TokenType != JsonToken.EndArray)
			{
				if (isIJsonReadable)
				{
					value = Activator.CreateInstance<T>();
					((IJsonReadable)value).ValueFrom(reader);
				}
				else if (isInt)
					value = (T)(object)Convert.ToInt32(reader.Value);
				else if (isEnum)
					value = reader.ParseEnumProperty<T>();
				else
					value = (T)reader.Value;
				result.Add(value);
				reader.Read();
			}
			return result;
		}

		public static string ParseStringProperty(this JsonReader reader)
		{
			reader.Read();
			return (string)reader.Value;
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

		public static T ParseIJsonReadableProperty<T>(this JsonReader reader)
		{
			var result = Activator.CreateInstance<T>();
			reader.Read();
			((IJsonReadable)result).ValueFrom(reader);
			return result;
		}

		public static T ParseIJsonReadableValue<T>(this JsonReader reader)
		{
			T result;
			var typeT = typeof(T);
			if (typeT.IsIJsonReadable())
				result = reader.ParseIJsonReadableProperty<T>();
			else if (typeT == typeof(bool))
			{
				reader.Read();
				result = (T)((object)((bool)reader.Value));
			}
			else if (typeT.GetTypeInfo().IsEnum)
			{
				return reader.ParseEnumProperty<T>();
			}
			else if (typeT == typeof(string))
			{
				reader.Read();
				result = (T)reader.Value;
			}
			else if (typeT == typeof(int))
			{
				reader.Read();
				var obj = (object)Convert.ToInt32(reader.Value);
				result = (T)obj;
			}
			else if (typeT == typeof(double))
			{
				double val = reader.ParseDoubleProperty();
				result = (T)((object)val);
			}
			else
				throw new InvalidDataContractException("Type [" + typeT + "] is not supported by ParseValueProperty [" + reader.ValueType + "] [" + reader.Value + "]");
			return result;
		}

		public static T ParseEnumProperty<T>(this JsonReader reader)
		{
			reader.Read();
			T result;
			if (reader.TokenType == JsonToken.Integer)
				result = (T)Enum.ToObject(typeof(T), reader.Value);
			else
			{
				//var valueStr = reader.ParseStringProperty();
				result = (T)Enum.Parse(typeof(T), (string)reader.Value);
			}
			return result;
		}
	}
}
