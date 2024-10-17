using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Numerics;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class JsonExtensions
{
    public static T? Instantiate<T>(this JsonReader reader) where T : IJsonReadable, new()
    {
        if (reader.TokenType == JsonToken.None)
            reader.Read();

        if (reader.TokenType == JsonToken.Null)
            return default;

        var result = new T();
        result.PropertiesFrom(reader);
        return result;
    }

    public static bool IsSimple(this Type type)
    {
        var info = type.GetTypeInfo();
        return info.IsPrimitive || info.IsEnum || type == typeof(string) || type == typeof(decimal);
    }

    public static List<T> ReadIJsonReadableList<T>(this JsonReader reader) where T : IJsonReadable, new()
    {
        var result = new List<T>();
        reader.Read();

        if (reader.TokenType == JsonToken.Null)
            return result;

        if (reader.TokenType != JsonToken.StartArray)
            throw new InvalidDataContractException("For the time being, all lists are assumed to be within square brackets");

        reader.Read();
        while (reader.TokenType != JsonToken.EndArray)
        {
            var value = new T();
            value.PropertiesFrom(reader);
            result.Add(value);
            reader.Read();
        }
        return result;
    }

    public static List<T> ReadSimpleList<T>(this JsonReader reader)
    {
        List<T> result = new ();
        reader.Read();

        if (reader.TokenType == JsonToken.Null)
            return result;

        if (reader.TokenType != JsonToken.StartArray)
            throw new InvalidDataContractException("For the time being, all lists are assumed to be within square brackets");

        reader.Read();
        while (reader.TokenType != JsonToken.EndArray)
        {
            if (reader.ParseSimple<T>() is { } value)
                result.Add(value);
            else
                QLog.Error($"ReadSimpleList<{typeof(T)}>: Item [{reader.Value}] is not of type {typeof(T)}]");
                
            reader.Read();
        }
        return result;
    }

    /*
    public static List<T> ReadJsonList<T>(this JsonReader reader)
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
                value = reader.ReadEnum<T>();
            else
                value = (T)reader.Value;
            result.Add(value);
            reader.Read();
        }
        return result;
    }
    */

    public static string ReadString(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseString();
    }

    public static string ParseString(this JsonReader reader)
        => reader.Value?.ToString() ?? string.Empty;

    public static int ReadInt(this JsonReader reader)
        => (int)ReadLong(reader);

    public static int ParseInt(this JsonReader reader)
        => (int)ParseLong(reader);
        
    public static long ReadLong(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseLong();
    }

    public static long ParseLong(this JsonReader reader)
    {
        return reader switch
        {
            { TokenType: JsonToken.Integer, Value: long value } => value,
            { TokenType: JsonToken.String, Value: string s } => long.Parse(s),
            { TokenType: JsonToken.Float, Value: float f } => (long)f,
            _ => throw new InvalidCastException($"Cannot convert [{reader.TokenType}] : [{reader.Value}] to long.")
        };
    } 

    public static bool ReadBool(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseBool();
    }

    public static bool ParseBool(this JsonReader reader)
        => (bool)(reader.Value ?? false);

    public static double ReadDouble(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseDouble();
    }

    public static double ParseDouble(this JsonReader reader)
    {
        double result;
        switch (reader.TokenType)
        {
            case JsonToken.Boolean:
                if (reader.Value is bool boolVal)
                    result = boolVal ? 1 : 0;
                else
                    result = 0;
                break;
            case JsonToken.Bytes:
                if (reader.Value is byte[] byteArray)
                    result = BitConverter.ToDouble(byteArray, 0);
                else
                    result = 0;
                break;
            case JsonToken.Float:
                if (reader.Value is double doubleVal)
                    result = doubleVal;
                else
                    result = 0;
                break;
            case JsonToken.Integer:
                if (reader.Value is long intVal)
                    result = intVal;
                else
                    result = 0;
                break;
            case JsonToken.String:
                if (reader.Value is string stringVal)
                {
                    if (!double.TryParse(stringVal, out doubleVal))
                        throw new InvalidDataContractException("Could not parse what JSON thinks is a double.");
                    result = doubleVal;
                }
                else
                    result = 0;
                break;
            default:
                throw new InvalidDataContractException(
                    $"Found JSON invalid value token [{reader.Value}] of type [{reader.TokenType}]");
        }
        return result;
    }

    public static double? ReadNullableDouble(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseNullableDouble();
    }

    public static double? ParseNullableDouble(this JsonReader reader)
    {
        if (reader.TokenType == JsonToken.Null || reader.Value is null)
            return null;
        return ParseDouble(reader);
    }

    public static DateTime ReadDateTime(this JsonReader reader)
    {
        if (reader.ReadAsDateTime() is { } dateTime)
            return dateTime;
        throw new InvalidDataContractException($"Unable to reader DateTime from [{reader.Value}]");
    }

    public static DateTime ParseDateTime(this JsonReader reader)
    {
        if (reader.TokenType != JsonToken.Date)
            throw new InvalidDataContractException($"Expecting JsonToken.Date but found [{reader.TokenType}]");
            
        if (reader.Value is DateTime dateTime)
            return dateTime;
            
        throw new InvalidDataContractException($"Value [{reader.Value}] for JsonToken.Date not DateTime");
    }

    public static T? ParseSimple<T>(this JsonReader reader)
    {
        if (reader.ValueType is T valueT)
            return valueT;
            
        T? result = default;
        var typeT = typeof(T);

        if (typeT == typeof(bool))
        {
            if (reader.Value is bool boolVal)
                result = (T)(object)boolVal;
            else
                QLog.Error($"ParseSimple<{typeof(T)}>: Value [{reader.Value}] is not a {typeT}.");
        }
        else if (typeT.GetTypeInfo().IsEnum)
            return reader.ParseEnum<T>();
        else if (typeT == typeof(string))
        {
            if (reader.ParseString() is { } stringVal)
                result = (T)(object)stringVal;
            else
                QLog.Error($"ParseSimple<{typeof(T)}>: Value [{reader.Value}] is not a {typeT}.");
        }
        else if (typeT == typeof(int))
        {
            var obj = (object)Convert.ToInt32(reader.Value);
            result = (T)obj;
        }
        else if (typeT == typeof(long))
        {
            var val = reader.ParseLong();
            result = (T)(object)val;
        }
        else if (typeT == typeof(double))
        {
            var val = reader.ParseDouble();
            result = (T)(object)val;
        }
        else if (typeT == typeof(DateTime))
        {
            var val = reader.ParseDateTime();
            result = (T)(object)val;
        }
        else
            throw new InvalidDataContractException("Type [" + typeT + "] is not supported by ReadJsonSimple [" + reader.ValueType + "] [" + reader.Value + "]");
        
        return result;
    }



    public static T ReadEnum<T>(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseEnum<T>();
    }

    public static T ParseEnum<T>(this JsonReader reader)
    {
        var result = (T)Enum.ToObject(typeof(T), -1);
        //var value = reader.Value;
        switch (reader.TokenType)
        {
            case JsonToken.Integer:
                if (reader.Value is int intVal)
                    result = (T)Enum.ToObject(typeof(T), intVal);
                else
                    QLog.Error($"ParseEnum<{typeof(T)}>: Value [{reader.Value}] is not a {reader.TokenType}.");
                break;
            case JsonToken.String:
                try
                {
                    if (reader.Value is string stringVal)
                        result = (T)Enum.Parse(typeof(T), stringVal);
                    else
                        QLog.Error($"ParseEnum<{typeof(T)}>: Value [{reader.Value}] is not a {reader.TokenType}.");
                }
                catch (Exception ex)
                {
                    QLog.Error(ex);
                }

                break;
            default:
                throw new Exception($"Invalid JSON token type [{reader.TokenType}] for Enum type [{typeof(T)}] and value [{reader.Value}]");
        }
        
        return result;
    }
    /*
    public static T ReadIJsonReadable<T>(this JsonReader reader) where T : IJsonReadable<T>, new()
    {
        reader.Read();
        return reader.ParseIJsonReadable<T>();
    }
    */

    public static T ParseIJsonReadable<T>(this JsonReader reader) where T : IJsonReadable<T>, new()
    {
        var result = new T();
        result.PropertiesFrom(reader);
        return result;
    }

    public static void WriteSafePropertyName(this JsonWriter writer, string name)
    {
        writer.WritePropertyName(name.ReplaceIllegalCharacters());
    }

    public static void WriteList<T>(this JsonWriter writer, IEnumerable<T> list, bool justValues = false)
    {
        writer.WriteStartArray();
        foreach (var item in list)
        {
            if (item is IJsonWriteable jsonWriteable)
                jsonWriteable.WriteValue(writer, justValues);
            else
                writer.WriteValue(item);
        }

        writer.WriteEnd();
    }



    /*
    public static void WritePvPair(this JsonWriter writer, string name, bool value)
    {
        if (value)
        {
            writer.WriteSafePropertyName(name);
            writer.WriteValue(value);
        }
    }

    public static void WritePvPair(this JsonWriter writer, string name, int value)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, double value)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            writer.WriteSafePropertyName(name);
            writer.WriteValue(value);
        }
    }

    public static void WritePvPair(this JsonWriter writer, string name, DateTime value)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }
    */



    public static void WritePvPair(this JsonWriter writer, string name, BigInteger value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, BigInteger? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, bool value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, bool? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, byte value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, byte? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, byte[] value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, char value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, char? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, DateTime value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, DateTime? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, DateTimeOffset value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, DateTimeOffset? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, decimal value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, decimal? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, double value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, double? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, float value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, float? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, Guid value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, Guid? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, int value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, int? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, long value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, long? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, sbyte value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, sbyte? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, short value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, short? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, string value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, TimeSpan value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, TimeSpan? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, ushort value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, ushort? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, uint value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, uint? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, ulong value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, ulong? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    public static void WritePvPair(this JsonWriter writer, string name, Uri value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }
        
    public static void WritePvPair(this JsonWriter writer, string name, IJsonWriteable value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        value.WriteValue(writer, justValues);
    }

    public static void WritePvPair<T>(this JsonWriter writer, string name, IEnumerable<T> enumerable, bool justValues = false)
    {
        if (enumerable is ICollection<T> { Count: 0 })
            return;
            
        writer.WriteSafePropertyName(name);
        writer.WriteList(enumerable, justValues);
    }
}
