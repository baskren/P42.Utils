using System.Collections.Generic;
using System.Reflection;

namespace P42.Utils;

// DO NOT MAKE PUBLIC.  THIS RUNS VERY SLOWLY ON UWP .NET TOOL CHAIN.  USE EmbeddedResourceCache.GetStreamAsync instead.
internal static class EmbeddedResource
{
    public static System.IO.Stream? GetStream(string resourceId, Assembly? assembly = null)
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        return assembly == null 
            ? null 
            : assembly.GetManifestResourceStream(resourceId);
    }

    private static readonly Dictionary<Assembly, List<string>> Resources = new();

    public static bool Available(string resourceId, Assembly? assembly=null)
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly == null)
            return false;
        if (!Resources.ContainsKey(assembly))
            Resources[assembly] = [..assembly.GetManifestResourceNames()];
        return Resources[assembly].Contains(resourceId);
    }
}
