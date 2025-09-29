using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P42.Utils.LocalData;

namespace P42.Utils;

public static class LocalData_DirectoryInfo_Extensions
{
    #region DirectoryInfo
    /// <summary>
    /// Returns the FileInfo for a LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static DirectoryInfo Directory(this Item item) => !item.IsFile ? new DirectoryInfo(item.FullPath) : throw new Exception($"Item [{item}] is a File");

    /// <summary>
    /// Tries to get FileInfo for LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>true if exists</returns>
    public static bool TryDirectory(this Item item, [MaybeNullWhen(false)] out DirectoryInfo dirInfo)
    {
        try
        {
            dirInfo = item.Directory();
            return true;
        }
        catch (Exception)
        {
            dirInfo = null;
            return false;
        }
    }

    /// <summary>
    /// Get DirectoryInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<DirectoryInfo> AssureSourcedDirectoryAsync(this AsynchronousSourcedItem item)
    {
        if (item is { Exists: true, IsDirectory: true })
            return item.Directory();

        await item.ForcePullAsync();
        return item.Directory();
    }

    /// <summary>
    /// Get DirectoryInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static DirectoryInfo AssureSourcedDirectory(this SynchronousSourcedItem item)
    {
        if (item is { Exists: true, IsDirectory: true })
            return item.Directory();

        item.ForcePull();
        return item.Directory();
    }



    /// <summary>
    /// Try to get DirectoryInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<DirectoryInfo?> TryAssureSourcedDirectoryAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            return await item.AssureSourcedDirectoryAsync();
        }
        catch (Exception) { }

        return null;
    }

    /// <summary>
    /// Try to get DirectoryInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static DirectoryInfo? TryAssureSourcedDirectory(this SynchronousSourcedItem item)
    {
        try
        {
            return item.AssureSourcedDirectory();
        }
        catch (Exception) { }

        return null;
    }



    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="IOException"></exception>
    public static void StoreDirectory(this Item item, DirectoryInfo? sourceItem, bool wipeOld = true)
    {
        var dir = new DirectoryInfo(item.FullPath);
        if (!dir.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{dir.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        LocalData.Semaphore.Wait();

        try
        {
            if (dir.Exists && wipeOld)
                dir.Delete(true);

            item.AssureParentDirectory();
            sourceItem?.Copy(dir, wipe: wipeOld);
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }

    }

    /// <summary>
    /// Store sourceItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    public static async Task StoreDirectoryAsync(this Item item, DirectoryInfo? sourceItem, bool wipeOld = true)
    {
        var dir = new DirectoryInfo(item.FullPath);
        if (!dir.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{dir.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        await LocalData.Semaphore.WaitAsync();

        try
        {
            if (dir.Exists && wipeOld)
                dir.Delete(true);

            item.AssureParentDirectory();
            await (sourceItem?.CopyAsync(dir, wipe: wipeOld) ?? Task.CompletedTask);
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }
    }

    /// <summary>
    /// Puts an sourceItem in local data store (null clears out the sourceItem)
    /// </summary>
    /// <param name="sourceItem">null to clear</param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public static bool TryStoreDirectory(this Item item, DirectoryInfo? sourceItem, bool wipeOld = true)
    {
        try
        {
            item.StoreDirectory(sourceItem, wipeOld);
            return true;
        }
        catch (Exception) { }

        return false;
    }


    /// <summary>
    /// Puts an sourceItem in local data store (null clears out the sourceItem)
    /// </summary>
    /// <param name="sourceItem">null to clear</param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public static async Task<bool> TryStoreDirectoryAsync(this Item item, DirectoryInfo sourceItem, bool wipeOld = true)
    {
        try
        {
            await item.StoreDirectoryAsync(sourceItem, wipeOld);
            return true;
        }
        catch (Exception) { }

        return false;
    }

    public static async Task<DirectoryInfo?> AssureUnpackAsync(this AsynchronousSourcedItem item)
    {
        if (!DirectoryExtensions.IsSupportedPackageExtension(item.FullPath))
        {
            if (System.IO.Directory.Exists(item.FullPath))
                return new System.IO.DirectoryInfo(item.FullPath);
            throw new Exception($"Key [{item}] is not already stored and does not have supported package extension");
        }

        var packagePath = item.FullPath;
        foreach (var unpackager in DirectoryExtensions.Unpackagers)
        {
            if (packagePath.EndsWith(unpackager.Key, StringComparison.OrdinalIgnoreCase))
                packagePath = packagePath[..^unpackager.Key.Length];
        }

        if (System.IO.Directory.Exists(packagePath))
            return new DirectoryInfo(packagePath);

        if (!await item.TryAssurePulledAsync() || !File.Exists(item.FullPath))
            throw new Exception($"Source file for Key [{item}] cannot be retrieved");

        // Unpack package file and return directory
        if (await DirectoryExtensions.UnpackArchiveAsync(packagePath, item.FullPath) is { } unpackedFolderPath && !string.IsNullOrEmpty(unpackedFolderPath) && System.IO.Directory.Exists(unpackedFolderPath))
            return new DirectoryInfo(unpackedFolderPath);

        return null;

    }

    public static async Task<DirectoryInfo?> UnpackAsync(this Item item)
    {
        if (!DirectoryExtensions.IsSupportedPackageExtension(item.FullPath))
        {
            if (System.IO.Directory.Exists(item.FullPath))
                return new System.IO.DirectoryInfo(item.FullPath);
            throw new Exception($"Key [{item}] is not already stored and does not have supported package extension");
        }

        var packagePath = item.FullPath;
        foreach (var unpackager in DirectoryExtensions.Unpackagers)
        {
            if (packagePath.EndsWith(unpackager.Key, StringComparison.OrdinalIgnoreCase))
                packagePath = packagePath[..^unpackager.Key.Length];
        }

        if (System.IO.Directory.Exists(packagePath))
            return new DirectoryInfo(packagePath);

        if (!File.Exists(item.FullPath))
            return null;

        // Unpack package file and return directory
        if (await DirectoryExtensions.UnpackArchiveAsync(packagePath, item.FullPath) is { } unpackedFolderPath && !string.IsNullOrEmpty(unpackedFolderPath) && System.IO.Directory.Exists(unpackedFolderPath))
            return new DirectoryInfo(unpackedFolderPath);

        return null;

    }

    #endregion



}
