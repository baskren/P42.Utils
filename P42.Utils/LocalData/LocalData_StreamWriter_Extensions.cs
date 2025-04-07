using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static P42.Utils.LocalData;

namespace P42.Utils;

public static class LocalData_StreamWriter_Extensions
{

    #region StreamWriter

    /// <summary>
    /// StreamWriter for LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public static StreamWriter StreamWriter(this Item item)
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
    /// Tries to get StreamWriter for LocalData.Item
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>false if item is not already in cache</returns>
    public static bool TryStreamWriter(this Item item, [MaybeNullWhen(false)] out StreamWriter reader)
    {
        try
        {
            reader = item.StreamWriter();
            return true;
        }
        catch (Exception) { }

        reader = null;
        return false;
    }

    #endregion


}
