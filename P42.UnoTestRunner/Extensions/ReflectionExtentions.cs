using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

static class ReflectionExtentions
{
    extension(Type type)
    {
        public List<MethodInfo> GetMethodsWithAttribute(Type attributeType)
            => type.GetMethods().Where(m => m.GetCustomAttribute(attributeType) != null).ToList();

        public List<MethodInfo> GetMethodsWithAttribute<T>()
            => GetMethodsWithAttribute(type, typeof(T));
    }
    /*
        => (
            from method in type.GetMethods()
            where method.GetCustomAttribute(attributeType) != null
            select method
        ).ToList();
        */

    extension(Assembly assembly)
    {
        public List<Type> GetTypesWithAttribute(Type attributeType)
            => assembly.GetTypes().Where(t => t.GetCustomAttribute((attributeType)) != null).ToList();

        public List<Type> GetTypesWithAttribute<T>()
            => GetTypesWithAttribute((Assembly)assembly, typeof(T));
    }
        /*
        =>
        (from type in assembly.GetTypes()
         where type.GetCustomAttribute(attributeType) != null
         select type)
        .ToList();
        */
}
