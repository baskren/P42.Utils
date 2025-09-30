using System;
using System.Collections.Generic;
using System.IO;

namespace P42.Utils;

[Obsolete("Use P42.Utils.LocalData.Text, instead", true)]
public static class TextCache 
{
    #region OBSOLETE IMPLEMENTATION

    [Obsolete("Use P42.Utils.LocalData.List() instead", true)]
    public static List<string> List(string folderName)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Text.TryStoreItem() or .TryStoreItemAsync() instead", true)]
    public static void Store(string text, string key, string? folderName = null)
        => throw new NotImplementedException();
    

    [Obsolete("Use P42.Utils.LocalData.Text.TryRecallItem(), RecallOrLoadUriItem(), or .RecallOrPullUriItemAsync(), instead", true)]
    public static string Recall(string key, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use 42.Utils.LocalData.StreamReader.TryRecallItem(), RecallOrLoadUriItem(), or .RecallOrPullUriItemAsync(), instead", true)]
    public static StreamReader GetStreamReader(string key, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.TryGetItemPath(), instead", true)]
    public static bool IsCached(string key, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(string? key = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(TimeSpan timeSpan, string? key = null, string? folderName = null)
        => throw new NotImplementedException();

    [Obsolete("Use P42.Utils.LocalData.Clear(), instead", true)]
    public static bool Clear(DateTime dateTime, string? key = null, string? folderName = null)
        => throw new NotImplementedException();
    #endregion

}
