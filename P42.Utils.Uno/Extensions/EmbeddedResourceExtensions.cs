using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno
{
    /// <summary>
    /// Embedded Resource extension methods
    /// </summary>
    public static class EmbeddedResourceExtensions
    {
        static readonly Dictionary<Assembly, string[]> _embeddedResourceNames = new Dictionary<Assembly, string[]>();

        /// <summary>
        /// Finds the assembly that contains an embedded resource matching the resourceId
        /// </summary>
        /// <param name="resourceId"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Assembly FindAssemblyForResourceId(string resourceId, Assembly assembly = null)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                return null;

            if (assembly == null)
            {
                if (resourceId[0] != '.')
                {
                    if (resourceId.IndexOf(".Resources.", StringComparison.Ordinal) is int index && index > 0)
                    {
                        var assemblyName = resourceId.Substring(0, index);
                        if (ReflectionExtensions.GetAssemblyByName(assemblyName) is Assembly asmA
                            && FindAssemblyForResourceId(resourceId, asmA) is Assembly)
                            return asmA;
                    }
                }
                assembly = Application.Current.GetType().Assembly;
                if (FindAssemblyForResourceId(resourceId, assembly) is Assembly)
                    return assembly;
                var assemblies = ReflectionExtensions.GetAssemblies();
                foreach (var asmB in assemblies)
                {
                    if (!asmB.IsDynamic && FindAssemblyForResourceId(resourceId, asmB) is Assembly)
                        return asmB;
                }
            }
            else if (resourceId[0] == '.')
            {
                if (_embeddedResourceNames.TryGetValue(assembly, out string[] names))
                    return names.Any(n => n.EndsWith(resourceId)) ? assembly : null;
                names = assembly.GetManifestResourceNames();
                if (names != null)
                {
                    _embeddedResourceNames[assembly] = names;
                    return names.Any(n => n.EndsWith(resourceId)) ? assembly : null;
                }
            }
            else
            {

                if (_embeddedResourceNames.TryGetValue(assembly, out string[] names))
                    return names.Contains(resourceId) ? assembly : null;
                names = assembly.GetManifestResourceNames();
                if (names != null)
                {
                    _embeddedResourceNames[assembly] = names;
                    return names.Contains(resourceId) ? assembly : null;
                }
            }
            return null;
        }

        public static Stream FindStreamForResourceId(string resourceId, Assembly assembly = null)
        {
            assembly = assembly ?? FindAssemblyForResourceId(resourceId);

            if (assembly == null)
                return null;

            if (!(_embeddedResourceNames.TryGetValue(assembly, out string[] names)))
            {
                names = assembly.GetManifestResourceNames();
                _embeddedResourceNames[assembly] = names;
            }

            if (names == null || !names.Any())
                return null;

            string name = string.Empty;
            if (resourceId[0] == '.')
                name = names.FirstOrDefault(n => n.EndsWith(resourceId));
            else
                name = names.FirstOrDefault(n => n == resourceId);

            if (string.IsNullOrEmpty(name))
                return null;

            return assembly.GetManifestResourceStream(name);
        }
    }
}
