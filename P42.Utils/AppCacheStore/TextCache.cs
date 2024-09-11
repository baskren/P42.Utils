using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using P42.Serilog.QuickLog;

namespace P42.Utils;

#nullable enable

public static class TextCache
{
    private const string LocalStorageFolderName = "P42.Utils.TextCache";
    private static readonly string s_rootPath;
        
    static TextCache()
    {
        DirectoryExtensions.AssureExists(Environment.ApplicationCachePath);
        s_rootPath = Path.Combine(Environment.ApplicationCachePath, LocalStorageFolderName);
        DirectoryExtensions.AssureExists(s_rootPath);
    }
    
    private static string FolderPath(string? folderName)
    {

        if (string.IsNullOrWhiteSpace(folderName))
            return s_rootPath;

        var folderPath = Path.Combine(s_rootPath, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }
    
    public static List<string> List(string folderName)
    {
        var folderPath = FolderPath(folderName);
        var files = Directory.EnumerateFiles(folderPath);
        return files.ToList();
    }

    private static string CachedPath(string key, string? folderName = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;
            
        var fileName = key.Trim().ToMd5HashString();
        return Path.Combine(FolderPath(folderName), fileName);
    }

    public static bool IsCached(string key, string? folderName = default)
    {
        var path = CachedPath(key, folderName);
        return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
    }

    public static void Store(string text, string key, string? folderName = default)
    {
        var path = CachedPath(key, folderName);
        if (string.IsNullOrWhiteSpace(path))
            return;

        var tmpPath = CachedPath(Guid.NewGuid().ToString(), folderName);
        File.WriteAllText(tmpPath, text);
        File.Move(tmpPath, path, true);
        File.Delete(tmpPath);
    }

    public static string Recall(string key, string? folderName = default)
    {
        var result = string.Empty;
        try
        {
            var path = CachedPath(key, folderName);
            if (string.IsNullOrWhiteSpace(path))
                return result;
            if (File.Exists(path))
                result = File.ReadAllText(path);
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }

        return result;
    }

    public static StreamReader? GetStreamReader(string key, string? folderName = default)
    {
        try
        {
            var path = CachedPath(key, folderName);
            if (string.IsNullOrWhiteSpace(path))
                return null;
                
            if (File.Exists(path)) 
                return new StreamReader(path); 
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        return null;
    }

    public static bool Clear(string? key = null, string? folderName = default)
        => Clear(DateTime.MinValue.AddYears(1), key, folderName);

    public static bool Clear(TimeSpan timeSpan, string? key = default, string? folderName = default)
        => Clear(DateTime.Now - timeSpan, key, folderName);

    public static bool Clear(DateTime dateTime, string? key = default, string? folderName = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            // complete clear
            var folderPath = FolderPath(folderName);
            if (!Directory.Exists(folderPath))
                return false;

            var filesRemaining = false;
            foreach (var file in Directory.EnumerateFiles(folderPath))
            {
                if (!File.Exists(file))
                    continue;

                if (File.GetLastWriteTime(file) < dateTime)
                    File.Delete(file);
                else
                    filesRemaining = true;
            }
            
            if (!filesRemaining && folderPath != FolderPath(null))
                Directory.Delete(folderPath);
                
            return true;
        }
        
        var path = CachedPath(key, folderName);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return false;

        if (File.GetLastWriteTime(path) >= dateTime)
            return false;

        File.Delete(path);
        return true;
    }

}
