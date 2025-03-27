using P42.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace P42.Utils.Uno;

public class DeviceDisk : IDiskSpace
{
    /// <summary>
    /// How much space is available on the disk
    /// </summary>
    public ulong Free
         => GetFreeSpace().GetAwaiter().GetResult();

    /// <summary>
    /// How bid is the disk
    /// </summary>
    public ulong Size 
        => GetCapacity().GetAwaiter().GetResult();

    /// <summary>
    /// How much space is utilized
    /// </summary>
    public ulong Used
        => GetUsedSpace().GetAwaiter().GetResult();

    private static async Task<T> GetAppFolderProperty<T>(string property)
    {
        var retrivedProperties = await GetAppFolderProperties([property]);
        return (T)retrivedProperties[property];
    }

    private const string FreeSpace = "System.FreeSpace";
    private const string Capacity = "System.Capacity";
    private static async Task<ulong> GetFreeSpace()
        => await GetAppFolderProperty<ulong>(FreeSpace);

    private static async Task<ulong> GetCapacity()
        => await GetAppFolderProperty<ulong>(Capacity);

    private static async Task<IDictionary<string, object>> GetAppFolderProperties(string[] properties)
    {
        var task = ApplicationData.Current.LocalFolder.Properties.RetrievePropertiesAsync(properties);
        return await task.AsTask();
    }

    private static async Task<ulong> GetUsedSpace()
    {
        var properties = await GetAppFolderProperties([FreeSpace, Capacity]);
        var free = (ulong)properties[FreeSpace];
        var size = (ulong)properties[Capacity];
        return size - free;
    }
}
