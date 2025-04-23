using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

static class ReflectionExtentions
{
    public static MethodInfo[] GetMethodsWithAttribute(this Type type, Type attributeType)
        => (
            from method in type.GetMethods()
            where method.GetCustomAttribute(attributeType) != null
            select method
        ).ToArray();

    public static MethodInfo[] GetMethodsWithAttribute<T>(this Type type)
        => GetMethodsWithAttribute(type, typeof(T));

}
