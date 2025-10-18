using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Google.Ads.Identifier;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string GetManufacturer()
    {
        var make = Build.Brand;
        if (!string.IsNullOrWhiteSpace(make))
            return make;
        
        make = Build.Manufacturer;
        return string.IsNullOrWhiteSpace(make) ? string.Empty : make;
    }

    private static string GetModel()
        => string.IsNullOrWhiteSpace(Build.Model) ? string.Empty : Build.Model;

    private static string GetDeviceName()
    {
        // DEVICE_NAME added in System.Global in API level 25
        // https://developer.android.com/reference/android/provider/Settings.Global#DEVICE_NAME
        return Android.Provider.Settings.Global
            .GetString(Android.App.Application.Context.ContentResolver, "device_name") ?? string.Empty;

    }


    private static string GetDeviceId()
    {
        var id = Android.Provider.Settings.Secure
            .GetString(Android.App.Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
        if (IsValidId(id))
            return id!;
        
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)   
#pragma warning disable CA1416
            id = Build.GetSerial();
#pragma warning restore CA1416
        if (IsValidId(id))
            return id!;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)   
#pragma warning disable CA1422
            id = Build.Serial;
#pragma warning restore CA1422
        return IsValidId(id) ? id! : string.Empty;
    } 

    private static bool _isEmulatorSet;
    private static bool _isEmulator;
    private static bool GetIsEmulator()
    {
        if (_isEmulatorSet)
            return _isEmulator;
        
        if (!_isEmulator && !string.IsNullOrWhiteSpace(Build.Brand) && !string.IsNullOrWhiteSpace(Build.Device))
        {
            _isEmulator = Build.Brand.StartsWith("generic", StringComparison.InvariantCulture) &&
                          Build.Device.StartsWith("generic", StringComparison.InvariantCulture);
        }
        
        if (!_isEmulator && !string.IsNullOrWhiteSpace(Build.Fingerprint))
        {
            _isEmulator = Build.Fingerprint.StartsWith("generic", StringComparison.InvariantCulture) ||
                          Build.Fingerprint.StartsWith("unknown", StringComparison.InvariantCulture);
        }
        
        if (!_isEmulator && !string.IsNullOrWhiteSpace(Build.Hardware))
        {
            _isEmulator = Build.Hardware.Contains("goldfish", StringComparison.OrdinalIgnoreCase) ||
                          Build.Hardware.Contains("ranchu", StringComparison.OrdinalIgnoreCase);
        }

        if (!_isEmulator && !string.IsNullOrWhiteSpace(Build.Model))
        {
            _isEmulator = Build.Model.Contains("emulator", StringComparison.OrdinalIgnoreCase) ||
                          Build.Model.Contains("google_sdk", StringComparison.OrdinalIgnoreCase) ||
                          Build.Model.Contains("Emulator", StringComparison.OrdinalIgnoreCase) ||
                          Build.Model.Contains("Android SDK", StringComparison.OrdinalIgnoreCase);
        }

        if (!_isEmulator && !string.IsNullOrWhiteSpace(Build.Manufacturer))
        {
            _isEmulator = Build.Manufacturer.Contains("emulator", StringComparison.OrdinalIgnoreCase) ||
                          Build.Manufacturer.Contains("Genymotion", StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(Build.Product))
        {
            _isEmulator = Build.Product.Contains("emulator", StringComparison.OrdinalIgnoreCase) ||
                          Build.Product.Contains("google_sdk", StringComparison.OrdinalIgnoreCase) ||
                          Build.Product.Contains("sdk", StringComparison.OrdinalIgnoreCase) ||
                          Build.Product.Contains("sdk_google", StringComparison.OrdinalIgnoreCase) ||
                          Build.Product.Contains("sdk_x86", StringComparison.OrdinalIgnoreCase) ||
                          Build.Product.Contains("simulator", StringComparison.OrdinalIgnoreCase) ||
                          Build.Product.Contains("vbox86p", StringComparison.OrdinalIgnoreCase);
        }

        _isEmulatorSet = true;
        return _isEmulator;
    }

    public static string QueryDeviceOs()
        => nameof(DeviceOperatingSystem.Android);
    
    public static string QueryDeviceOsVersion()
        => Windows.System.Profile.AnalyticsInfo.VersionInfo.ParseDeviceFamilyVersion().ToString();

    public static async Task<string> GetAdvertiserIdAsync()
    {
        var emptyGuid = Guid.Empty.ToString();

        if (Android.App.Application.Context.ApplicationContext is not {} context)
        {
            Console.WriteLine("Could not get ApplicationContext.  Are you calling GetAdvertiserIdAsync() too soon?");
            return emptyGuid;
        }
        
        if (context.PackageManager is not { } packageManager)
        {
            Console.WriteLine("Could not get PackageManager.  Are you calling GetAdvertiserIdAsync() too soon?");
            return emptyGuid;
        }
        
        if (context.PackageName is not { } packageName || string.IsNullOrWhiteSpace(packageName))
        {
             Console.WriteLine("Could not get PackageName.  Are you calling GetAdvertiserIdAsync() too soon?");
             return emptyGuid;
        }
        
        if (packageManager.CheckPermission("com.google.android.gms.permission.AD_ID", packageName) != Permission.Granted)
        {
            Console.WriteLine("GooglePlay AD_ID permission has not been set in AndroidManifest.xml");
            return emptyGuid;
        }
    
        // Must be run off the main thread. We use Task.Run for this.
        return await Task.Run(() =>
        {
            try
            {
                // Retrieve the AdvertisingIdClient.Info object
                AdvertisingIdClient.Info adInfo =
                    AdvertisingIdClient.GetAdvertisingIdInfo(context);

                // Check for the Limit Ad Tracking setting
                var isLimitAdTrackingEnabled = adInfo.IsLimitAdTrackingEnabled;

                if (!isLimitAdTrackingEnabled)
                    return adInfo.Id ?? emptyGuid;

            }
            catch (Java.IO.IOException e)
            {
                // This typically means connection to Google Play Services failed (no internet, etc.)
                Console.WriteLine($"IOException while getting Ad ID: {e.Message}");
            }
            catch (GooglePlayServicesNotAvailableException e)
            {
                // Google Play Services is not available on this device
                Console.WriteLine($"Play Services Not Available: {e.Message}");
            }
            catch (Exception e)
            {
                // General exception
                Console.WriteLine($"Error getting Ad ID: {e.Message}");
            }
            
            return emptyGuid;
        });
    }   
}
