namespace P42.Utils.Uno;

public partial class DeviceDisk
{
    public static async Task<Dictionary<string, ulong>> NativeGetAppFolderPropertiesAsync(string[] properties)
    {
        var result = new Dictionary<string, ulong>();
        
        try
        {
            if (await ApplicationData.Current.LocalFolder.Properties.RetrievePropertiesAsync(properties) is not { } dict)
                return result;

            foreach (var prop in properties)
            {
                result[prop] = prop switch
                {
                    DeviceDisk.Capacity => (ulong)dict[DeviceDisk.Capacity],
                    DeviceDisk.FreeSpace => (ulong)dict[DeviceDisk.FreeSpace],
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        
            return result;
        }
        catch (Exception)
        {
        }
        
        return result;
    }
    
}
