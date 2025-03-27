using System;
using Windows.System.Profile;

namespace P42.Utils.Uno;

public static class AnalyticsInfoExtensions
{
    /// <summary>
    /// Converts Windows.System.Profile.AnalyticsVersionInfo to System.Version
    /// </summary>
    /// <param name="versionInfo"></param>
    /// <returns></returns>
    public static Version ParseDeviceFamilyVersion(this AnalyticsVersionInfo versionInfo)
    {
        var v = ulong.Parse(versionInfo.DeviceFamilyVersion);
        var v1 = (v & 0xFFFF000000000000L) >> 48;
        if (v1 == ushort.MaxValue)
            return new Version();
        var v2 = (v & 0x0000FFFF00000000L) >> 32;
        if (v2 == ushort.MaxValue)
            return new Version(v1.ToString());
        var v3 = (v & 0x00000000FFFF0000L) >> 16;
        if (v3 == ushort.MaxValue)
            return new Version((int)v1, (int)v2);
        var v4 = v & 0x000000000000FFFFL;
        return v4 == ushort.MaxValue 
            ? new Version((int)v1, (int)v2, (int)v3) 
            : new Version((int)v1, (int)v2, (int)v3, (int)v4);
    }
}
