using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Linq;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// Cache for embedded resources
/// </summary>
public static class EmbeddedResourceCache
{
    private const string LocalStorageFolderName = "P42.Utils.EmbeddedResourceCache";
    private static readonly string RootPath;
    private static readonly object Locker = new();
    private static readonly Dictionary<string, Task<bool>> CacheTasks = new();

    static EmbeddedResourceCache()
    {
        // DO NOT CHANGE Environment.ApplicationDataPath to another path.  This is used to pass EmbeddedResource Fonts to UWP Text elements and there is zero flexibility here.
        DirectoryExtensions.GetOrCreateDirectory(Environment.ApplicationCachePath);
        RootPath = Path.Combine(Environment.ApplicationCachePath, LocalStorageFolderName);
        if (Directory.Exists(RootPath))
            Directory.Delete(RootPath, true);
        DirectoryExtensions.GetOrCreateDirectory(RootPath);
    }


    internal static string FolderPath(Assembly? assembly, string? subFolderName = default)
    {
        var root = RootPath;
        if (assembly is not null)
        {
            var assemblyName = assembly.GetName().Name;
            root = string.IsNullOrWhiteSpace(assemblyName) 
                ? root 
                : Path.Combine(root, assemblyName);
            DirectoryExtensions.GetOrCreateDirectory(root);
        }

        if (string.IsNullOrWhiteSpace(subFolderName))
            return root;

        var folderPath = Path.Combine(root, subFolderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }

    /// <summary>
    /// List cached embedded resource files
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="subFolderName"></param>
    /// <returns></returns>
    public static List<string> List(Assembly assembly, string subFolderName)
    {
        var folderPath = FolderPath(assembly, subFolderName);
        var files = Directory.EnumerateFiles(folderPath);
        return files.ToList();
    }
    
    /// <summary>
    /// Get Stream for cached embedded resource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <param name="subFolderName"></param>
    /// <returns>stream</returns>
    public static Stream? GetStream(string resourceId, Assembly? assembly = null, string subFolderName = "")
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly is null)
            return null;
        
        var fileName = LocalStorageSubPathForEmbeddedResource(resourceId, assembly, subFolderName);
        if (string.IsNullOrWhiteSpace(fileName))
            return null;
        
