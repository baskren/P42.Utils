namespace P42.Utils.Uno;
public static class UriExtensions
{
    /// <summary>
    /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
    /// </summary>
    /// <param name="uri"></param>
    /// <returns>null on failure</returns>
    public static async Task<StorageFile?> TryGetStorageFileAsync(this Uri uri)
    {
        try
        {
            return await uri.GetStorageFileAsync();
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
    public static async Task<StorageFile> GetStorageFileAsync(this Uri uri)
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
    public static StorageFile GetStorageFile(this Uri uri)
        => uri.GetStorageFileAsync().GetAwaiter().GetResult();

    /// <summary>
    /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
    /// NOTE: Use TryAsStorageFileAsync when possible
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="storageFile"></param>
    /// <returns></returns>
    public static bool TryGetStorageFile(this Uri uri, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out StorageFile storageFile)
    {
        try
        {
            storageFile = uri.GetStorageFile();
            return true;
        }
        catch (Exception)
        {
            storageFile = null;
            return false;
        }
    }

    public static async Task<string?> GetFilePathAsync(this Uri uri)
    {
        try
        {
            return await StorageFile.GetFileFromApplicationUriAsync(uri) is not {} storageFile 
                ? null 
                : storageFile.Path;
        }
        catch (Exception)
        {
            // because we're an Unpackaged WinAppSdk app?
            if (uri.IsFile)
                return uri.AbsolutePath;
            if (!uri.IsAppData())
                throw;
            
            var segments = uri.Segments.ToList();
            segments.RemoveRange(0, 2);
            var dir = uri.Segments[1].ToLower() switch
            {
                "local/" => Utils.Platform.ApplicationLocalFolderPath,
                "temp/" => Utils.Platform.ApplicationTemporaryFolderPath,
                _ => throw new Exception($"invalid root folder [{uri}]")
            };
            return Path.Combine(dir, string.Join("", segments));
        }
    }

    private static bool IsAppData(this Uri uri)
        => uri.Scheme.Equals("ms-appdata", StringComparison.OrdinalIgnoreCase);

    // ReSharper disable once UnusedMember.Local
    private static bool IsLocalResource(this Uri uri)
        => uri.Scheme.Equals("ms-appx", StringComparison.OrdinalIgnoreCase);
}
