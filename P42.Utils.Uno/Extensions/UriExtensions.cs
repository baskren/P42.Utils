using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;

namespace P42.Utils.Uno;
public static class UriExtensions
{
    /// <summary>
    /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
    /// </summary>
    /// <param name="uri"></param>
    /// <returns>null on failure</returns>
    public static async Task<StorageFile?> TryAsStorageFileAsync(this Uri uri)
    {
        try
        {
            return await uri.AsStorageFileAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public static async Task<StorageFile> AsStorageFileAsync(this Uri uri)
    {
        if (uri.Scheme.StartsWith("file", StringComparison.OrdinalIgnoreCase)) 
            return await StorageFile.GetFileFromPathAsync(uri.AbsolutePath);
        return await StorageFile.GetFileFromApplicationUriAsync(uri);
    }

    /// <summary>
    /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
    /// NOTE: Use TryAsStorageFileAsync when possible
    /// </summary>
    /// <param name="uri"></param>
    /// <returns>null on failure</returns>
    public static StorageFile AsStorageFile(this Uri uri)
        => uri.AsStorageFileAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
    /// NOTE: Use TryAsStorageFileAsync when possible
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="storageFile"></param>
    /// <returns></returns>
    public static bool TryAsStorageFile(this Uri uri, [MaybeNullWhen(false)] out StorageFile storageFile)
    {
        try
        {
            storageFile = uri.AsStorageFile();
            return true;
        }
        catch (Exception)
        {
            storageFile = null;
            return false;
        }
    }
}