        var result = File.Open(Path.Combine(FolderPath(assembly, subFolderName), fileName), FileMode.Open);
        return result;
    }

    /// <summary>
    /// Get Stream for cached embedded resource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <param name="subFolderName"></param>
    /// <returns>task to stream</returns>
    public static async Task<Stream?> GetStreamAsync(string resourceId, Assembly? assembly = null, string subFolderName = "")
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly is null)
            return null;
        
        var fileName = await LocalStorageSubPathForEmbeddedResourceAsync(resourceId, assembly, subFolderName);
        if (string.IsNullOrWhiteSpace(fileName))
            return null;
        
        var result = File.Open(Path.Combine(FolderPath(assembly, subFolderName), fileName), FileMode.Open);
        return result;
    }


    /// <summary>
    /// Get app uri (ms-appdata://) for cached embedded resource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <param name="subFolderName"></param>
    /// <returns></returns>
    public static string ApplicationUri(string resourceId, Assembly? assembly = null, string subFolderName = "")
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly is null)
            return string.Empty;

        var localStorageFileName = LocalStorageSubPathForEmbeddedResource(resourceId, assembly, subFolderName);
        var asmName = assembly.GetName().Name;
        var updatedLocalStorageFileName = localStorageFileName.Replace('\\', '/');
        var uriString =
            $"ms-appdata:///local/{LocalStorageFolderName}/{(asmName is null ? string.Empty : $"{asmName}/")}{updatedLocalStorageFileName}";
        return uriString;
    }


    /// <summary>
    /// Get full path for cached embedded resource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <param name="subFolderName"></param>
    /// <returns>full path</returns>
    public static string LocalStorageFullPathForEmbeddedResource(string resourceId, Assembly? assembly = null, string subFolderName = "")
    {
        if (LocalStorageSubPathForEmbeddedResource(resourceId, assembly, subFolderName) is { } subPath
            && !string.IsNullOrWhiteSpace(subPath))
            return Path.Combine(FolderPath(assembly, subFolderName), subPath);
        return string.Empty;
    }

    
    /// <summary>
    /// Full path for cached embedded resource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <param name="subFolderName"></param>
    /// <returns>path</returns>
    public static async Task<string> LocalStorageFullPathForEmbeddedResourceAsync(string resourceId, Assembly? assembly = null, string subFolderName = "")
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly is null)
            return string.Empty;

        return await LocalStorageSubPathForEmbeddedResourceAsync(resourceId, assembly, subFolderName) is { } subPath 
            ? Path.Combine(FolderPath(assembly, subFolderName), subPath)
            : string.Empty;
    }
    

    /// <summary>
    /// Clear cached embedded resource
    /// </summary>
    /// <param name="resourceId">optional, target ResourceId</param>
    /// <param name="assembly">optional, target assembly</param>
    /// <param name="subFolderName">optional subfolder name</param>
    /// <returns>true if success</returns>
    public static bool TryClear(string? resourceId = null, Assembly? assembly = null, string subFolderName = "")
        => TryClear(DateTime.MinValue.AddYears(1), resourceId, assembly, subFolderName);

    /// <summary>
    /// Clear cached embedded resource
    /// </summary>
    /// <param name="timeSpan">when older than TimeSpan</param>
    /// <param name="resourceId">optional, target ResourceId</param>
    /// <param name="assembly">optional, target assembly</param>
    /// <param name="subFolderName">optional subfolder name</param>
    /// <returns>true if success</returns>
    public static bool TryClear(TimeSpan timeSpan, string? resourceId = null, Assembly? assembly = null, string subFolderName = "")
        => TryClear(DateTime.Now - timeSpan, resourceId, assembly, subFolderName);

    /// <summary>
    /// Clear cached embedded resource
    /// </summary>
    /// <param name="dateTime">when older than DateTime</param>
    /// <param name="resourceId">optional, target ResourceId</param>
    /// <param name="assembly">optional, target assembly</param>
    /// <param name="subFolderName">optional subfolder name</param>
    /// <returns>true if success</returns>
    public static bool TryClear(DateTime dateTime, string? resourceId = null, Assembly? assembly = null, string subFolderName = "")
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            // complete clear
            var folderPath = FolderPath(assembly, subFolderName);
            if (!Directory.Exists(folderPath))
                return false;

            var files = Directory.EnumerateFiles(folderPath);
            var filesRemaining = false;
            foreach (var file in files)
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

        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly == null)
            return false;

        var path = Path.Combine(FolderPath(assembly, subFolderName), resourceId);
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
            return false;

        if (File.GetLastWriteTime(path) >= dateTime)
            return false;

        File.Delete(path);
        return true;
    }


    /// <summary>
    /// SubPath to cached copy of embedded resource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly">optional, assembly</param>
    /// <param name="subFolderName">optional, subfolder name</param>
    /// <returns></returns>
    public static async Task<string> LocalStorageSubPathForEmbeddedResourceAsync(string resourceId, Assembly? assembly = null, string subFolderName = "")
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly == null)
            return string.Empty;

        var fileName = resourceId;
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        var isZip = fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        if (fileName.StartsWith(subFolderName + ".", StringComparison.Ordinal))
            fileName = fileName[(subFolderName + ".").Length..];

        try
        {
            var path = isZip
                ? Path.Combine(FolderPath(assembly), resourceId)
                : Path.Combine(FolderPath(assembly, subFolderName), fileName);

            lock (Locker)
            {
                if (!CacheTasks.ContainsKey(path) && (File.Exists(path) || Directory.Exists(path)))
                    return isZip
                        ? FolderPath(assembly, subFolderName)
                        : fileName;
            }

            if (TryCacheEmbeddedResourceAsync(resourceId, assembly, path) is not { } task)
                return string.Empty;

            try
            {
                if (await task)
                {
                    if (!isZip)
                        return fileName;

                    System.IO.Compression.ZipFile.ExtractToDirectory(path, FolderPath(assembly, subFolderName));
                    return FolderPath(assembly, subFolderName);
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }
            finally
            {
                lock (Locker)
                {
                    CacheTasks.Remove(path);
                }
            }
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        
        return string.Empty;
    }
    
    /// <summary>
    /// SubPath to cached copy of embedded resource
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly">optional, assembly</param>
    /// <param name="subFolderName">optional, subfolder name</param>
    /// <returns></returns>
    public static string LocalStorageSubPathForEmbeddedResource(string resourceId, Assembly? assembly = null, string subFolderName = "")
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly == null)
            return string.Empty;

        var fileName = resourceId;
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        var isZip = fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        if (fileName.StartsWith(subFolderName + ".", StringComparison.Ordinal))
            fileName = fileName[(subFolderName + ".").Length..];

        var path = isZip
            ? Path.Combine(FolderPath(assembly), resourceId)
            : Path.Combine(FolderPath(assembly, subFolderName), fileName);

        lock (Locker)
        {
            if (!CacheTasks.ContainsKey(path) && (File.Exists(path) || Directory.Exists(path)))
                return isZip
                    ? FolderPath(assembly, subFolderName)
                    : fileName;
        }

        if (!TryCacheEmbeddedResource(resourceId, assembly, path))
            return string.Empty;

        if (!isZip)
            return fileName;

        System.IO.Compression.ZipFile.ExtractToDirectory(path, FolderPath(assembly, subFolderName));
        return FolderPath(assembly, subFolderName);
    }


    private static bool TryCacheEmbeddedResource(string resourceId, Assembly assembly, string path)
    {
        try
        {
            using var stream = EmbeddedResource.GetStream(resourceId, assembly);
            if (stream is null)
                return false;
            
            using var fileStream = new FileStream(path, FileMode.Create);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fileStream);
            fileStream.Flush(true);
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        
        if (File.Exists(path))
            File.Delete(path);
        
        return false;
    }
    
    private static Task<bool> TryCacheEmbeddedResourceAsync(string resourceId, Assembly assembly, string fileName)
    {
        lock (Locker)
        {
            if (CacheTasks.TryGetValue(fileName, out var task))
                return task;
            CacheTasks.Add(fileName, task = TryCacheTaskAsync(resourceId, assembly, fileName));
            return task;
        }
    }

    private static async Task<bool> TryCacheTaskAsync(string resourceId, Assembly assembly, string path)
    {
        try
        {
            await using var stream = EmbeddedResource.GetStream(resourceId, assembly);
            if (stream == null)
                return false;
            
            await using var fileStream = new FileStream(path, FileMode.Create);
            stream.Seek(0, SeekOrigin.Begin);
            await stream.CopyToAsync(fileStream);
            fileStream.Flush(true);
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        if (File.Exists(path))
            File.Delete(path);
        return false;
    }
}
