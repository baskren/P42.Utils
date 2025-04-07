using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static P42.Utils.LocalData;

namespace P42.Utils;

public static class LocalData_Stream_Extensions
{

    #region Stream

    /// <summary>
    /// Get stream for item in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="fileMode"></param>
    /// <returns></returns>
    public static Stream Stream(this Item item, FileMode fileMode)
    {
        LocalData.Semaphore.Wait();
        try
        {
            return File.Open(item.FullPath, fileMode);
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }

    }

    /// <summary>
    /// Tries to get stream for item in local data store
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="item"></param>
    /// <returns>false if item is not already in local data store</returns>
    public static bool TryStream(this Item item, [MaybeNullWhen(false)] out Stream stream, FileMode fileMode)
    {
        try
        {
            stream = item.Stream(fileMode);
            return true;
        }
        catch (Exception)
        {
            stream = null;
            return false;
        }

    }

    /// <summary>
    /// Get Stream, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<Stream> AssureSourcedStreamAsync(this AsynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return item.Stream(FileMode.Open);

        await item.ForcePullAsync();
        return item.Stream(FileMode.Open);
    }

    /// <summary>
    /// Get Stream, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static Stream AssureSourcedStream(this SynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return item.Stream(FileMode.Open);

        item.ForcePullAsync();
        return item.Stream(FileMode.Open);
    }


    /// <summary>
    /// Try to get Stream, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<Stream?> TryAssureSourcedStreamAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            return await item.AssureSourcedStreamAsync();
        }
        catch (Exception) { }

        return null;
    }

    /// <summary>
    /// Try to get Stream, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static Stream? TryAssureSourcedStream(this SynchronousSourcedItem item)
    {
        try
        {
            return item.AssureSourcedStream();
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
    public static void StoreStream(this Item item, Stream? sourceItem, bool wipeOld = true)
    {
        var file = item.File();
        if (!file.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        LocalData.Semaphore.Wait();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (sourceItem is null)
                return;

            using var fileStream = file.OpenWrite();
            sourceItem.CopyTo(fileStream);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            LocalData.Semaphore.Release();
        }

    }

    /// <summary>
    /// Store sourceItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    public static async Task StoreStreamAsync(this Item item, Stream? sourceItem, bool wipeOld = true)
    {
        var file = item.File();
        if (!file.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        await LocalData.Semaphore.WaitAsync();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (sourceItem is null)
                return;

            await using var fileStream = file.OpenWrite();
            await sourceItem.CopyToAsync(fileStream);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            LocalData.Semaphore.Release();
        }

    }

    /// <summary>
    /// Puts an sourceItem in local data store (null clears out the sourceItem)
    /// </summary>
    /// <param name="sourceItem">null to clear</param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public static bool TryStoreStream(this Item item, Stream sourceItem, bool wipeOld = true)
    {
        try
        {
            item.StoreStream(sourceItem, wipeOld);
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
    public static async Task<bool> TryStoreStreamAsync(this Item item, Stream sourceItem, bool wipeOld = true)
    {
        try
        {
            await item.StoreStreamAsync(sourceItem, wipeOld);
            return true;
        }
        catch (Exception) { }

        return false;
    }

    #endregion


}
