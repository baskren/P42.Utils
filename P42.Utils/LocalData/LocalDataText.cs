using System;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// storage/recall of text in local data store for common caching scenarios
/// </summary>
public class LocalDataText : LocalData<string>
{
    /// <summary>
    /// Singleton instance of LocalDataText
    /// </summary>
    internal static LocalDataText Instance { get; } = new();
    
    #region Recall / Load

    /// <summary>
    /// Try recall item from local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public override bool TryRecallItem(out string? item, ItemKey key)
    {
        item = null;
        if (!System.IO.File.Exists(key.Path))
            return false;

        Semaphore.Wait();

        try
        {
            item = System.IO.File.ReadAllText(key.Path);
            return true;
        }
        catch (Exception e)
        {
            QLog.Error(e);
            return false;
        }
        finally
        {
            Semaphore.Release();
        }

    }

    /// <summary>
    /// Gets from local data store or loads from embedded resource if not in local data store
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public override async Task<string?> RecallOrPullItemAsync(ItemKey key)
    {
        if (!await key.TryRecallOrPullItemAsync() || !System.IO.File.Exists(key.Path))
            return null;

        await Semaphore.WaitAsync();
        try
        {
            return await System.IO.File.ReadAllTextAsync(key.Path);
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

    #region Store

    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="System.IO.IOException"></exception>
    public override void StoreItem(string? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var file = new System.IO.FileInfo(key.Path);
        if (!file.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        Semaphore.Wait();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (sourceItem != null)
                System.IO.File.WriteAllText(file.FullName, sourceItem);
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
    public override async Task StoreItemAsync(string? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var file = new System.IO.FileInfo(key.Path);
        if (!file.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");
        
        await Semaphore.WaitAsync();
        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (sourceItem != null)
                await System.IO.File.WriteAllTextAsync(file.FullName, sourceItem);
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
