using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// DirectoryInfo extensions
/// </summary>
public static class DirectoryExtensions
{
    /// <summary>
    /// Assure DirectoryInfo exists
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception">Failure to assure directory existence</exception>
    [Obsolete("Use GetOrCreate or TryGetOrCreate instead.", true)]
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static DirectoryInfo AssureExists(string fullPath)
        => GetOrCreateDirectory(fullPath);

    /// <summary>
    /// Return DirectoryInfo Info for path, creating if it doesn't exist
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="exceptLast"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public static DirectoryInfo GetOrCreateDirectory(string fullPath, bool exceptLast = false)
    {
        var path = $"{Path.DirectorySeparatorChar}";
        var directoryInfo = new DirectoryInfo(path);

        if (string.IsNullOrWhiteSpace(fullPath))
            throw new ArgumentNullException(nameof(fullPath));

        if (fullPath.Length > 260)
            throw new ArgumentException("Full path is too long.", nameof(fullPath));

        var parts = fullPath.Split('/', '\\').ToList();
        for (var i = 0; i < parts.Count;)
        {
            var part = parts[i];
            if (part == ".")
                parts.RemoveAt(i);
            else if (part == ".." && i > 0)
            {
                parts.RemoveAt(i);
                parts.RemoveAt(i - 1);
                i--;
            }
            else
                i++;
        }

        if (parts.Count > 0 && exceptLast)
            parts.RemoveAt(parts.Count - 1);
        
        foreach (var part in parts)
        {
            if (!part.IsLegalFileName())
                throw new ArgumentException($"Illegal characters are not allowed. Part [{part}] of fullPath [{fullPath}] ", nameof(fullPath));
            path = Path.Combine(path, part);
            directoryInfo = new DirectoryInfo(path);
            if (!directoryInfo.Exists)
                directoryInfo = Directory.CreateDirectory(path);
        }
        
        return directoryInfo;
    }


    /// <summary>
    /// Attempt to get or create a directory
    /// </summary>
    /// <param name="fullPath">Path</param>
    /// <param name="directoryInfo">result</param>
    /// <param name="exceptLast"></param>
    /// <returns>true on success</returns>
    public static bool TryGetOrCreateDirectory(string fullPath, out DirectoryInfo directoryInfo, bool exceptLast = false)
    {
        try
        {
            directoryInfo = GetOrCreateDirectory(fullPath, exceptLast);
            return directoryInfo.Exists;
        }
        catch (Exception e)
        {
            QLog.Debug(e);
            directoryInfo = new DirectoryInfo(fullPath);
            return false;
        }
        
    }


    /// <summary>
    /// Recursively copy directory
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="overwrite"></param>
    /// <param name="wipe">delete destination before writing</param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static void Copy(this DirectoryInfo source, DirectoryInfo destination, bool overwrite = false, bool wipe = false)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(destination);

        if (!source.Exists)
            throw new DirectoryNotFoundException($"Source directory [{source.FullName}] does not exist.");

        if (wipe)
            destination.Delete(true);

        if (!destination.WritePossible(overwrite))
            throw new IOException($"FileInfo [{destination.FullName}] exists at destination directory and overwrite is false.");
        
        if (!destination.Exists)
            destination.Create();
        
        var files = source.GetFiles();
        foreach (var file in files)
            file.CopyTo(Path.Combine(destination.FullName, file.Name), overwrite);

