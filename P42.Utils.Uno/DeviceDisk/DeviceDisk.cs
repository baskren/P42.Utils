using P42.Utils.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace P42.Utils.Uno;

internal class DeviceDisk : IDiskSpace
{

    private const string FreeSpace = "System.FreeSpace";
    private const string Capacity = "System.Capacity";


    public async Task<ulong> FreeAsync()
        => await GetAppFolderProperty<ulong>(FreeSpace);

    public async Task<ulong> SizeAsync()
        => await GetAppFolderProperty<ulong>(Capacity);

    public async Task<ulong> UsedAsync()
    {
        var properties = await GetAppFolderProperties([FreeSpace, Capacity]);
        var free = (ulong)properties[FreeSpace];
        var size = (ulong)properties[Capacity];
        return size - free;
    }

    private static async Task<IDictionary<string, object>> GetAppFolderProperties(string[] properties)
    {
        var task = ApplicationData.Current.LocalFolder.Properties.RetrievePropertiesAsync(properties);
        return await task.AsTask();
    }

    private static async Task<T> GetAppFolderProperty<T>(string property)
    {
        var retrivedProperties = await GetAppFolderProperties([property]);
        return (T)retrivedProperties[property];
    }

}
