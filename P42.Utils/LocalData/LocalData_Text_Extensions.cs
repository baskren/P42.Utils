using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P42.Utils.LocalData;

namespace P42.Utils;

public static class LocalData_Text_Extensions
{


    #region Text

    /// <summary>
    /// Recall text from item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string RecallText(this Item item)
    {
        try
        {
            LocalData.Semaphore.Wait();
            return File.ReadAllText(item.FullPath);
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }

    }


    /// <summary>
    /// Try recall item from local data store
    /// </summary>
    /// <param name="text"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static bool TryRecallText(this Item item, out string text)
    {
        text = string.Empty;
        if (!File.Exists(item.FullPath))
            return false;


        try
        {
            text = item.RecallText();
            return true;
        }
        catch (Exception) { }

        return false;
    }

    /// <summary>
    /// Get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<string> AssureSourcedTextAsync(this AsynchronousSourcedItem item)
    {
        if (item is { Exists: true, IsFile: true })
            return RecallText(item);

        await item.ForcePullAsync();

        return RecallText(item);
    }

    /// <summary>
    /// Get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static string AssureSourcedText(this SynchronousSourcedItem item)
    {
        if (item is { Exists: true, IsFile: true })
            return RecallText(item);

        item.ForcePullAsync();

        return RecallText(item);
    }


    /// <summary>
    /// Try to get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="text"></param>
    /// <returns>null on fail</returns>
    public static async Task<string?> TryAssureSourcedTextAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            return await item.AssureSourcedTextAsync();
        }
        catch (Exception) { }
            
        return null;
    }

    /// <summary>
    /// Try to get Text, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="text"></param>
    /// <returns>null on fail</returns>
    public static string? TryAssureSourcedText(this SynchronousSourcedItem item)
    {
        try
        {
            return item.AssureSourcedText();
        }
        catch (Exception) { }

        return null;
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

            if (text != null)
            {
                item.AssureParentDirectory();
                File.WriteAllText(file.FullName, text);
            }
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }

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
                item.AssureParentDirectory();
                await File.WriteAllTextAsync(file.FullName, text);
            }
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }

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
        catch (Exception) { }

        return false;
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
        catch (Exception) { }

        return false;
    }

    #endregion


}
