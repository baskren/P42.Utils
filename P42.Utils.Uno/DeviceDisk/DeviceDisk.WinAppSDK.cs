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
                    Capacity => (ulong)dict[Capacity],
                    FreeSpace => (ulong)dict[FreeSpace],
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        
            return result;
        }
        catch (Exception)
        {
            // ignored
        }

        return result;
    }
    
}
