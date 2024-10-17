using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class TextCache
{
    private const string LocalStorageFolderName = "P42.Utils.TextCache";
    private static readonly string RootPath;
        
    static TextCache()
    {
        DirectoryExtensions.GetOrCreateDirectory(Environment.ApplicationCachePath);
        RootPath = Path.Combine(Environment.ApplicationCachePath, LocalStorageFolderName);
        DirectoryExtensions.GetOrCreateDirectory(RootPath);
    }
    
    private static string FolderPath(string? folderName = null)
    {

        if (string.IsNullOrWhiteSpace(folderName))
            return RootPath;

        var folderPath = Path.Combine(RootPath, folderName);
        DirectoryExtensions.GetOrCreateDirectory(folderPath);
        return folderPath;
    }
    
    /// <summary>
    /// List all files in TextCache folder
    /// </summary>
    /// <param name="subFolderName">Optional subfolder name</param>
    /// <returns></returns>
    public static List<string> List(string? subFolderName = null)
    {
        var folderPath = FolderPath(subFolderName);
        var files = Directory.EnumerateFiles(folderPath);
        return files.ToList();
    }

    /// <summary>
    /// Path for Text Cache
    /// </summary>
    /// <param name="key">Text Cache Key</param>
    /// <param name="subFolderName">optional text cache subfolder</param>
    /// <returns></returns>
    private static string CachedPath(string key, string? subFolderName = default)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;
            
        var fileName = key.Trim().ToMd5HashString();
        return Path.Combine(FolderPath(subFolderName), fileName);
    }

    /// <summary>
    /// Is the key cached?
    /// </summary>
    /// <param name="key">Text cache key</param>
    /// <param name="subFolderName">optional text cache subfolder</param>
    /// <returns></returns>
    public static bool IsCached(string key, string? subFolderName = default)
    {
        var path = CachedPath(key, subFolderName);
        return !string.IsNullOrWhiteSpace(path) && File.Exists(path);
    }

    /// <summary>
    /// Store text using key
    /// </summary>
    /// <param name="text">Text to store</param>
    /// <param name="key">Text cache Key</param>
    /// <param name="subFolderName">Optional text cache subfolder</param>
    public static void Store(string text, string key, string? subFolderName = default)
    {
        var path = CachedPath(key, subFolderName);
        if (string.IsNullOrWhiteSpace(path))
            return;

        var tmpPath = CachedPath(Guid.NewGuid().ToString(), subFolderName);
        File.WriteAllText(tmpPath, text);
        File.Move(tmpPath, path, true);
        File.Delete(tmpPath);
    }

    /// <summary>
    /// Recall cached text using key
    /// </summary>
    /// <param name="key">Text cache key</param>
    /// <param name="subFolderName">Optional text cache subfolder</param>
    /// <returns></returns>
    public static string Recall(string key, string? subFolderName = default)
    {
        var result = string.Empty;
        try
        {
            var path = CachedPath(key, subFolderName);
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

    /// <summary>
    /// Get stream reader for text cache
    /// </summary>
    /// <param name="key">Text cache key</param>
    /// <param name="subFolderName">Optional text cache subfolder</param>
    /// <returns></returns>
    public static StreamReader? GetStreamReader(string key, string? subFolderName = default)
    {
        try
        {
            var path = CachedPath(key, subFolderName);
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

    /// <summary>
    /// Clear cache (older than one year)
    /// </summary>
    /// <param name="key">optional key to target; optional all keys in folder</param>
    /// <param name="subFolderName">optional text cache subfolder, default is cache root folder</param>
    /// <returns></returns>
    public static bool Clear(string? key = null, string? subFolderName = default)
        => Clear(DateTime.MinValue.AddYears(1), key, subFolderName);

    /// <summary>
    /// Clear cache (older than timeSpan)
    /// </summary>
    /// <param name="timeSpan">min age cleared</param>
    /// <param name="key">optional targeted key; optional all keys in folder</param>
    /// <param name="subFolderName">optional subfolder, default is cache root folder</param>
    /// <returns>true if successful</returns>
    public static bool Clear(TimeSpan timeSpan, string? key = default, string? subFolderName = default)
        => Clear(DateTime.Now - timeSpan, key, subFolderName);

    /// <summary>
    /// Clear cache (older than dateTime)
    /// </summary>
    /// <param name="dateTime">any files not touched after dateTime</param>
    /// <param name="key">optional targeted key; optional all keys in folder</param>
    /// <param name="subFolderName">optional subfolder, default is cache root folder</param>
    /// <returns>true on success</returns>
    public static bool Clear(DateTime dateTime, string? key = default, string? subFolderName = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            // complete clear
            var folderPath = FolderPath(subFolderName);
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
            
            if (!filesRemaining && folderPath != FolderPath())
                Directory.Delete(folderPath);
                
            return true;
        }
        
        var path = CachedPath(key, subFolderName);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return false;

        if (File.GetLastWriteTime(path) >= dateTime)
            return false;

        File.Delete(path);
        return true;
    }

}
