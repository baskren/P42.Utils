using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Numerics;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// Extensions for serializing/deserializing JSON
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Is the object type a primitive, an enum, a string, a decimal, or a DateTime?
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsSimple(this Type type)
    {
        var info = type.GetTypeInfo();
        return info.IsPrimitive || info.IsEnum || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime);
    }

    #region Reading / Parsing
    /// <summary>
    /// Deserialize a T, using properties in JsonReader
    /// </summary>
    /// <param name="reader">JsonReader</param>
    /// <typeparam name="T">IJsonReadable</typeparam>
    /// <returns>default if JsonToken is null</returns>
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
    
    /// <summary>
    /// Deserialize a list of T
    /// </summary>
    /// <param name="reader">JsonReader</param>
    /// <typeparam name="T">IJsonReadable</typeparam>
    /// <returns>null if JsonToken is null</returns>
    /// <exception cref="InvalidDataContractException"></exception>
    public static List<T>? ReadIJsonReadableList<T>(this JsonReader reader) where T : IJsonReadable, new()
    {
        var result = new List<T>();
        reader.Read();

        if (reader.TokenType == JsonToken.Null)
            return null;

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

    /// <summary>
    /// Deserialize list of JsonExtensions.IsSimple() items
    /// </summary>
    /// <param name="reader">JsonReader</param>
    /// <typeparam name="T">Where IsSimple() returns true</typeparam>
    /// <returns>null if JsonToken is null</returns>
    /// <exception cref="InvalidDataContractException"></exception>
    public static List<T>? ReadSimpleList<T>(this JsonReader reader)
    {
        var result = new List<T>();
        reader.Read();

        if (reader.TokenType == JsonToken.Null)
            return null;

        if (reader.TokenType != JsonToken.StartArray)
            throw new InvalidDataContractException("For the time being, all lists are assumed to be within square brackets");

        reader.Read();
        while (reader.TokenType != JsonToken.EndArray)
        {
            var value = reader.ParseSimple<T>();
            result.Add(value);
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

    /// <summary>
    /// Read string from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>string</returns>
    public static string? ReadString(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseString();
    }

    /// <summary>
    /// casts the current reader value as a string
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>null if not string</returns>
    public static string? ParseString(this JsonReader reader)
         => reader.TokenType == JsonToken.String
                ? reader.Value as string
                : null;
    
    /// <summary>
    /// Read int from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static int ReadInt(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseInt();
    }

    /// <summary>
    /// Casts the current reader value as int
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>0 if reader.Value cannot be cast to int</returns>
    public static int ParseInt(this JsonReader reader)
        => (int)ParseLong(reader);
    
    /// <summary>
    /// Read long from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static long ReadLong(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseLong();
    }

    /// <summary>
    /// Casts current value to long
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>0 if reader.Value cannot be cast to long</returns>
    public static long ParseLong(this JsonReader reader)
        => reader.TokenType == JsonToken.Integer
            ? (long)(reader.Value ?? 0)
            : 0;
    

    /// <summary>
    /// Read bool from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static bool ReadBool(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseBool();
    }

    /// <summary>
    /// Casts current value to bool
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>false if reader.TokenType is not Boolean</returns>
    public static bool ParseBool(this JsonReader reader)
        => reader.TokenType == JsonToken.Boolean && (bool)(reader.Value ?? false);
    

    /// <summary>
    /// Read double from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static double ReadDouble(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseDouble();
    }


    /// <summary>
    /// Casts current value to double
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataContractException"></exception>
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

    /// <summary>
    /// Read double? from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static double? ReadNullableDouble(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseNullableDouble();
    }

    /// <summary>
    /// Casts reader.Value to double or returns null
    /// </summary>
    /// <param name="reader"></param>
    /// <returns>null if reader.Value cannot be cast to double</returns>
    public static double? ParseNullableDouble(this JsonReader reader)
    {
        if (reader.TokenType == JsonToken.Null || reader.Value is null)
            return null;
        try
        {
            return ParseDouble(reader);
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Read DateTime from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataContractException"></exception>
    public static DateTime ReadDateTime(this JsonReader reader)
    {
        //reader.Read();
        //return reader.ParseDateTime();
        var nullDateTime = reader.ReadAsDateTime();
        if (nullDateTime == null)
            throw new InvalidDataContractException($"Unable to reader DateTime from [{reader.Value}]");
        return nullDateTime.Value;
    }

    /// <summary>
    /// Casts reader.Value to DateTime
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataContractException"></exception>
    public static DateTime ParseDateTime(this JsonReader reader)
    {
        if (reader.TokenType != JsonToken.Date)
            throw new InvalidDataContractException($"Expecting JsonToken.Date but found [{reader.TokenType}]");
        return (DateTime)(reader.Value ?? DateTime.MinValue);
    }

    /// <summary>
    /// Casts reader.Value to T, where IsSimpleType(typeof(T)) is true
    /// </summary>
    /// <param name="reader"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>default(T) or string.Empty (if string), if cast fails</returns>
    /// <exception cref="InvalidDataContractException"></exception>
    public static T ParseSimple<T>(this JsonReader reader)
    {
        T result;
        var typeT = typeof(T);

        if (typeT == typeof(bool))
            result = (T)(object)ParseBool(reader);
        else if (typeT.GetTypeInfo().IsEnum)
            return reader.ParseEnum<T>();
        else if (typeT == typeof(string))
            result = (T)(reader.Value ?? string.Empty);
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
            throw new InvalidDataContractException(
                $"Type [{typeT}] is not supported by ReadJsonSimple [{reader.ValueType}] [{reader.Value}]");
        return result;
    }


    /// <summary>
    /// Read enum (of type T) from JsonReader
    /// </summary>
    /// <param name="reader"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ReadEnum<T>(this JsonReader reader)
    {
        reader.Read();
        return reader.ParseEnum<T>();
    }

    /// <summary>
    /// Casts reader.Value to enum of type(T)
    /// </summary>
    /// <param name="reader"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns>default if cast fails</returns>
    /// <exception cref="Exception"></exception>
    public static T ParseEnum<T>(this JsonReader reader)
    {
        var result = (T)Enum.ToObject(typeof(T), -1);
        if (reader.TokenType == JsonToken.Integer)
            result = (T)Enum.ToObject(typeof(T), reader.Value ?? 0);
        else if (reader.TokenType == JsonToken.String)
        {
            try
            {
                var s = ParseString(reader);
                if (string.IsNullOrEmpty(s))
                    result = (T)Enum.ToObject(typeof(T), reader.Value ?? 0);
                else
                    result = (T)Enum.Parse(typeof(T), s);
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }
        }
        else
            throw new Exception($"Invalid JSON token type [{reader.TokenType}] for Enum type [{typeof(T)}]");
        return result;
    }
    /*
    public static T ReadIJsonReadable<T>(this JsonReader reader) where T : IJsonReadable<T>, new()
    {
        reader.Read();
        return reader.ParseIJsonReadable<T>();
    }
    */

    /// <summary>
    /// Casts reader.Value to IJsonReadable T
    /// </summary>
    /// <param name="reader"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T ParseIJsonReadable<T>(this JsonReader reader) where T : IJsonReadable<T>, new()
    {
        var result = new T();
        result.PropertiesFrom(reader);
        return result;
    }
    #endregion

    
    #region Writing
    /// <summary>
    /// Write PropertyName replacing any characters that are OS and Json unsafe
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    public static void WriteSafePropertyName(this JsonWriter writer, string name)
        => writer.WritePropertyName(name.ReplaceIllegalCharacters());
    


    private static readonly TypeInfo IJsonWriteableTypeInfo = typeof(IJsonWriteable).GetTypeInfo();
    /// <summary>
    /// Write list to JsonWriter
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="list"></param>
    /// <param name="justValues"></param>
    /// <typeparam name="T"></typeparam>
    public static void WriteList<T>(this JsonWriter writer, IEnumerable<T> list, bool justValues = false)
    {
        writer.WriteStartArray();
        foreach (var item in list)
        {
            if (item is IJsonWriteable writeable && IJsonWriteableTypeInfo.IsAssignableFrom(typeof(T).GetTypeInfo()))
                writeable.WriteValue(writer, justValues);
            else
                writer.WriteValue(item);
        }        
        writer.WriteEnd();
    }
    

    // ReSharper disable UnusedParameter.Global
    
    /// <summary>
    /// Write BigInteger Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, BigInteger value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write BigInteger? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, BigInteger? value, bool justValues = false)
    {
        if (!value.HasValue && justValues)
            return;
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write bool Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, bool value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write bool? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, bool? value, bool justValues = false)
    {
        if (!value.HasValue && justValues)
            return;
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write byte Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, byte value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write byte? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, byte? value, bool justValues = false)
    {
        if (!value.HasValue && justValues)
            return;
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write byte[] Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, byte[] value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write char Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, char value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write char? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, char? value, bool justValues = false)
    {
        if (!value.HasValue && justValues)
            return;
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write DateTime Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, DateTime value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write DateTime? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, DateTime? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write DateTimeOffset Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, DateTimeOffset value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write DateTimeOffset? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, DateTimeOffset? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write decimal Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, decimal value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write decimal? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, decimal? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write double Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, double value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write double? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, double? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write float Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, float value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write float? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, float? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write Guid Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, Guid value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write Guid? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, Guid? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write int Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, int value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write int? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, int? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write long Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, long value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write long? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, long? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write sbyte Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, sbyte value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write sbyte? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, sbyte? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write short Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, short value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write short? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, short? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write string Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, string? value, bool justValues = false)
    {
        if (value is null && justValues)
            return;
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }
    
    /// <summary>
    /// Write TimeSpan Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, TimeSpan value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write TimeSpan? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, TimeSpan? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write ushort Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, ushort value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write ushort? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, ushort? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write uint Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, uint value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write uint? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, uint? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write ulong Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, ulong value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write ulong? Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, ulong? value, bool justValues = false)
    {
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }

    /// <summary>
    /// Write Uri Property key and value
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="justValues"></param>
    public static void WritePvPair(this JsonWriter writer, string name, Uri? value, bool justValues = false)
    {
        if (value is null && justValues)
            return;
        writer.WriteSafePropertyName(name);
        writer.WriteValue(value);
    }


    public static void WritePvPair(this JsonWriter writer, string name, IJsonWriteable value, bool justValues = false)
    {
        //if (value == null)
        //    return;

        writer.WriteSafePropertyName(name);
        value.WriteValue(writer, justValues);
    }

    public static void WritePvPair<T>(this JsonWriter writer, string name, IEnumerable<T> enumerable, bool justValues = false)
    {
        //if (enumerable == null)
        //    return;

        if (enumerable is ICollection<T> { Count: 0 })
            return;
        writer.WriteSafePropertyName(name);
        writer.WriteList(enumerable, justValues);
    }
    
    
    // ReSharper restore UnusedParameter.Global
    #endregion
    
    
}
