﻿#if __ANDROID__
using System;
using Android.OS;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string GetModel() => Build.Model ?? string.Empty;

    private static string GetManufacturer() => Build.Manufacturer ?? string.Empty;

    
    private static string? _deviceName;
    private static string GetDeviceName()
    {
        if (!string.IsNullOrWhiteSpace(_deviceName))
            return _deviceName;
        // DEVICE_NAME added in System.Global in API level 25
        // https://developer.android.com/reference/android/provider/Settings.Global#DEVICE_NAME
        _deviceName = Android.Provider.Settings.Global.GetString(Android.App.Application.Context.ContentResolver, "device_name");
        if (string.IsNullOrWhiteSpace(_deviceName))
            _deviceName = Model;
        return _deviceName;
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

}
#endif
