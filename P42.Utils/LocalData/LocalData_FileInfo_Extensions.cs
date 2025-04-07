using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P42.Utils.LocalData;

namespace P42.Utils;

public static class LocalData_FileInfo_Extensions
{
    #region FileInfo
    /// <summary>
    /// Returns the FileInfo for a LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static FileInfo File(this Item item) => !item.IsDirectory ? new FileInfo(item.FullPath) : throw new Exception($"Item [{item}] is a Directory");

    /// <summary>
    /// Tries to get FileInfo for LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>true if exists</returns>
    public static bool TryFile(this Item item, [MaybeNullWhen(false)] out FileInfo fileInfo)
    {
        try
        {
            fileInfo = item.File();
            return true;
        }
        catch (Exception)
        {
            fileInfo = null;
            return false;
        }
    }

    /// <summary>
    /// Get FileInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<FileInfo> AssureSourcedFileAsync(this AsynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return item.File();

        await item.ForcePullAsync();
        return item.File();
    }

    /// <summary>
    /// Get FileInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static FileInfo AssureSourcedFile(this SynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return item.File();

        item.ForcePull();
        return item.File();
    }


    /// <summary>
    /// Try to get FileInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<FileInfo?> TryAssureSourcedFileAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            return await item.AssureSourcedFileAsync();
        }
        catch (Exception) { }

        return null;
    }

    /// <summary>
    /// Try to get FileInfo, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static FileInfo? TryAssureSourcedFile(this SynchronousSourcedItem item)
    {
        try
        {
            return item.AssureSourcedFile();
        }
        catch (Exception) { }

        return null;
    }

    /// <summary>
    /// Stores sourceItem to LocalData.Item 
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="IOException"></exception>
    public static void StoreFile(this Item item, FileInfo? sourceItem, bool wipeOld = true)
    {
        var file = new FileInfo(item.FullPath);
        if (!file.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        LocalData.Semaphore.Wait();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            sourceItem?.CopyTo(item.FullPath, wipeOld);
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }

    }

    /// <summary>
    /// Stores sourceItem to LocalData.Item 
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    public static async Task StoreFileAsync(this Item item, FileInfo? sourceItem, bool wipeOld = true)
    {
        var file = new FileInfo(item.FullPath);
        if (!file.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        await LocalData.Semaphore.WaitAsync();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            sourceItem?.CopyTo(item.FullPath, wipeOld);
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
    public static bool TryStoreFile(this Item item, FileInfo sourceItem, bool wipeOld = true)
    {
        try
        {
            item.StoreFile(sourceItem, wipeOld);
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
    public static async Task<bool> TryStoreFileAsync(this Item item, FileInfo sourceItem, bool wipeOld = true)
    {
        try
        {
            await item.StoreFileAsync(sourceItem, wipeOld);
            return true;
        }
        catch (Exception) { }

        return false;
    }


    #endregion



}
