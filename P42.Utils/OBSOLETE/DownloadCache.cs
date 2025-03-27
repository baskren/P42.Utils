using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P42.Utils;

[Obsolete("Use P42.Utils.LocalData and it's reference derivatives (.FileInfo, .DirectoryInfo, .Text, .Stream, .StreamReader, or .StreamWriter), instead", true)]
public static class DownloadCache
{

    [Obsolete("Use .TryStoreItem(), .RecallOrLoadUriAsync(), or .RecallOrLoadUriAsync() in P42.Utils.LocalData's format derivatives (.FileInfo, .DirectoryInfo, .Text, .Stream, .StreamReader, or .StreamWriter), instead", true)]
    public static string Download(string url, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.List(), instead", true)]
    public static List<string> List(string folderName)
        => throw new NotImplementedException();


    [Obsolete("Use .TryStoreItem(), .RecallOrLoadUriAsync(), or .RecallOrLoadUriAsync() in P42.Utils.LocalData's format derivatives (.FileInfo, .DirectoryInfo, .Text, .Stream, .StreamReader, or .StreamWriter), instead", true)]
    public static Task<string> DownloadAsync(string url, string? folderName = null)
        => throw new NotImplementedException();


    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead", true)]
    public static bool IsCached(string url, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(string? url = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(TimeSpan timeSpan, string? url = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(DateTime dateTime, string? url = null, string? folderName = null)
        => throw new NotImplementedException();
    
}
