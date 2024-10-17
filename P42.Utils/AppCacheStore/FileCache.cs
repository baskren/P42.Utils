using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// FileCache
/// </summary>
public static class FileCache
{
    private const string FileCacheFolderName = "P42.Utils.FileCache";

    private static string FolderPath(string folderName = "")
    {
        DirectoryExtensions.GetOrCreateDirectory(Environment.ApplicationDataPath);
        var root = Path.Combine(Environment.ApplicationDataPath, FileCacheFolderName);
        DirectoryExtensions.GetOrCreateDirectory(root);

        if (string.IsNullOrWhiteSpace(folderName))
            return root;

        var folderPath = Path.Combine(root, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }

    private static readonly object Locker = new();
    private static readonly Dictionary<string, Task<bool>> DownloadTasks = new();

    /// <summary>
    /// Lists cached files in folder
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns></returns>
    public static List<string> List(string folderName = "")
    {
        var folderPath = FolderPath(folderName);
        var files = Directory.EnumerateFiles(folderPath);
        return files.ToList();
    }
    
    /// <summary>
    /// Caches file
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="folderName"></param>
    /// <returns>path to file</returns>
    public static string? CacheFile(string sourceFilePath, string folderName = "")
    {
        var fileName = sourceFilePath.Trim().ToMd5HashString();
        return InnerCacheFile(sourceFilePath, folderName, fileName);
    }

    
    /// <summary>
    /// Caches file
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="folderName"></param>
    /// <returns>path to file</returns>
    public static Task<string?> CacheFileAsync(string sourceFilePath, string folderName = "")
    {
        var fileName = sourceFilePath.Trim().ToMd5HashString();
        return InnerCacheFileAsync(sourceFilePath, fileName, folderName);
    }

    private static string? InnerCacheFile(string sourceFilePath, string destFileName, string folderName = "")
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath) || string.IsNullOrWhiteSpace(destFileName))
            return null;
        
        try
        {
            var destPath = Path.Combine(FolderPath(folderName), destFileName);
            lock (Locker)
            {
                if (File.Exists(destPath) && !DownloadTasks.ContainsKey(destPath))
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadCache: [{destPath}] exists as [{destFileName}]");
                    return destPath;
                }
            }

            return TryCopy(sourceFilePath, destPath) 
                ? destPath 
                : null;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return null;
        }
    }

    /// <summary>
    /// Caches file
    /// </summary>
    /// <param name="sourceFilePath"></param>
    /// <param name="destFileName"></param>
    /// <param name="folderName"></param>
    /// <returns>path to file</returns>
    private static async Task<string?> InnerCacheFileAsync(string sourceFilePath, string destFileName, string folderName = "")
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath) || string.IsNullOrWhiteSpace(destFileName))
            return null;
        
        try
        {
            var destPath = Path.Combine(FolderPath(folderName), destFileName);
            lock (Locker)
            {
                if (File.Exists(destPath) && !DownloadTasks.ContainsKey(destPath))
                {
                    System.Diagnostics.Debug.WriteLine($"DownloadCache: [{destPath}] exists as [{destFileName}]");
                    return destPath;
                }
            }

            var success = await TryQueueCacheFileAsync(sourceFilePath, destPath);
            return success ? destPath : null;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return null;
        }
    }

    /// <summary>
    /// Queues the caching of a file
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="destPath"></param>
    /// <returns>true on success</returns>
    private static Task<bool> TryQueueCacheFileAsync(string sourcePath, string destPath)
    {
        lock (Locker)
        {
            if (DownloadTasks.TryGetValue(sourcePath, out var task))
                return task;

            DownloadTasks.Add(sourcePath, task = TryCopyAsync(sourcePath, destPath));
            return task;
        }
    }

    /// <summary>
    /// Copy file from source to dest
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="destPath"></param>
    /// <returns>true on success</returns>
    private static async Task<bool> TryCopyAsync(string sourcePath, string destPath)
    {
        try
        {
            await using var outFileStream = File.Create(destPath);
            await using var sourceStream = File.OpenRead(sourcePath);
            await sourceStream.CopyToAsync(outFileStream, 81920);
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        
        if (File.Exists(destPath))
            File.Delete(destPath);
        
        return false;
    }

    private static bool TryCopy(string sourcePath, string destPath)
    {
        try
        {
            using var outFileStream = File.Create(destPath);
            using var sourceStream = File.OpenRead(sourcePath);
            sourceStream.CopyTo(outFileStream, 81920);
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        
        if (File.Exists(destPath))
            File.Delete(destPath);
        
        return false;
    }

    /// <summary>
    /// Clear file cache
    /// </summary>
    /// <param name="sourceFilePath">path to source file, all in folder if null</param>
    /// <param name="folderName">optional subfolder name</param>
    /// <returns>true on success</returns>
    public static bool TryClear(string? sourceFilePath = null, string folderName = "")
        => TryClear(DateTime.MinValue.AddYears(1), sourceFilePath, folderName);

    /// <summary>
    /// Clear file cache
    /// </summary>
    /// <param name="timeSpan">items older than TimeSpan</param>
    /// <param name="sourceFilePath">path to source file, all in folder if null</param>
    /// <param name="folderName">optional subfolder name</param>
    /// <returns>true on success</returns>
    public static bool TryClear(TimeSpan timeSpan, string? sourceFilePath = null, string folderName = "")
        => TryClear(DateTime.Now - timeSpan, sourceFilePath, folderName);

    /// <summary>
    /// Clear file cache
    /// </summary>
    /// <param name="dateTime">items older than DateTime</param>
    /// <param name="sourceFilePath">path to source file, all in folder if null</param>
    /// <param name="folderName">optional subfolder name</param>
    /// <returns>true on success</returns>
    public static bool TryClear(DateTime dateTime, string? sourceFilePath = null, string folderName = "")
    {
        if (string.IsNullOrWhiteSpace(sourceFilePath))
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
            if (!filesRemaining && folderPath != FolderPath())
                Directory.Delete(folderPath);
            return true;
        }
        var path = CachedPath(sourceFilePath, folderName);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return false;

        if (File.GetLastWriteTime(path) >= dateTime)
            return false;

        File.Delete(path);
        return true;
    }

    private static string CachedPath(string sourceFilePath, string folderName = "")
    {
        var fileName = sourceFilePath.Trim().ToMd5HashString();
        return Path.Combine(FolderPath(folderName), fileName);
    }


}
