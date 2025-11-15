using P42.Serilog.QuickLog;
using static P42.Utils.LocalData;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class LocalDataTextExtensions
{


    #region Text

    /// <summary>
    /// Recall text from item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string? RecallText(this Item item)
    {
        try
        {
            LocalData.Semaphore.Wait();
            return File.ReadAllText(item.FullPath);
        }
        finally
        {
            LocalData.Semaphore.Release();
        }

    }

    public static async Task<string?> RecallTextAsync(this Item item)
    {
        try
        {
            await LocalData.Semaphore.WaitAsync();
            return await File.ReadAllTextAsync(item.FullPath);
        }
        finally
        {
            LocalData.Semaphore.Release();
        }
    }


    /// <summary>
    /// Try recall item from local data store
    /// </summary>
    /// <param name="text"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool TryRecallText(this Item item, out string? text)
    {
        text = null;
        if (!item.IsFile) return false;
        
        try
        {
            text = item.RecallText();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        
    }

    public static async Task<(bool success, string? text)> TryRecallTextAsync(this Item item)
    {
        
        if (!item.IsFile)
            return (false, null);
        
        try
        {
            var text = await item.RecallTextAsync();
            return (true, text);
        }
        catch (Exception)
        {
            return (false, null);
        }
        
    }

    /// <summary>
    /// Get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<string?> AssureExistsTextAsync(this AsynchronousSourcedItem item)
    {
        await item.AssureExistsAsync();
        return await RecallTextAsync(item);
    }

    /// <summary>
    /// Get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string? AssureExistsText(this SynchronousSourcedItem item)
    {
        item.AssureExists();
        return RecallText(item);
    }


    /// <summary>
    /// Try to get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns>null on fail</returns>
    public static async Task<string?> TryAssureExistsTextAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            return await item.AssureExistsTextAsync();
        }
        catch (Exception)
        {
            return null;
        }
        
    }

    /// <summary>
    /// Try to get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns>null on fail</returns>
    public static string? TryAssureExistsText(this SynchronousSourcedItem item)
    {
        try
        {
            return item.AssureExistsText();
        }
        catch (Exception)
        {
            return null;
        }
        
    }

    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="text"></param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="IOException"></exception>
    public static void StoreText(this Item item, string? text, bool wipeOld = true)
    {
        var file = item.File();
        if (!file.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        LocalData.Semaphore.Wait();

        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (text is null)
                return;

            item.AssureExistsParentDirectory();
            File.WriteAllText(file.FullName, text);
        }
        finally
        {
            LocalData.Semaphore.Release();
        }

    }

    /// <summary>
    /// Store sourceItem in LocalData store
    /// </summary>
    /// <param name="text"></param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    public static async Task StoreTextAsync(this Item item, string? text, bool wipeOld = true)
    {
        var file = item.File();
        if (!file.WritePossible(wipeOld))
            throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        await LocalData.Semaphore.WaitAsync();
        try
        {
            if (file.Exists && wipeOld)
                file.Delete();

            if (text != null)
            {
                item.AssureExistsParentDirectory();
                await File.WriteAllTextAsync(file.FullName, text);
            }
        }
        finally
        {
            LocalData.Semaphore.Release();
        }

    }

    /// <summary>
    /// Puts an sourceItem in local data store (null clears out the sourceItem)
    /// </summary>
    /// <param name="text">null to clear</param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public static bool TryStoreText(this Item item, string? text, bool wipeOld = true)
    {
        try
        {
            item.StoreText(text, wipeOld);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        
    }


    /// <summary>
    /// Puts an sourceItem in local data store (null clears out the sourceItem)
    /// </summary>
    /// <param name="text">null to clear</param>
    /// <param name="item"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public static async Task<bool> TryStoreTextAsync(this Item item, string? text, bool wipeOld = true)
    {
        try
        {
            await item.StoreTextAsync(text, wipeOld);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
        
    }

    #endregion


}
