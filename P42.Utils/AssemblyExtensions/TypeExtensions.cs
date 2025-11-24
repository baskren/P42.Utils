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

    /// <param name="from"></param>
    extension(Type from)
    {
        /// <summary>
        /// Can type From be cast to type To?
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public bool IsCastableTo(Type to)
        {
            if (to.IsAssignableFrom(from))
                return true;
        
            if (TypeMaps.TryGetValue(to, out var value) && value.Contains(from))
                return true;
        
            var castable = from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Any(m => m.ReturnType == to && m.Name is "op_Implicit" or "op_Explicit");
        
            return castable;
        }

        /// <summary>
        /// Can type From be cast to type T?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsCastableTo<T>()
            => IsCastableTo(from, typeof(T));

        /// <summary>
        /// A qualified class name, consistent between platforms
        /// </summary>
        /// <returns></returns>
        [Obsolete("This should be obsolete in newer versions of .NET.  ")]
        public string SimpleQualifiedTypeName()
        {
            var result = new StringBuilder($"{from.Namespace}.{from.Name}");

            if (from.GetTypeInfo().IsGenericType)
            {
                var genericParameters = from.GenericTypeArguments;
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

            result.Append($",{from.Assembly.Name()}");
            return result.ToString();
        }
    }
}
