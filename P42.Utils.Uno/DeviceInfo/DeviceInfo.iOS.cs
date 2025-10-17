using System.Runtime.InteropServices;
using AdSupport;
using AppTrackingTransparency;
using UIKit;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string GetManufacturer() =>
        new[]
            {
                EasDeviceInfo.SystemManufacturer,
                "Apple inc",
                string.Empty
            }
        .FirstNotNullOrWhiteSpace();


    private static string GetModel()
    {
        return new[]
        {
            GetSystemLibraryProperty("hw.model"),
            EasDeviceInfo.SystemProductName,
            UIDevice.CurrentDevice.Model,
            string.Empty
        }.FirstNotNullOrWhiteSpace();

    }

    private static string GetDeviceName()
        => UIDevice.CurrentDevice.Name;
    
    private static string GetDeviceId()
        => UIDevice.CurrentDevice.IdentifierForVendor?.AsString() ?? string.Empty;


    private static bool GetIsEmulator()
        => ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR;
    

    private static string? GetSystemLibraryProperty(string property)
    {
        try
        {
            var lengthPtr = Marshal.AllocHGlobal(sizeof(int));
            SysctlByName(property, IntPtr.Zero, lengthPtr, IntPtr.Zero, 0);

            var propertyLength = Marshal.ReadInt32(lengthPtr);

            if (propertyLength == 0)
                return null;

            var valuePtr = Marshal.AllocHGlobal(propertyLength);
            SysctlByName(property, valuePtr, lengthPtr, IntPtr.Zero, 0);

            var returnValue = Marshal.PtrToStringAnsi(valuePtr);

            Marshal.FreeHGlobal(lengthPtr);
            Marshal.FreeHGlobal(valuePtr);

            return returnValue;
        }
        catch (Exception)
        {
            // ignored
        }

        return null;
    }

    public static string QueryDeviceOs()
        => nameof(DeviceOperatingSystem.iOS);

    public static string QueryDeviceOsVersion()
        => FallbackQueryDeviceOsVersion();
    
    [LibraryImport(ObjCRuntime.Constants.SystemLibrary, EntryPoint = "sysctlbyname")]
    // ReSharper disable once UnusedMethodReturnValue.Local
    private static partial int SysctlByName([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newLen);

    /*
    [DllImport(ObjCRuntime.Constants.SystemLibrary, EntryPoint = "sysctlbyname")]
       private static extern int SysctlByName([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newLen);
     
     */
    
    public static async Task<string> GetAdvertiserIdAsync()
    {
        if (!UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
            return ASIdentifierManager.SharedManager.AdvertisingIdentifier.AsString();

        var status = ATTrackingManager.TrackingAuthorizationStatus;

        // If not determined, request permission
        if (status == ATTrackingManagerAuthorizationStatus.NotDetermined)
        {
            var tcs = new TaskCompletionSource<ATTrackingManagerAuthorizationStatus>();
            ATTrackingManager.RequestTrackingAuthorization(authStatus => tcs.SetResult(authStatus));
            status = await tcs.Task;
        }

        // Check if tracking is authorized
        if (status == ATTrackingManagerAuthorizationStatus.Authorized)
            return ASIdentifierManager.SharedManager.AdvertisingIdentifier.AsString();

        Console.WriteLine("Tracking not authorized by the user.");
        return "not authorized"; // IDFA is not accessible

        // Get the IDFA using ASIdentifierManager

    }
    
    
}
