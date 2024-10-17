using P42.Utils.Interfaces;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace P42.Utils.Uno;

/// <summary>
/// Disk Space utilities
/// </summary>
public class DiskSpace : IDiskSpace
{
    /// <summary>
    /// Free disk space
    /// </summary>
    public ulong Free
    {
        get
        {
            var task = GetFreeSpaceAsync();
            var waiter = task.GetAwaiter();
            var result = waiter.GetResult();
            return result;
        }
    }

    /// <summary>
    /// Size of disk
    /// </summary>
    public ulong Size
    {
        get
        {
            var task = GetCapacityAsync();
            var waiter = task.GetAwaiter();
            var result = waiter.GetResult();
            return result;
        }
    }

    /// <summary>
    /// Used disk space (Size-Free)
    /// </summary>
    public ulong Used => Size - Free;
    

    private static async Task<ulong> GetAppFolderProperty(string property)
    {
        var local = ApplicationData.Current.LocalFolder;
        var task = local.Properties.RetrievePropertiesAsync([property]);
        var retrievedProperties = await task.AsTask();
        return (ulong)retrievedProperties[property];
    }

    /// <summary>
    /// Get free disk space
    /// </summary>
    /// <returns></returns>
    public async Task<ulong> GetFreeSpaceAsync()
        => await GetAppFolderProperty("System.FreeSpace");

    /// <summary>
    /// Get disk capacity
    /// </summary>
    /// <returns></returns>
    public async Task<ulong> GetCapacityAsync()
        => await GetAppFolderProperty("System.Capacity");



}
