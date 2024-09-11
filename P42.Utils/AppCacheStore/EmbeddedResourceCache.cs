using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Linq;
using P42.Serilog.QuickLog;

namespace P42.Utils;

#nullable enable

public static class EmbeddedResourceCache
{
    private const string LocalStorageFolderName = "P42.Utils.EmbeddedResourceCache";
    private static readonly string s_rootPath;
    private static readonly object s_locker = new();
    private static readonly Dictionary<string, Task<bool>> s_cacheTasks = new();

    static EmbeddedResourceCache()
    {
        DirectoryExtensions.AssureExists(Environment.ApplicationCachePath);
        s_rootPath = Path.Combine(Environment.ApplicationCachePath, LocalStorageFolderName);
        if (Directory.Exists(s_rootPath))
            Directory.Delete(s_rootPath, true);
        DirectoryExtensions.AssureExists(s_rootPath);
    }


    // DO NOT CHANGE Environment.ApplicationDataPath to another path.  This is used to pass EmbeddedResource Fonts to UWP Text elements and there is zero flexibility here.
    internal static string FolderPath(Assembly? assembly, string? folderName = default)
    {
        var root = s_rootPath;
        if (assembly is not null)
        {
            var assemblyName = assembly.GetName().Name;
            root = string.IsNullOrWhiteSpace(assemblyName) 
                ? root 
                : Path.Combine(root, assemblyName);
            DirectoryExtensions.AssureExists(root);
        }

        if (string.IsNullOrWhiteSpace(folderName))
            return root;

        var folderPath = Path.Combine(root, folderName);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }

    public static List<string> List(Assembly assembly, string folderName)
    {
        var folderPath = FolderPath(assembly, folderName);
        var files = Directory.EnumerateFiles(folderPath);
        return files.ToList();
    }



    public static Stream? GetStream(string resourceId, Assembly? assembly = null, string? folderName = null)
    {
        var task = Task.Run(() => GetStreamAsync(resourceId, assembly, folderName));
        return task.Result;
    }

