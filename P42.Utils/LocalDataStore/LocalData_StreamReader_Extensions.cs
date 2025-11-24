using System.Diagnostics.CodeAnalysis;
using static P42.Utils.LocalData;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class LocalDataStreamReaderExtensions
{

    #region StreamReader

    /// <param name="item"></param>
    extension(Item item)
    {
        /// <summary>
        /// StreamReader for LocalData.Item
        /// </summary>
        /// <returns></returns>
        public StreamReader StreamReader()
        {
            LocalData.Semaphore.Wait();
            try
            {
                return new StreamReader(item.FullPath);
            }
            finally { LocalData.Semaphore.Release(); }
        }

        /// <summary>
        /// Tries to get StreamReader for LocalData.Item
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>false if item is not already in cache</returns>
        // ReSharper disable once OutParameterValueIsAlwaysDiscarded.Global
        public bool TryStreamReader([MaybeNullWhen(false)] out StreamReader reader)
        {
            reader = null;
            if (!item.IsFile)
                return false;
        
            try
            {
                reader = item.StreamReader();
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
        /// Get StreamReader, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public async Task<StreamReader> AssureExistsStreamReaderAsync()
        {
            await item.AssureExistsAsync();
            return StreamReader(item);
        }

        /// <summary>
        /// Try to get StreamReader, pulling from source if not stored locally
        /// </summary>
        /// <returns>null on fail</returns>
        public async Task<StreamReader?> TryAssureExistsStreamReaderAsync()
        {
            try
            {
                return await item.AssureExistsStreamReaderAsync();
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
        /// Get StreamReader, pulling from source if not stored locally
        /// </summary>
        /// <returns></returns>
        public StreamReader AssureExistsStreamReader()
        {
            item.AssureExists();
            return StreamReader(item);
        }

        /// <summary>
        /// Try to get StreamReader, pulling from source if not stored locally
        /// </summary>
        /// <returns>null on fail</returns>
        public StreamReader? TryAssureExistsStreamReader()
        {
            try
            {
                return item.AssureExistsStreamReader();
            }
            catch (Exception)
            {
                return null;
            }
        
        }
    }

    #endregion



}
