#if !__ANDROID__ && !__IOS__ && !__MACCATALYST__ && !__MACOS__ && !__WATCHOS__ && !__TVOS__ && !__WASM__ && !__WINUI__ && !DESKTOP
using System;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    static string GetManufacturer() => throw new NotSupportedException();

    static string GetModel() => throw new NotSupportedException();

    static string GetDeviceName() => throw new NotSupportedException();

    static bool GetIsEmulator() => throw new NotSupportedException();
}
#endif
