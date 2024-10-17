using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace P42.Utils;

public static class DownloadCache
{
    private const string DownloadStorageFolderName = "P42.Utils.DownloadCache";
    private static readonly string RootPath;

    static DownloadCache()
    {
        DirectoryExtensions.GetOrCreateDirectory(Environment.ApplicationCachePath);
        RootPath = Path.Combine(Environment.ApplicationCachePath, DownloadStorageFolderName);
        Directory.CreateDirectory(RootPath);
    }
            
    private static string FolderPath(string? folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return RootPath;

        var folderPath = Path.Combine(RootPath, folderName);
        DirectoryExtensions.GetOrCreateDirectory(folderPath);
        return folderPath;
    }

    private static readonly object Locker = new ();
    private static readonly Dictionary<string, Task<bool>> DownloadTasks = new ();

    /// <summary>
    /// Cache the file found at Url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="folderName"></param>
    /// <returns>path to cached file</returns>
    public static string Download(string url, string? folderName = null)
    {
        var task = Task.Run(() => DownloadAsync(url, folderName));
        return task.Result;
    }

    /// <summary>
    /// List the cached files
    /// </summary>
    /// <param name="folderName"></param>
    /// <returns>path to cached files</returns>
    public static List<string> List(string folderName)
    {
        var folderPath = FolderPath(folderName);
        var files = Directory.EnumerateFiles(folderPath);
        return files.ToList();
    }


    /// <summary>
    /// Async cache file at url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="folderName"></param>
    /// <returns>path to cached file</returns>
    public static async Task<string> DownloadAsync(string url, string? folderName = null)
    {
        return await GetDownloadAsync(url, folderName);
    }

    private static string CachedPath(string url, string? folderName = null)
    {
        var fileName = url.Trim().ToMd5HashString();
        var path = Path.Combine(FolderPath(folderName), fileName);
        //return File.Exists(path) ? path : null;
        return path;
    }

    /// <summary>
    /// Has a url been cached?
    /// </summary>
    /// <param name="url"></param>
    /// <param name="folderName"></param>
    /// <returns>true on success</returns>
    public static bool IsCached(string url, string? folderName = null)
    {
        var path = CachedPath(url, folderName);
        return File.Exists(path) && !DownloadTasks.ContainsKey(path);
    }

    /// <summary>
    /// Clear all cached files
    /// </summary>
    /// <param name="url"></param>
    /// <param name="folderName"></param>
    /// <returns>true on success</returns>
    public static bool Clear(string? url = null, string? folderName = null)
        => Clear(DateTime.MinValue.AddYears(1), url, folderName);

    /// <summary>
    /// Clear cached files older than TimeSpan
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <param name="url"></param>
    /// <param name="folderName"></param>
    /// <returns>true on success</returns>
    public static bool Clear(TimeSpan timeSpan, string? url = null, string? folderName = null)
        => Clear(DateTime.Now - timeSpan, url, folderName);

    /// <summary>
    /// Clear cached files older than DateTime
    /// </summary>
    /// <param name="dateTime"></param>
    /// <param name="url"></param>
    /// <param name="folderName"></param>
    /// <returns>true on success</returns>
    public static bool Clear(DateTime dateTime, string? url = null, string? folderName = null)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            // complete clear
            var folderPath = FolderPath(folderName);
            if (!Directory.Exists(folderPath))
                return false;

            var files = Directory.EnumerateFiles(folderPath);
            var filesRemaining = false;
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    if (File.GetLastWriteTime(file) < dateTime)
                        File.Delete(file);
                    else
                        filesRemaining = true;
                }
                
                if (!filesRemaining && folderPath != FolderPath(null))
                    Directory.Delete(folderPath);
                
                return true;
            }
            
            return false;
        }
        
        var path = CachedPath(url, folderName);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return false;

        if (File.GetLastWriteTime(path) >= dateTime)
            return false;

        File.Delete(path);
        return true;

    }

    private static async Task<string> GetDownloadAsync(string url, string? folderName = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;
                
            var path = CachedPath(url, folderName);
            if (string.IsNullOrWhiteSpace(path))
                return string.Empty;
                
            if (!File.Exists(path) || DownloadTasks.ContainsKey(path))
                return await GetDownloadCoreAsync(url, path) ? path : string.Empty;

            System.Diagnostics.Debug.WriteLine("DownloadCache: [" + url + "] exists as [" + path + "]");
            return path;

        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            return string.Empty;
        }
    }

    private static Task<bool> GetDownloadCoreAsync(string url, string fileName)
    {
        lock (Locker)
        {
            //Task<bool> task = null;
            //if (_downloadTasks.TryGetValue(fileName, out task))
            if (DownloadTasks.TryGetValue(fileName, out var task))
                return task;

            DownloadTasks.Add(fileName, task = DownloadTaskAsync(url, fileName));
            return task;
        }
    }

    private static async Task<bool> DownloadTaskAsync(string url, string path)
    {

        try
        {
            using var client = new System.Net.Http.HttpClient();
            var data = await client.GetByteArrayAsync(url);
            System.Diagnostics.Debug.WriteLine($"DownloadTask: byte array downloaded for [{url}]");
            //var fileNamePaths = path.Split('\\');
            if (File.Exists(path))
                System.Diagnostics.Debug.WriteLine($"DownloadTask: FILE ALREADY EXISTS [{path}] [{url}]");
            await using var stream = new FileStream(path, FileMode.Create);
            stream.Write(data, 0, data.Length);

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
        if (File.Exists(path))
            File.Delete(path);
        return false;
    }
}
