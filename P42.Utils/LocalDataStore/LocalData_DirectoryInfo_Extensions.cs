using System.Diagnostics.CodeAnalysis; 
using static P42.Utils.LocalData;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class LocalDataDirectoryInfoExtensions
{
    #region DirectoryInfo

    /// <param name="item"></param>
    extension(Item item)
    {
        /// <summary>
        /// Returns the FileInfo for a LocalData.Item
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo Directory() 
            => !item.IsFile 
                ? new DirectoryInfo(item.FullPath) 
                : throw new Exception($"Item [{item}] is a File");

        /// <summary>
        /// Tries to get FileInfo for LocalData.Item
        /// </summary>
        /// <param name="dirInfo"></param>
        /// <returns>true if exists</returns>
        public bool TryDirectory([MaybeNullWhen(false)] out DirectoryInfo dirInfo)
        {
            try
            {
                dirInfo = item.Directory();
                return true;
            }
            catch (Exception)
            {
                dirInfo = null;
                return false;
            }
        }
        
        public async Task<DirectoryInfo?> UnpackAsync()
        {
            if (!DirectoryExtensions.IsSupportedPackageExtension(item.FullPath))
            {
                return System.IO.Directory.Exists(item.FullPath) 
                    ? new DirectoryInfo(item.FullPath) 
                    : throw new Exception($"Key [{item}] is not already stored and does not have supported package extension");
            }

            var packagePath = item.FullPath;
            foreach (var unpackager in DirectoryExtensions.Unpackagers
                         .Where(unpackager => item.FullPath.EndsWith(unpackager.Key, StringComparison.OrdinalIgnoreCase)))
                packagePath = packagePath[..^unpackager.Key.Length];

            if (System.IO.Directory.Exists(packagePath))
                return new DirectoryInfo(packagePath);

            if (!File.Exists(item.FullPath))
                return null;

            // Unpack package file and return directory
            if (await DirectoryExtensions.UnpackArchiveAsync(packagePath, item.FullPath) is { } unpackedFolderPath 
                && !string.IsNullOrEmpty(unpackedFolderPath) 
                && System.IO.Directory.Exists(unpackedFolderPath))
                return new DirectoryInfo(unpackedFolderPath);

            return null;

        }


        /// <summary>
        /// StoreItem in LocalData store
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="wipeOld"></param>
        /// <exception cref="IOException"></exception>
        public void StoreDirectory(DirectoryInfo? sourceItem, bool wipeOld = true)
        {
            var dir = new DirectoryInfo(item.FullPath);
            if (!dir.WritePossible(wipeOld))
                throw new IOException($"DirectoryInfo [{dir.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

            LocalData.Semaphore.Wait();

            try
            {
                if (dir.Exists && wipeOld)
                    dir.Delete(true);

                item.AssureExistsParentDirectory();
                sourceItem?.Copy(dir, wipe: wipeOld);
            }
            finally { LocalData.Semaphore.Release(); }

        }

        /// <summary>
        /// Store sourceItem in LocalData store
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="wipeOld"></param>
        public async Task StoreDirectoryAsync(DirectoryInfo? sourceItem, bool wipeOld = true)
        {
            var dir = new DirectoryInfo(item.FullPath);
            if (!dir.WritePossible(wipeOld))
                throw new IOException($"DirectoryInfo [{dir.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

            await LocalData.Semaphore.WaitAsync();

            try
            {
                if (dir.Exists && wipeOld)
                    dir.Delete(true);

                item.AssureExistsParentDirectory();
                await (sourceItem?.CopyAsync(dir, wipe: wipeOld) ?? Task.CompletedTask);
            }
            finally { LocalData.Semaphore.Release(); }
        }

        /// <summary>
        /// Puts an sourceItem in local data store (null clears out the sourceItem)
        /// </summary>
        /// <param name="sourceItem">null to clear</param>
        /// <param name="wipeOld"></param>
        /// <returns>true on success</returns>
        public bool TryStoreDirectory(DirectoryInfo? sourceItem, bool wipeOld = true)
        {
            try
            {
                item.StoreDirectory(sourceItem, wipeOld);
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
        /// <param name="sourceItem">null to clear</param>
        /// <param name="wipeOld"></param>
        /// <returns>true on success</returns>
        public async Task<bool> TryStoreDirectoryAsync(DirectoryInfo sourceItem, bool wipeOld = true)
        {
            try
            {
                await item.StoreDirectoryAsync(sourceItem, wipeOld);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }


    /// <param name="item"></param>
    extension(AsynchronousSourcedItem item)
    {
        /// <summary>
        /// Get DirectoryInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public async Task<DirectoryInfo> AssureExistsDirectoryAsync()
        {
            if (item is { Exists: true, IsDirectory: true })
                return item.Directory();

            await item.RefreshAsync();
            return item.Directory();
        }

        /// <summary>
        /// Try to get DirectoryInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public async Task<DirectoryInfo?> TryAssureExistsDirectoryAsync()
        {
            try
            {
                return await item.AssureExistsDirectoryAsync();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public async Task<DirectoryInfo?> AssureUnpackAsync()
        {
            if (!DirectoryExtensions.IsSupportedPackageExtension(item.FullPath))
            {
                if (System.IO.Directory.Exists(item.FullPath))
                    return new DirectoryInfo(item.FullPath);
                throw new Exception($"Key [{item}] is not already stored and does not have supported package extension");
            }

            var packagePath = item.FullPath;
            foreach (var unpackager in DirectoryExtensions.Unpackagers)
            {
                if (packagePath.EndsWith(unpackager.Key, StringComparison.OrdinalIgnoreCase))
                    packagePath = packagePath[..^unpackager.Key.Length];
            }

            if (System.IO.Directory.Exists(packagePath))
                return new DirectoryInfo(packagePath);

            if (!await item.TryRefreshAsync() || !File.Exists(item.FullPath))
                throw new Exception($"Source file for Key [{item}] cannot be retrieved");

            // Unpack package file and return directory
            if (await DirectoryExtensions.UnpackArchiveAsync(packagePath, item.FullPath) is { } unpackedFolderPath && !string.IsNullOrEmpty(unpackedFolderPath) && System.IO.Directory.Exists(unpackedFolderPath))
                return new DirectoryInfo(unpackedFolderPath);

            return null;

        }
    }


    /// <param name="item"></param>
    extension(SynchronousSourcedItem item)
    {
        /// <summary>
        /// Get DirectoryInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo AssureExistsDirectory()
        {
            if (item is { Exists: true, IsDirectory: true })
                return item.Directory();

            item.Refresh();
            return item.Directory();
        }

        /// <summary>
        /// Try to get DirectoryInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public DirectoryInfo? TryAssureExistsDirectory()
        {
            try
            {
                return item.AssureExistsDirectory();
            }
            catch (Exception) 
            { 
                return null;
            }

        }
    }

    #endregion



}
