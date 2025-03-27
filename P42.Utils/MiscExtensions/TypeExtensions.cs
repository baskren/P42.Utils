using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace P42.Utils;

public static class TypeExtensions
{
    private static readonly Dictionary<Type, List<Type>> TypeMaps = new() {
        { typeof(decimal),
            [
                typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
                typeof(ulong), typeof(char)
            ]
        },
        { typeof(double),
            [
                typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
                typeof(ulong), typeof(char), typeof(float)
            ]
        },
        { typeof(float),
            [
                typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long),
                typeof(ulong), typeof(char), typeof(float)
            ]
        },
        { typeof(ulong), [typeof(byte), typeof(ushort), typeof(uint), typeof(char)] },
        { typeof(long),
            [typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char)]
        },
        { typeof(uint), [typeof(byte), typeof(ushort), typeof(char)] },
        { typeof(int), [typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(char)] },
        { typeof(ushort), [typeof(byte), typeof(char)] },
        { typeof(short), [typeof(byte)] }
    };

    /// <summary>
    /// Can type From be cast to type To?
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static bool IsCastableTo(this Type from, Type to)
    {
        if (to.IsAssignableFrom(from))
            return true;
        
        if (TypeMaps.ContainsKey(to) && TypeMaps[to].Contains(from))
            return true;
        
        var castable = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Any(m => m.ReturnType == to && m.Name is "op_Implicit" or "op_Explicit");
        
        return castable;
    }
    
    /// <summary>
    /// A qualified class name, consistent between platforms
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [Obsolete("This should be obsolete in newer versions of .NET.  ")]
    public static string SimpleQualifiedTypeName(this Type type)
    {
        var result = new StringBuilder($"{type.Namespace}.{type.Name}");

        if (type.GetTypeInfo().IsGenericType)
        {
            var genericParameters = type.GenericTypeArguments;
            result.Append('[');
            for (var i = 0; i < genericParameters.Length; i++)
            {
                var parameter = genericParameters[i];
                if (i > 0)
                    result.Append(',');
                result.Append($"[{SimpleQualifiedTypeName(parameter)}]");
            }
            result.Append(']');
        }

        result.Append($",{type.Assembly.Name()}");
        return result.ToString();
    }


}
