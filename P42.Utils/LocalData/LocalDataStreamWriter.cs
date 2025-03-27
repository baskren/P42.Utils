using System;
using System.Threading.Tasks;

namespace P42.Utils;

/// <summary>
/// Recall of StreamWriter data in local data store for common caching scenarios
/// WARNING: NO AUTOMATIC READ/WRITE SEMAPHORE LOCKING
/// </summary>

public class LocalDataStreamWriter : LocalData<System.IO.StreamWriter>
{
    internal static LocalDataStreamWriter Instance { get; } = new();
    
    #region Recall / Load

    /// <summary>
    /// Tries to get item out of local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>false if item is not already in local data store</returns>
    public override bool TryRecallItem(out System.IO.StreamWriter? item, ItemKey key)
    {
        if (LocalDataStream.Instance.TryRecallItem(out var stream, key) && stream is not null)
        {
            item = new System.IO.StreamWriter(stream);
            return true;
        }
        
        item = null;
        return false;
    }
    
    /// <summary>
    /// Gets or downloads item to LocalData store and returns a StreamWriter pointing to it 
    /// </summary>
    /// <param name="key"></param>
    /// <returns>null if not available</returns>
    public override async Task<System.IO.StreamWriter?> RecallOrPullItemAsync(ItemKey key)
        => await LocalDataStream.Instance.RecallOrPullItemAsync(key) is {} stream
            ? new System.IO.StreamWriter(stream)
            : null;
    #endregion

    
    #region Store

    #pragma warning disable CS0809 // Obsolete member overrides non-obsolete member

    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="System.IO.IOException"></exception>
    [Obsolete("INVALID: Store of StreamWriter doesn't make sense.", true)]
    public override void StoreItem(System.IO.StreamWriter? sourceItem, ItemKey key, bool wipeOld = true)
        => throw new NotImplementedException();

    /// <summary>
    /// Store sourceItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    [Obsolete("INVALID: Store of StreamWriter doesn't make sense.", true)]
    public override Task StoreItemAsync(System.IO.StreamWriter? sourceItem, ItemKey key, bool wipeOld = true)
        => throw new NotImplementedException();

    /// <summary>
    /// INVALID
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <returns>NotImplementedException</returns>
    [Obsolete("INVALID: Store of StreamWriter doesn't make sense.", true)]
    public override Task<bool> TryStoreItemAsync(System.IO.StreamWriter? sourceItem, ItemKey key,
        bool wipeOld = true)
        => throw new NotImplementedException();


    /// <summary>
    /// INVALID
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <returns>NotImplementedException</returns>
    [Obsolete("INVALID: Store of StreamWriter doesn't make sense.", true)]
    public override bool TryStoreItem(System.IO.StreamWriter? sourceItem, ItemKey key, bool wipeOld = true)
        => throw new NotImplementedException();

    #pragma warning restore CS0809 // Obsolete member overrides non-obsolete member
    
    #endregion

}
