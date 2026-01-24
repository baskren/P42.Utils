using System.Diagnostics.CodeAnalysis;
using static P42.Utils.LocalData;

namespace P42.Utils;

public static class LocalDataFileInfoExtensions
{
    
    #region FileInfo

    /// <param name="item"></param>
    extension(Item item)
    {
        /// <summary>
        /// Returns the FileInfo for a LocalData.Item
        /// </summary>
        /// <returns></returns>
        public FileInfo File() => !item.IsDirectory ? new FileInfo(item.FullPath) : throw new Exception($"Item [{item}] is a Directory");

        /// <summary>
        /// Tries to get FileInfo for LocalData.Item
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns>true if exists</returns>
        public bool TryFile([MaybeNullWhen(false)] out FileInfo fileInfo)
        {
            try
            {
                fileInfo = item.File();
                return true;
            }
            catch (Exception)
            {
                fileInfo = null;
                return false;
            }
        }

        /// <summary>
        /// Stores sourceItem to LocalData.Item 
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="wipeOld"></param>
        /// <exception cref="IOException"></exception>
        [JetBrains.Annotations.PublicAPI]
        public void StoreFile(FileInfo? sourceItem, bool wipeOld = true)
        {
            var file = new FileInfo(item.FullPath);
            if (!file.WritePossible(wipeOld))
                throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

            LocalData.Semaphore.Wait();

            try
            {
                if (file.Exists && wipeOld)
                    file.Delete();

                item.AssureExistsParentDirectory();
                sourceItem?.CopyTo(item.FullPath, wipeOld);
            }
            finally { LocalData.Semaphore.Release(); }

        }

        /// <summary>
        /// Stores sourceItem to LocalData.Item 
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="wipeOld"></param>
        [JetBrains.Annotations.PublicAPI]
        public async Task StoreFileAsync(FileInfo? sourceItem, bool wipeOld = true)
        {
            var file = new FileInfo(item.FullPath);
            if (!file.WritePossible(wipeOld))
                throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

            await LocalData.Semaphore.WaitAsync();

            try
            {
                if (file.Exists && wipeOld)
                    file.Delete();

                item.AssureExistsParentDirectory();
                sourceItem?.CopyTo(item.FullPath, wipeOld);
            }
            finally { LocalData.Semaphore.Release(); }

        }

        /// <summary>
        /// Puts an sourceItem in local data store (null clears out the sourceItem)
        /// </summary>
        /// <param name="sourceItem">null to clear</param>
        /// <param name="wipeOld"></param>
        /// <returns>true on success</returns>
        public bool TryStoreFile(FileInfo sourceItem, bool wipeOld = true)
        {
            try
            {
                item.StoreFile(sourceItem, wipeOld);
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
        public async Task<bool> TryStoreFileAsync(FileInfo sourceItem, bool wipeOld = true)
        {
            try
            {
                await item.StoreFileAsync(sourceItem, wipeOld);
                return true;
            }
            catch (Exception) 
            { 
                return false;
            }

        }
    }

    #endregion


    /// <param name="item"></param>
    extension(AsynchronousSourcedItem item)
    {
        /// <summary>
        /// Get FileInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public async Task<FileInfo> AssureExistsFileAsync()
        {
            await item.AssureExistsAsync();
            return item.File();
        }

        /// <summary>
        /// Try to get FileInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public async Task<FileInfo?> TryAssureExistsFileAsync()
        {
            try
            {
                return await item.AssureExistsFileAsync();
            }
            catch (Exception)
            {
                return null;
            }
        
        }
    }

    /// <param name="item"></param>
    extension(SynchronousSourcedItem item)
    {
        /// <summary>
        /// Get FileInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        [JetBrains.Annotations.PublicAPI]
        public FileInfo AssureExistsFile()
        {
            item.AssureExists();
            return item.File();
        }

        /// <summary>
        /// Try to get FileInfo, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public FileInfo? TryAssureExistsFile()
        {
            try
            {
                return item.AssureExistsFile();
            }
            catch (Exception)
            {
                return null;
            }
        
        }
    }
}