    public static async Task<Stream?> GetStreamAsync(string resourceId, Assembly? assembly = null, string? folderName = null)
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly is null)
            return null;
        
        var fileName = await LocalStorageSubPathForEmbeddedResourceAsync(resourceId, assembly, folderName);
        if (string.IsNullOrWhiteSpace(fileName))
            return null;
        
        var result = File.Open(Path.Combine(FolderPath(assembly, folderName), fileName), FileMode.Open);
        return result;
    }


    public static string ApplicationUri(string resourceId, Assembly? assembly = null, string? folderName = null)
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly is null)
            return string.Empty;

        var localStorageFileName = LocalStorageSubPathForEmbeddedResource(resourceId, assembly, folderName);
        var asmName = assembly.GetName().Name;
        var updatedLocalStorageFileName = localStorageFileName.Replace('\\', '/');
        var uriString =
            $"ms-appdata:///local/{LocalStorageFolderName}/{(asmName is null ? string.Empty : $"{asmName}/")}{updatedLocalStorageFileName}";
        return uriString;
    }

    public static string LocalStorageSubPathForEmbeddedResource(string resourceId, Assembly? assembly = null, string? folderName = null)
    {
        var task = Task.Run(() => LocalStorageSubPathForEmbeddedResourceAsync(resourceId, assembly, folderName));
        return task.Result;
    }

    public static string LocalStorageFullPathForEmbeddedResource(string resourceId, Assembly? assembly = null, string? folderName = null)
    {
        var task = Task.Run(async () =>
        {
            var path = await LocalStorageFullPathForEmbeddedResourceAsync(resourceId, assembly, folderName);
            return path;
        });

        return task.Result;
    }

    public static async Task<string> LocalStorageFullPathForEmbeddedResourceAsync(string resourceId, Assembly? assembly = null, string? folderName = null)
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly is null)
            return string.Empty;

        return await LocalStorageSubPathForEmbeddedResourceAsync(resourceId, assembly, folderName) is { } subPath 
            ? Path.Combine(FolderPath(assembly, folderName), subPath)
            : string.Empty;
    }

    public static bool Clear(string? resourceId = null, Assembly? assembly = null, string? folderName = null)
        => Clear(DateTime.MinValue.AddYears(1), resourceId, assembly, folderName);

    public static bool Clear(TimeSpan timeSpan, string? resourceId = null, Assembly? assembly = null, string? folderName = null)
        => Clear(DateTime.Now - timeSpan, resourceId, assembly, folderName);

    public static bool Clear(DateTime dateTime, string? resourceId = null, Assembly? assembly = null, string? folderName = null)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
        {
            // complete clear
            var folderPath = FolderPath(assembly, folderName);
            if (Directory.Exists(folderPath))
            {
                var files = Directory.EnumerateFiles(folderPath);
                bool filesRemaining = false;
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        if (File.GetLastWriteTime(file) < dateTime)
                            File.Delete(file);
                        else
                            filesRemaining = true;
                    }
                }
                if (!filesRemaining && folderPath != FolderPath(null))
                    Directory.Delete(folderPath);
                return true;
            }
            return false;
        }

        assembly = assembly ?? Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly == null)
            return false;

        var path = Path.Combine(FolderPath(assembly, folderName), resourceId);
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            if (File.GetLastWriteTime(path) < dateTime)
            {
                File.Delete(path);
                return true;
            }
        }
        return false;
    }


    public static async Task<string> LocalStorageSubPathForEmbeddedResourceAsync(string resourceId, Assembly? assembly = null, string? folderName = null)
    {
        assembly ??= Environment.EmbeddedResourceAssemblyResolver?.Invoke(resourceId, null);
        if (assembly == null)
            return string.Empty;

        var fileName = resourceId;
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        var isZip = fileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase);

        if (fileName.StartsWith(folderName + ".", StringComparison.Ordinal))
            fileName = fileName[(folderName + ".").Length..];

        try
        {
            var path = isZip
                ? Path.Combine(FolderPath(assembly), resourceId)
                : Path.Combine(FolderPath(assembly, folderName), fileName);

            lock (s_locker)
            {
                if (!s_cacheTasks.ContainsKey(path) && (File.Exists(path) || Directory.Exists(path)))
                    return isZip
                        ? FolderPath(assembly, folderName)
                        : fileName;
            }

            if (CacheEmbeddedResourceAsync(resourceId, assembly, path) is not { } task)
                return string.Empty;

            try
            {
                if (await task)
                {
                    if (!isZip)
                        return fileName;

                    System.IO.Compression.ZipFile.ExtractToDirectory(path, FolderPath(assembly, folderName));
                    return FolderPath(assembly, folderName);
                }
            }
            catch (Exception ex)
            {
                QLog.Error(ex);
            }
            finally
            {
                lock (s_locker)
                {
                    s_cacheTasks.Remove(path);
                }
            }
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        return string.Empty;
    }

    private static Task<bool> CacheEmbeddedResourceAsync(string resourceId, Assembly assembly, string fileName)
    {
        lock (s_locker)
        {
            if (s_cacheTasks.TryGetValue(fileName, out var task))
                return task;
            s_cacheTasks.Add(fileName, task = CacheTaskAsync(resourceId, assembly, fileName));
            return task;
        }
    }

    private static async Task<bool> CacheTaskAsync(string resourceId, Assembly assembly, string path)
    {
        try
        {
            await using var stream = EmbeddedResource.GetStream(resourceId, assembly);
            if (stream is null)
            {

                var msg =
                    $"Cannot find EmbeddedResource [{resourceId}] in assembly [{assembly.FullName}].   Here are the ResourceIds in that assembly:";
                msg = assembly.GetManifestResourceNames().Aggregate(msg, (current, id) => current + $"\t{id}");
                QLog.Error(msg);
            }
            else
            {
                await using var fileStream = new FileStream(path, FileMode.Create);
                stream.Seek(0, SeekOrigin.Begin);
                await stream.CopyToAsync(fileStream);
                fileStream.Flush(true);
                return true;
            }
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
