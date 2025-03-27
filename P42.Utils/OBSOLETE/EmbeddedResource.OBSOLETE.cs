using System;
using System.Collections.Generic;
using System.Reflection;

namespace P42.Utils;

// DO NOT MAKE PUBLIC.  THIS RUNS VERY SLOWLY ON UWP .NET TOOL CHAIN.  USE EmbeddedResourceCache.GetStreamAsync instead.
internal static class EmbeddedResource
{
    [Obsolete("Use EmbeddedResourceExtensions.FindStreamForResourceId instead")]
    public static System.IO.Stream? GetStream(string resourceId, Assembly? assembly = null)
        => throw new NotImplementedException();

    [Obsolete("Use EmbeddedResourceExtensions.EmbeddedResourceExists instead")]
    public static bool Available(string resourceId, Assembly? assembly=null)
        => throw new NotImplementedException();
    
}
