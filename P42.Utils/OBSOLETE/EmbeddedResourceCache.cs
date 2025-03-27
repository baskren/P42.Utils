using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Linq;

namespace P42.Utils;

[Obsolete("Use P42.Utils.LocalData and it's reference derivatives (.FileInfo, .DirectoryInfo, .Text, .Stream, .StreamReader, or .StreamWriter), instead", true)]
public static class EmbeddedResourceCache
{ 

    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead", true)]
    public static string FolderPath(Assembly assembly, string? folderName = null)
        => throw new NotImplementedException();


    [Obsolete("Use P42.Utils.LocalData.List() instead", true)]
    public static List<string> List(Assembly assembly, string folderName)
    {
        var folderPath = FolderPath(assembly, folderName);
        var files = Directory.EnumerateFiles(folderPath);
        return files.ToList();
    }


    [Obsolete("Use 42.Utils.LocalData.Stream.TryRecallItem(), RecallOrLoadUriItem(), or .RecallOrPullUriItemAsync(), instead", true)]
    public static Stream GetStream(string resourceId, Assembly assembly, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use 42.Utils.LocalData.Stream.TryRecallItem(), RecallOrLoadUriItem(), or .RecallOrPullUriItemAsync(), instead", true)]
    public static Task<Stream> GetStreamAsync(string resourceId, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use .TryGetAppDataUrl() or .LocalDataStoreToAppDataUrl() P42.Utils.LocalData, instead.")]
    public static string ApplicationUri(string resourceId, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();

    
    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead", true)]
    public static string LocalStorageSubPathForEmbeddedResource(string resourceId, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead", true)]
    public static string LocalStorageFullPathForEmbeddedResource(string resourceId, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead", true)]
    public static Task<string> LocalStorageFullPathForEmbeddedResourceAsync(string resourceId, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(string? resourceId = null, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(TimeSpan timeSpan, string? resourceId = null, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(DateTime dateTime, string? resourceId = null, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();


    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead", true)]
    public static Task<string> LocalStorageSubPathForEmbeddedResourceAsync(string resourceId, Assembly? assembly = null, string? folderName = null)
        => throw new NotImplementedException();
    
}
