using System;
using System.IO;
using System.Reflection;

namespace P42.Utils;

[Obsolete("Use P42.Utils.LocalData, instead.")]
public enum AppResource
{
    Default,
    EmbeddedResource,
    Local,
    None
}

// ReSharper disable once UnusedType.Global
[Obsolete("Use P42.Utils.LocalData instead.")]
public static class AppResourceExtensions
{
    [Obsolete("Use .Exists or .IsSourceAvailableAsync() in P42.Utils.LocalData.ItemKey, instead", true)]
    public static bool Exists(Assembly assembly, string folder, string resourceName, AppResource source = AppResource.Default)
    {
        var result = false;
        if (!result && source is AppResource.Default or AppResource.Local)
            result = AppLocalStorage.Exists(assembly, folder, resourceName);
        if (!result && source is AppResource.Default or AppResource.EmbeddedResource)
            result = assembly.Exists(resourceName);
        return result;
    }

    [Obsolete("Try to use P42.Utils.LocalData.StreamReader.TryRecallItem(), P42.Utils.LocalData.StreamReader.RecallOrPullUriItemAsync() or P42.Utils.LocalData.StreamReader.RecallOrPullResourceItemAsync() instead")]
    public static StreamReader? StreamReader(Assembly assembly, string folder, string resourceName, AppResource source = AppResource.Default)
    {
        StreamReader? reader = null;
        if (source is AppResource.Default or AppResource.Local)
            reader = AppLocalStorage.StreamReader(assembly, folder, resourceName);
        if (reader == null && source is AppResource.Default or AppResource.EmbeddedResource)
            reader = EmbeddedResourceExtensions.FindStreamReader(resourceName, assembly);
        return reader;
    }
    
    
}