        var directories = source.GetDirectories();
        foreach (var directory in directories)
        {
            var newDirectory = Directory.CreateDirectory(Path.Combine(destination.FullName, directory.Name));
            directory.Copy(newDirectory, overwrite);
        }
    }
    
    public static async Task CopyAsync(this DirectoryInfo source, DirectoryInfo destination, bool overwrite = false, bool wipe = false)
        => await Task.Run(() => Copy(source, destination, overwrite, wipe));
    
    /// <summary>
    /// Recursively copy directory
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="overwrite"></param>
    /// <param name="wipe">delete destination before writing</param>
    /// <returns>true on success</returns>
    public static bool TryCopy(this DirectoryInfo source, DirectoryInfo destination, bool overwrite = false, bool wipe = false)
    {
        if (!source.Exists)
            return false;

        try
        {
            source.Copy(destination, overwrite, wipe);
            return true;
        }
        catch (Exception e)
        {
            QLog.Error(e);
        }
        
        return false;
    }
    
    /// <summary>
    /// Store of Unpackager functions
    /// </summary>
    private static readonly Dictionary<string, Func<string, string?, bool, Task<string>>> BuiltInUnpackagers = new()
    {
        { ".zip", UnpackZipAsync },
        { ".tar", UnpackTarAsync },
        { ".tar.gz", UnTarGzPackageAsync },
        { ".tgz", UnpackTgzAsync }
    };
    
    /// <summary>
    /// Store for user defined Unpackager functions
    /// </summary>
    // ReSharper disable once CollectionNeverUpdated.Global
    public static readonly Dictionary<string, Func<string, string?, bool, Task<string>>> ExtendedUnpackagers = new();

    internal static Dictionary<string, Func<string, string?, bool, Task<string>>> Unpackagers =>
        BuiltInUnpackagers
            .Concat(ExtendedUnpackagers)
            .ToDictionary(x => x.Key, x => x.Value);

    /// <summary>
    /// Zip unpackager
    /// </summary>
    /// <param name="sourcePath">path to .zip file</param>
    /// <param name="destPath">path to unpacked contents</param>
    /// <param name="overwriteFiles"></param>
    /// <returns>path to unpacked contents</returns>
    private static async Task<string> UnpackZipAsync(string sourcePath, string? destPath = null, bool overwriteFiles = true)
    {
        if (string.IsNullOrEmpty(destPath))
        {
            if (sourcePath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                destPath = sourcePath[..^".zip".Length];
            else
                destPath = sourcePath + ".unpacked";
        }        
        
        if (!Directory.Exists(destPath))
            Directory.CreateDirectory(destPath);
        
        System.IO.Compression.ZipFile.ExtractToDirectory(sourcePath, destPath, overwriteFiles);
        await Task.CompletedTask;
        return destPath;
    }
    
    /// <summary>
    /// Tar unpackager
    /// </summary>
    /// <param name="sourcePath">path to .tar file</param>
    /// <param name="destPath">path to unpacked contents</param>
    /// <param name="overwriteFiles"></param>
    /// <returns>path to unpacked contents</returns>
    private static async Task<string> UnpackTarAsync(string sourcePath, string? destPath = null, bool overwriteFiles = true)
    {
        if (string.IsNullOrEmpty(destPath))
        {
            if (sourcePath.EndsWith(".tar", StringComparison.OrdinalIgnoreCase))
                destPath = sourcePath[..^".tar".Length];
            else
                destPath = sourcePath + ".unpacked";
        }        
        
        if (!Directory.Exists(destPath))
            Directory.CreateDirectory(destPath);
        
        await TarFile.ExtractToDirectoryAsync(sourcePath, destPath, overwriteFiles);
        return destPath;
    }
    
    /// <summary>
    /// tar.gz unpackager
    /// </summary>
    /// <param name="sourcePath">path to .tar.gz file</param>
    /// <param name="destPath">path to unpacked contents</param>
    /// <param name="overwriteFiles"></param>
    /// <returns>path to unpacked contents</returns>
    private static async Task<string> UnTarGzPackageAsync(string sourcePath, string? destPath = null, bool overwriteFiles = true)
    {
        var tarPath = await FileExtensions.UncompressGzAsync(sourcePath, null, overwriteFiles);
        return await UnpackTarAsync(tarPath, destPath);
    }

    /// <summary>
    /// tgz unpackager
    /// </summary>
    /// <param name="sourcePath">path to .tgz file</param>
    /// <param name="destPath">path to unpacked contents</param>
    /// <param name="overwriteFiles"></param>
    /// <returns>path to unpacked contents</returns>
    private static async Task<string> UnpackTgzAsync(string sourcePath, string? destPath = null, bool overwriteFiles = true)
    {
        if (string.IsNullOrEmpty(destPath))
        {
            if (sourcePath.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
                destPath = sourcePath[..^".tgz".Length];
            else
                destPath = sourcePath + ".unpacked";
        }

        return await UnTarGzPackageAsync(sourcePath, destPath);
    }

    /// <summary>
    /// Does the file, url, resourceId, etc. have an extension supported by UnpackArchiveAsync?
    /// </summary>
    /// <param name="path"></param>
    /// <returns>true if supported</returns>
    public static bool IsSupportedPackageExtension(string path)
        => Unpackagers.Any(x => path.EndsWith(x.Key, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Unpacks package file into destPath folder
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="destPath"></param>
    /// <param name="overwrite"></param>
    /// <returns>path to unpacked folder, null upon failure</returns>
    public static async Task<string?> UnpackArchiveAsync(string sourcePath, string? destPath = null, bool overwrite = true)
    {
        if (Unpackagers.FirstOrDefault(x => sourcePath.EndsWith(x.Key, StringComparison.OrdinalIgnoreCase)) is { Value: {} unpack })
            return await unpack(sourcePath, destPath, overwrite);
        return null;
    }

    /// <summary>
    /// Human-readable folder tree
    /// </summary>
    /// <param name="path"></param>
    /// <param name="depth"></param>
    /// <param name="writer"></param>
    /// <returns></returns>
    public static StringWriter FolderTree(string path, StringWriter writer, int depth = 1)
    {
        var tabs = new string('\t', depth);
        
        var dirs = Directory.GetDirectories(path);
        for (var i=0; i < dirs.Length; i++)
        {
            var dir = dirs[i];
            var line = i == dirs.Length - 1 ? "└──" : "├──";
            writer.WriteLine($"{tabs}{line}{dir}");
            FolderTree(dir, writer, depth + 1);
        }

        var files = Directory.GetFiles(path);
        for (var i=0; i < files.Length; i++)
        {
            var file = files[i];
            var line = i == files.Length - 1 ? "└──" : "├──";
            writer.WriteLine($"{tabs}{line}{file}");
        }
        
        return writer;
    }

    /// <summary>
    /// Human-readable folder tree
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string FolderTree(string path)
        => FolderTree(path, new StringWriter()).ToString();
    
    /// <summary>
    /// Human-readable folder tree
    /// </summary>
    /// <param name="directoryInfo"></param>
    /// <returns></returns>
    public static string FolderTree(this DirectoryInfo directoryInfo)
        => FolderTree(directoryInfo.FullName);
    
    /// <summary>
    /// Human-readable folder tree
    /// </summary>
    /// <param name="directoryInfo"></param>
    /// <param name="writer"></param>
    /// <param name="depth"></param>
    /// <returns></returns>
    public static StringWriter FolderTree(this DirectoryInfo directoryInfo, StringWriter writer, int depth = 1)
        => FolderTree(directoryInfo.FullName, writer, depth);
}
