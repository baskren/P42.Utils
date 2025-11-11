
namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string GetManufacturer() 
        => string.Empty;

    private static string GetModel() 
        => string.Empty;

    private static string GetDeviceName()
        => string.Empty;

    private static string GetDeviceId()
        => string.Empty;

    private static bool GetIsEmulator() => false;
    
    public static string QueryDeviceOs()
        => FallbackQueryDeviceOs();

    public static string QueryDeviceOsVersion()
        => FallbackQueryDeviceOsVersion();

}
