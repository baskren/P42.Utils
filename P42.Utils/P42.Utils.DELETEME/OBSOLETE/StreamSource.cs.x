using System;

namespace P42.Utils;

/// <summary>
/// Source for stream input
/// </summary>
[Obsolete("Use P42.Utils.AppResource instead.")]
public enum StreamSource
{
    Default,
    EmbeddedResource,
    Local,
    None,
}
    
