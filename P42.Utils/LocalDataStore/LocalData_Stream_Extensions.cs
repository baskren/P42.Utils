using System.Diagnostics.CodeAnalysis;
using static P42.Utils.LocalData;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class LocalDataStreamExtensions
{

    #region Stream

    /// <param name="item"></param>
    extension(Item item)
    {
        /// <summary>
        /// Get stream for item in local data store
        /// </summary>
        /// <param name="fileMode"></param>
        /// <returns></returns>
        public Stream Stream(FileMode fileMode)
        {
            LocalData.Semaphore.Wait();
            try
            {
                return File.Open(item.FullPath, fileMode);
            }
            finally { LocalData.Semaphore.Release(); }

        }

        /// <summary>
        /// Tries to get stream for item in local data store
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileMode"></param>
        /// <returns>false if item is not already in local data store</returns>
        public bool TryStream([MaybeNullWhen(false)] out Stream stream, FileMode fileMode)
        {
            try
            {
                stream = item.Stream(fileMode);
                return true;
            }
            catch (Exception)
            {
                stream = null;
                return false;
            }

        }

        /// <summary>
        /// StoreItem in LocalData store
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="wipeOld"></param>
        /// <exception cref="IOException"></exception>
        public void StoreStream(Stream? sourceItem, bool wipeOld = true)
        {
            var file = item.File();
            if (!file.WritePossible(wipeOld))
                throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

            LocalData.Semaphore.Wait();

            try
            {
                if (file.Exists && wipeOld)
                    file.Delete();

                if (sourceItem is null)
                    return;

                item.AssureExistsParentDirectory();
                using var fileStream = file.OpenWrite();
                sourceItem.CopyTo(fileStream);
            }
            finally
            {
                LocalData.Semaphore.Release();
            }

        }

        /// <summary>
        /// Store sourceItem in LocalData store
        /// </summary>
        /// <param name="sourceItem"></param>
        /// <param name="wipeOld"></param>
        public async Task StoreStreamAsync(Stream? sourceItem, bool wipeOld = true)
        {
            var file = item.File();
            if (!file.WritePossible(wipeOld))
                throw new IOException($"DirectoryInfo [{file.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

            await LocalData.Semaphore.WaitAsync();

            try
            {
                if (file.Exists && wipeOld)
                    file.Delete();

                if (sourceItem is null)
                    return;

                item.AssureExistsParentDirectory();
                await using var fileStream = file.OpenWrite();
                await sourceItem.CopyToAsync(fileStream);
            }
            finally
            {
                LocalData.Semaphore.Release();
            }

        }

        /// <summary>
        /// Puts an sourceItem in local data store (null clears out the sourceItem)
        /// </summary>
        /// <param name="sourceItem">null to clear</param>
        /// <param name="wipeOld"></param>
        /// <returns>true on success</returns>
        public bool TryStoreStream(Stream sourceItem, bool wipeOld = true)
        {
            try
            {
                item.StoreStream(sourceItem, wipeOld);
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
        public async Task<bool> TryStoreStreamAsync(Stream sourceItem, bool wipeOld = true)
        {
            try
            {
                await item.StoreStreamAsync(sourceItem, wipeOld);
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
        /// Get Stream, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> AssureExistsStreamAsync()
        {
            await item.AssureExistsAsync();
            return item.Stream(FileMode.Open);
        }

        /// <summary>
        /// Try to get Stream, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public async Task<Stream?> TryAssureExistsStreamAsync()
        {
            try
            {
                return await item.AssureExistsStreamAsync();
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
        /// Get Stream, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public Stream AssureExistsStream()
        {
            item.AssureExists();
            return item.Stream(FileMode.Open);
        }

        /// <summary>
        /// Try to get Stream, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public Stream? TryAssureExistsStream()
        {
            try
            {
                return item.AssureExistsStream();
            }
            catch (Exception)
            {
                return null;
            }
        
        }
    }

    #endregion


}
