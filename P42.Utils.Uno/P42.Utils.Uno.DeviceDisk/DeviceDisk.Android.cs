using Android.OS;

namespace P42.Utils.Uno;

public partial class DeviceDisk
{
    public static Task<Dictionary<string, ulong>> NativeGetAppFolderPropertiesAsync(string[] properties)
    {
        return Task.Run(() =>
        {
            var result = new Dictionary<string, ulong>();
            var stat = new StatFs(ApplicationData.Current.LocalFolder.Path);

            foreach (var prop in properties)
            {
                result[prop] = prop switch
                {
                    Capacity => (ulong)stat.AvailableBytes,
                    FreeSpace => (ulong)stat.FreeBytes,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return result;
        });
    }
    
}
