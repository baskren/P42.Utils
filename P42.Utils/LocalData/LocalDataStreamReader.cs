using System.Threading.Tasks;

namespace P42.Utils;

/// <summary>
/// storage/recall of StreamReader data in local data store for common caching scenarios
/// WARNING: NO AUTOMATIC READ/WRITE SEMAPHORE LOCKING
/// </summary>
public class LocalDataStreamReader : LocalData<System.IO.StreamReader>
{
    internal static LocalDataStreamReader Instance { get; } = new();
    
    #region Recall

    /// <summary>
    /// Tries to get item out of local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>false if item is not already in cache</returns>
    public override bool TryRecallItem(out System.IO.StreamReader? item, ItemKey key)
    {
        if (Stream.TryRecallItem(out var stream, key) && stream is not null)
        {
            item = new System.IO.StreamReader(stream);
            return true;
        }
        
        item = null;
        return false;
    }

    /// <summary>
    /// Gets item from local data store or downloads it
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null if not available</returns>
    public override async Task<System.IO.StreamReader?> RecallOrPullItemAsync(ItemKey key)
        => await Stream.RecallOrPullItemAsync(key) is { } stream
            ? new System.IO.StreamReader(stream)
            : null;

    #endregion

    
    #region Store

    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="System.IO.IOException"></exception>
    public override void StoreItem(System.IO.StreamReader? sourceItem, ItemKey key, bool wipeOld = true)
       => Stream.StoreItem(sourceItem?.BaseStream, key, wipeOld);
    
    /// <summary>
    /// Store sourceItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    public override async Task StoreItemAsync(System.IO.StreamReader? sourceItem, ItemKey key, bool wipeOld = true)
        => await Stream.StoreItemAsync(sourceItem?.BaseStream, key, wipeOld);
    
    /// <summary>
    /// Stores sourceItem into local data store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public override bool TryStoreItem(System.IO.StreamReader? sourceItem, ItemKey key, bool wipeOld = true)
        => Stream.TryStoreItem(sourceItem?.BaseStream, key, wipeOld);
    
    /// <summary>
    /// Stores sourceItem into local data store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public override async Task<bool> TryStoreItemAsync(System.IO.StreamReader? sourceItem, ItemKey key, bool wipeOld = true)
        => await Stream.TryStoreItemAsync(sourceItem?.BaseStream, key, wipeOld);
    
    #endregion
    
}
