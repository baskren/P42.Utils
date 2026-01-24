using static P42.Utils.LocalData;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class LocalDataTextExtensions
{


    #region Text

    /// <param name="item"></param>
    extension(Item item)
    {
        /// <summary>
        /// Recall text from item
        /// </summary>
        /// <returns></returns>
        public string? RecallText()
        {
            try
            {
                if (!item.IsFile) return null;
                LocalData.Semaphore.Wait();
                return File.ReadAllText(item.FullPath);
            }
            finally
            {
                LocalData.Semaphore.Release();
            }

        }

        public async Task<string?> RecallTextAsync()
        {
            try
            {
                if (!item.IsFile) return null;
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
        /// <returns></returns>
        // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Global
        public bool TryRecallText(out string? text)
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

        public async Task<(bool success, string? text)> TryRecallTextAsync()
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
        /// StoreItem in LocalData store
        /// </summary>
        /// <param name="text"></param>
        /// <param name="wipeOld"></param>
        /// <exception cref="IOException"></exception>
        public void StoreText(string? text, bool wipeOld = true)
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
        /// <param name="wipeOld"></param>
        public async Task StoreTextAsync(string? text, bool wipeOld = true)
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
        /// <param name="wipeOld"></param>
        /// <returns>true on success</returns>
        public bool TryStoreText(string? text, bool wipeOld = true)
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
        /// <param name="wipeOld"></param>
        /// <returns>true on success</returns>
        public async Task<bool> TryStoreTextAsync(string? text, bool wipeOld = true)
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
    }


    /// <param name="item"></param>
    extension(AsynchronousSourcedItem item)
    {
        /// <summary>
        /// Get Text, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public async Task<string?> AssureExistsTextAsync()
        {
            await item.AssureExistsAsync();
            return await item.RecallTextAsync();
        }

        /// <summary>
        /// Try to get Text, pulling from source if not stored locally
        /// </summary>
        /// <returns>null on fail</returns>
        public async Task<string?> TryAssureExistsTextAsync()
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
    }

    /// <param name="item"></param>
    extension(SynchronousSourcedItem item)
    {
        /// <summary>
        /// Get Text, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public string? AssureExistsText()
        {
            item.AssureExists();
            return item.RecallText();
        }

        /// <summary>
        /// Try to get Text, pulling from source if not stored locally
        /// </summary>
        /// <returns>null on fail</returns>
        public string? TryAssureExistsText()
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
    }

    #endregion


}
