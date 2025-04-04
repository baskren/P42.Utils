using System;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// storage/recall of Stream data in local data store for common caching scenarios
/// WARNING: NO AUTOMATIC READ SEMAPHORE LOCKING
/// </summary>
public class LocalDataStream : LocalData<System.IO.Stream>
{
    internal static LocalDataStream Instance { get; } = new();

    #region Recall

    /// <summary>
    /// Tries to get item out of local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>false if item is not already in local data store</returns>
    public override bool TryRecallItem(out System.IO.Stream? item, ItemKey key)
    {
        if (System.IO.File.Exists(key.FullPath))
        {
            item = System.IO.File.OpenRead(key.FullPath);
            return true;
        }
        
        item = null;
        return false;
    }
    
    /// <summary>
    /// Gets from local data store or loads from Uri if not in local data store
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null if not available</returns>
    public override async Task<System.IO.Stream?> RecallOrPullItemAsync(ItemKey key)
        => await key.TryRecallOrPullItemAsync() && System.IO.File.Exists(key.FullPath)
            ? System.IO.File.OpenRead(key.FullPath)
            : null;

    #endregion

    
    #region Store

    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="System.IO.IOException"></exception>
    public override void StoreItem(System.IO.Stream? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var file = new System.IO.FileInfo(key.FullPath);
        if (!file.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        Semaphore.Wait();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (sourceItem is null)
                return;

            using var fileStream = file.OpenWrite();
            sourceItem.CopyTo(fileStream);
        }
        catch (Exception e)
        {
            QLog.Error(e);
            throw;
        }
        finally
        {
            Semaphore.Release();
        }
        
    }
    
    /// <summary>
    /// Store sourceItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    public override async Task StoreItemAsync(System.IO.Stream? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var file = new System.IO.FileInfo(key.FullPath);
        if (!file.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        await Semaphore.WaitAsync();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (sourceItem is null)
                return;

            await using var fileStream = file.OpenWrite();
            await sourceItem.CopyToAsync(fileStream);
        }
        catch (Exception e)
        {
            QLog.Error(e);
            throw;
        }
        finally
        {
            Semaphore.Release();
        }
        
    }


    #endregion
    
    
}
