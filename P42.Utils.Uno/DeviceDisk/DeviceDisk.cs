using P42.Utils.Interfaces;

namespace P42.Utils.Uno;

public partial class DeviceDisk : IDiskSpace
{

    internal const string FreeSpace = "System.FreeSpace";
    internal const string Capacity = "System.Capacity";


    public async Task<ulong> FreeAsync()
        => await GetAppFolderProperty(FreeSpace);

    public async Task<ulong> SizeAsync()
        => await GetAppFolderProperty(Capacity);

    public async Task<ulong> UsedAsync()
    {
        var properties = await NativeGetAppFolderPropertiesAsync([FreeSpace, Capacity]);
        var free = properties[FreeSpace];
        var size = properties[Capacity];
        return size - free;
    }
    
    private static async Task<ulong> GetAppFolderProperty(string property)
    {
        var retrievedProperties = await NativeGetAppFolderPropertiesAsync([property]);
        return retrievedProperties[property];
    }

    //public static partial Task<Dictionary<string, ulong>> NativeGetAppFolderPropertiesAsync(string[] properties);

}
