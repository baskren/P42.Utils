using System;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// storage/recall of directory content in local data store for common caching scenarios
/// WARNING: NO AUTOMATIC READ/WRITE SEMAPHORE LOCKING
/// </summary>
public class LocalDataDirectoryInfo : LocalData<System.IO.DirectoryInfo>
{
    /// <summary>
    /// Singleton instance of LocalDataDirectoryInfo
    /// </summary>
    internal static LocalDataDirectoryInfo Instance { get; } = new();

    #region Recall

    /// <summary>
    /// Get item from local data store IF it already is in local data store
    /// </summary>
    /// <param name="item"></param>
    /// <param name="key"></param>
    /// <returns>false if not in local data store</returns>
    public override bool TryRecallItem(out System.IO.DirectoryInfo? item, ItemKey key)
    {
        if (System.IO.Directory.Exists(key.Path))
        {
            item = new System.IO.DirectoryInfo(key.Path);
            return true;
        }

        item = null;
        return false;
    }

    public override async Task<System.IO.DirectoryInfo?> RecallOrPullItemAsync(ItemKey key)
    {
        if (!DirectoryExtensions.IsSupportedPackageExtension(key.Path))
        {
            if (System.IO.Directory.Exists(key.Path))
                return new System.IO.DirectoryInfo(key.Path);
            throw new Exception($"Key [{key}] is not already stored and does not have supported package extension");
        }
        
        var packagePath = key.Path;
        foreach (var unpackager in DirectoryExtensions.Unpackagers)
        {
            if (packagePath.EndsWith(unpackager.Key, StringComparison.OrdinalIgnoreCase))
                packagePath = packagePath[..^unpackager.Key.Length];
        }
        if (System.IO.Directory.Exists(packagePath))
            return new System.IO.DirectoryInfo(packagePath);

        if (!await key.TryRecallOrPullItemAsync() || !System.IO.File.Exists(key.Path))
            throw new Exception($"Source file for Key [{key}] cannot be retrieved");
        
        // Unpack package file and return directory
        if (await DirectoryExtensions.UnpackArchiveAsync(packagePath, key.Path) is { } unpackedFolderPath && !string.IsNullOrEmpty(unpackedFolderPath) && System.IO.Directory.Exists(unpackedFolderPath))
            return new System.IO.DirectoryInfo(unpackedFolderPath);

        return null;
    }
    
    #endregion


    #region Store

    /// <summary>
    /// StoreItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <exception cref="System.IO.IOException"></exception>
    public override void StoreItem(System.IO.DirectoryInfo? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var dir = new System.IO.DirectoryInfo(key.Path);
        if (!dir.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{dir.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        if (dir.Exists && wipeOld)
            dir.Delete(true);

        sourceItem?.Copy(dir, wipe: wipeOld);
    }
    
    /// <summary>
    /// Store sourceItem in LocalData store
    /// </summary>
    /// <param name="sourceItem"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    public override async Task StoreItemAsync(System.IO.DirectoryInfo? sourceItem, ItemKey key, bool wipeOld = true)
    {
        var dir = new System.IO.DirectoryInfo(key.Path);
        if (!dir.WritePossible(wipeOld))
            throw new System.IO.IOException($"DirectoryInfo [{dir.FullName}] exists but is not writable.  WipeOld=[{wipeOld}]]");

        if (dir.Exists && wipeOld)
            dir.Delete(true);

        await (sourceItem?.CopyAsync(dir, wipe: wipeOld) ?? Task.CompletedTask);
    }

    
    /// <summary>
    /// Trys to store contents of package file into local data store
    /// </summary>
    /// <param name="packageFile"></param>
    /// <param name="key"></param>
    /// <param name="wipeOld"></param>
    /// <returns>true on success</returns>
    public static async Task<bool> TryStorePackageAsync(System.IO.FileInfo packageFile, ItemKey key, bool wipeOld = true)
    {
        var dir = new System.IO.DirectoryInfo(key.Path);
        if (!dir.WritePossible(wipeOld))
            return false;

        if (!DirectoryExtensions.IsSupportedPackageExtension(packageFile.FullName))
        {
            QLog.Error($"The extension of the package file [{packageFile.FullName}] is not supported.");
            return false;
        }        
        
        try
        {
            if (dir.Exists && wipeOld)
                dir.Delete(true);
            
            await DirectoryExtensions.UnpackArchiveAsync(packageFile.FullName, key.Path);
            return true;
        }
        catch (Exception e)
        {
            QLog.Error(e);
            return false;
        }
        
    }
    
    #endregion
    
    
}
