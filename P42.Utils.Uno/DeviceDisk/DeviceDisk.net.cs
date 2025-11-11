namespace P42.Utils.Uno;

public partial class DeviceDisk
{
    public static Task<Dictionary<string, ulong>> NativeGetAppFolderPropertiesAsync(string[] properties)
        => Task.FromResult(new Dictionary<string, ulong>());

}
