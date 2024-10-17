using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace P42.Utils;

public static class TypeExtensions
{
    private static readonly Dictionary<Type, List<Type>> Dict = new() {
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
        { typeof(ulong),  [typeof(byte), typeof(ushort), typeof(uint), typeof(char)] },
        { typeof(long),   [typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char)] },
        { typeof(uint),   [typeof(byte), typeof(ushort), typeof(char)] },
        { typeof(int),    [typeof(sbyte), typeof(byte), typeof(short), typeof(ushort), typeof(char)] },
        { typeof(ushort), [typeof(byte), typeof(char)] },
        { typeof(short),  [typeof(byte)] }
    };

    public static bool IsCastableTo(this Type from, Type to)
    {
        if (to.IsAssignableFrom(from))
            return true;
        if (Dict.TryGetValue(to, out var value) && value.Contains(from))
            return true;
        
        return from.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Any(
                m => m.ReturnType == to &&
                     (m.Name == "op_Implicit" ||
                      m.Name == "op_Explicit")
            );
        
    }
        
        
    /// <summary>
    /// Comb through assemblies to find classes inherited from parentClass
    /// </summary>
    /// <param name="parentType">Parent class</param>
    /// <returns>child classes</returns>
    public static IEnumerable<Type> GetChildClassesOf(Type parentType)
        => from asm in AssemblyExtensions.GetAssemblies() from type in asm.DefinedTypes where type.IsSubclassOf(parentType) select type.AsType();
        
    /// <summary>
    /// A qualified class name, consistent between platforms
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string SimpleQualifiedTypeName(this Type type)
    {
        var result = new StringBuilder(type.Namespace + "." + type.Name);

        if (type.GetTypeInfo().IsGenericType)
        {
            var genericParameters = type.GenericTypeArguments;
            result.Append("[");
            for (var i = 0; i < genericParameters.Length; i++)
            {
                var parameter = genericParameters[i];
                if (i > 0)
                    result.Append(',');
                result.Append("[" + SimpleQualifiedTypeName(parameter) + "]");
            }
            result.Append(']');
        }

        result.Append("," + type.GetAssembly().GetName().Name);
        return result.ToString();
    }
        
}
