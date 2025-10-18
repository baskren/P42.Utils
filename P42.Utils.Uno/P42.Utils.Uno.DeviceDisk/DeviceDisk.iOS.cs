using Foundation;

namespace P42.Utils.Uno;

public partial class DeviceDisk
{

    public static Task<Dictionary<string, ulong>> NativeGetAppFolderPropertiesAsync(string[] properties)
    {
        return Task.Run(() =>
        {
            var result = new Dictionary<string, ulong>();
            // Ask iOS for filesystem attributes
            var attributes =
                NSFileManager.DefaultManager.GetFileSystemAttributes(ApplicationData.Current.LocalFolder.Path,
                    out var error);

            if (attributes is null || !string.IsNullOrWhiteSpace(error.LocalizedDescription))
            {
                Console.WriteLine($"Error getting file system attributes: {error.LocalizedDescription}");
                foreach (var prop in properties)
                    result[prop] = 0;
                return result;
            }

            foreach (var prop in properties)
            {
                result[prop] = prop switch
                {
                    Capacity => attributes.Size,
                    FreeSpace => attributes.FreeSize,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            return result;
        });
    }
}
