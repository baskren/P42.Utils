using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

static class ReflectionExtentions
{
    public static List<MethodInfo> GetMethodsWithAttribute(this Type type, Type attributeType)
        => (
            from method in type.GetMethods()
            where method.GetCustomAttribute(attributeType) != null
            select method
        ).ToList();

    public static List<MethodInfo> GetMethodsWithAttribute<T>(this Type type)
        => GetMethodsWithAttribute(type, typeof(T));

    public static List<Type> GetTypesWithAttribute(this Assembly assembly, Type attributeType)
        =>
        (from type in assembly.GetTypes()
         where type.GetCustomAttribute(attributeType) != null
         select type)
        .ToList();

    public static List<Type> GetTypesWithAttribute<T>(this Assembly asm)
        => GetTypesWithAttribute((Assembly)asm, typeof(T));
}
