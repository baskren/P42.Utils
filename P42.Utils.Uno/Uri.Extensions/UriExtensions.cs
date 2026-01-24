
namespace P42.Utils.Uno;

[JetBrains.Annotations.UsedImplicitly]
public static class UriExtensions
{
    /// <param name="uri"></param>
    extension(Uri uri)
    {
        /// <summary>
        /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
        /// </summary>
        /// <returns>null on failure</returns>
        public async Task<StorageFile?> TryGetStorageFileAsync()
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
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public async Task<StorageFile> GetStorageFileAsync()
        {
            if (uri.Scheme.StartsWith("file", StringComparison.OrdinalIgnoreCase)) 
                return await StorageFile.GetFileFromPathAsync(uri.AbsolutePath);
            return await StorageFile.GetFileFromApplicationUriAsync(uri);
        }

        /// <summary>
        /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
        /// NOTE: Use TryAsStorageFileAsync when possible
        /// </summary>
        /// <returns>null on failure</returns>
        [JetBrains.Annotations.PublicAPI]
        public StorageFile GetStorageFile()
            => uri.GetStorageFileAsync().GetAwaiter().GetResult();

        /// <summary>
        /// Converts most local uris ("file:/", "ms-appdata:/", "ms-appx:" uris to a StorageFile
        /// NOTE: Use TryAsStorageFileAsync when possible
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns></returns>
        public bool TryGetStorageFile([System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out StorageFile storageFile)
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

        public async Task<string?> GetFilePathAsync()
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
                    "local/" => P42.Utils.Platform.ApplicationLocalFolderPath,
                    "temp/" => P42.Utils.Platform.ApplicationTemporaryFolderPath,
                    _ => throw new Exception($"invalid root folder [{uri}]")
                };
                return Path.Combine(dir, string.Join("", segments));
            }
        }

        private bool IsAppData()
            => uri.Scheme.Equals("ms-appdata", StringComparison.OrdinalIgnoreCase);

        // ReSharper disable once UnusedMember.Local
        private bool IsLocalResource()
            => uri.Scheme.Equals("ms-appx", StringComparison.OrdinalIgnoreCase);
    }

    // ReSharper disable once UnusedMember.Local
}
