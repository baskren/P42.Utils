#if !__ANDROID__ && !__IOS__ && !__MACCATALYST__ && !__MACOS__ && !__WATCHOS__ && !__TVOS__ && !__WASM__ && !WINDOWS && !DESKTOP
using System;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    static string GetManufacturer() => EasDeviceInfo.SystemManufacturer;

    static string GetModel() => EasDeviceInfo.SystemProductName;

    static string GetDeviceName() => EasDeviceInfo.FriendlyName;

    static string GetDeviceId() => GetGeneratedDeviceId();

    static bool GetIsEmulator() => false;
}
#endif
