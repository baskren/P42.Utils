using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P42.Utils.LocalData;

namespace P42.Utils;

public static class LocalData_StreamReader_Extensions
{

    #region StreamReader

    /// <summary>
    /// StreamReader for LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static StreamReader StreamReader(this Item item)
    {
        LocalData.Semaphore.Wait();
        try
        {
            return new(item.FullPath);
        }
        catch (Exception) { throw; }
        finally { LocalData.Semaphore.Release(); }
    }

    /// <summary>
    /// Tries to get StreamReader for LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>false if item is not already in cache</returns>
    public static bool TryStreamReader(this Item item, [MaybeNullWhen(false)] out StreamReader reader)
    {
        try
        {
            reader = item.StreamReader();
            return true;
        }
        catch (Exception) { }

        reader = null;
        return false;
    }

    /// <summary>
    /// Get StreamReader, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static async Task<StreamReader> AssureSourcedStreamReaderAsync(this AsynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return StreamReader(item);

        await item.ForcePullAsync();

        return StreamReader(item);
    }

    /// <summary>
    /// Get StreamReader, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static StreamReader AssureSourcedStreamReader(this SynchronousSourcedItem item)
    {
        if (item.Exists && item.IsFile)
            return StreamReader(item);

        item.ForcePull();

        return StreamReader(item);
    }


    /// <summary>
    /// Try to get StreamReader, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="reader"></param>
    /// <returns>null on fail</returns>
    public static async Task<StreamReader?> TryAssureSourcedStreamReaderAsync(this AsynchronousSourcedItem item)
    {
        try
        {
            return await item.AssureSourcedStreamReaderAsync();
        }
        catch (Exception) { }

        return null;
    }

    /// <summary>
    /// Try to get StreamReader, pulling from source if not stored locally
    /// </summary>
    /// <param name="item"></param>
    /// <param name="reader"></param>
    /// <returns>null on fail</returns>
    public static StreamReader? TryAssureSourcedStreamReader(this SynchronousSourcedItem item)
    {
        try
        {
            return item.AssureSourcedStreamReader();
        }
        catch (Exception) { }

        return null;
    }

    #endregion



}
