using System.Diagnostics.CodeAnalysis;
using static P42.Utils.LocalData;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class LocalDataStreamWriterExtensions
{

    #region StreamWriter

    /// <param name="item"></param>
    extension(Item item)
    {
        /// <summary>
        /// StreamWriter for LocalData.Item
        /// </summary>
        /// <returns></returns>
        public StreamWriter StreamWriter()
        {
            LocalData.Semaphore.Wait();
            try
            {
                return new StreamWriter(item.FullPath);
            }
            finally 
            {
                LocalData.Semaphore.Release();
            }

        }

        /// <summary>
        /// Tries to get StreamWriter for LocalData.Item
        /// </summary>
        /// <param name="reader"></param>
        /// <returns>false if item is not already in cache</returns>
        public bool TryStreamWriter([MaybeNullWhen(false)] out StreamWriter reader)
        {
            try
            {
                reader = item.StreamWriter();
                return true;
            }
            catch (Exception)
            {
                reader = null;
                return false;
            }
        
        }
    }

    #endregion


}
