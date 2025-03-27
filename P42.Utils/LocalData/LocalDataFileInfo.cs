using System;
using System.Threading.Tasks;

namespace P42.Utils;

/// <summary>
/// storage/recall of files in local data store for common caching scenarios.  WARNING: FILE SYSTEM LOCKING RISKS
/// WARNING: NO AUTOMATIC READ SEMAPHORE LOCKING
/// </summary>
public class LocalDataFileInfo : LocalData<System.IO.FileInfo> 
{

    /// <summary>
    /// Singleton for LocalDataFileInfo
    /// </summary>
    internal static LocalDataFileInfo Instance { get; } = new();

    #region Recall

    /// <summary>
    /// Tries to get item out of local data store, if exists
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>true if exists</returns>
    public override bool TryRecallItem(out System.IO.FileInfo? item, ItemKey key)
    {
        if (System.IO.File.Exists(key.Path))
        {
            item = new System.IO.FileInfo(key.Path);
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
    public override async Task<System.IO.FileInfo?> RecallOrPullItemAsync(ItemKey key)
        => await key.TryRecallOrPullItemAsync() && System.IO.File.Exists(key.Path)
            ? new System.IO.FileInfo(key.Path)
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
    public override void StoreItem(System.IO.FileInfo? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var file = new System.IO.FileInfo(key.Path);
        if (!file.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        Semaphore.Wait();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            sourceItem?.CopyTo(key.Path, wipeOld);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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
    public override async Task StoreItemAsync(System.IO.FileInfo? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var file = new System.IO.FileInfo(key.Path);
        if (!file.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        await Semaphore.WaitAsync();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            sourceItem?.CopyTo(key.Path, wipeOld);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            Semaphore.Release();
        }
    }
    

    #endregion
    
    
}
