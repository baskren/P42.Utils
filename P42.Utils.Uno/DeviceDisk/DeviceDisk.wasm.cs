namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public partial class DeviceDisk
{
    public static Task<Dictionary<string, ulong>> NativeGetAppFolderPropertiesAsync(string[] properties)
    {
        return Task.Run(() =>
        {
            var result = new Dictionary<string, ulong>();

            try
            {
                var json = Task.Run(WasmNative.StorageEstimateJsonAsync).Result;
                if (Serializer.Default.Deserialize<Dictionary<string, string>>(json) is not { } dict)
                    return result;

                var quota = GetUlongValue(dict, "quota");
                var usage = GetUlongValue(dict, "usage");

                foreach (var prop in properties)
                {
                    result[prop] = prop switch
                    {
                        Capacity => quota,
                        FreeSpace => quota - usage,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }

                return result;
            }
            catch (Exception)
            {
                // Ignore
            }

            return result;
        });
    }
    
    private static ulong GetUlongValue(IDictionary<string, string> dict, string key)
    {
        if (dict.TryGetValue(key, out var strVal) && ulong.TryParse(strVal, out var value))
            return value;
        return 0;
    }
}
