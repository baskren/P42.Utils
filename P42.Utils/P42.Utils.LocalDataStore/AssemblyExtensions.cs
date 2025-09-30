using System;
using System.Reflection;

namespace P42.Utils;

internal static class AssemblyBuildTimeExtensions
{
    private static DateTime WasmFakeDate 
    {
        get
        {
            if (!OperatingSystem.IsBrowser())
                return DateTime.Now;

            var wasmFakeDateItem = LocalData.TagItem.Get(nameof(WasmFakeDate), nameof(AssemblyExtensions), typeof(AssemblyExtensions).Assembly);
            if (wasmFakeDateItem.TryDeserialize<DateTime>(out var fakeDate))
                return fakeDate;

            fakeDate = DateTime.Now;
            wasmFakeDateItem.Serialize(fakeDate);
            return fakeDate;
        }
    }

    /// <summary>
    /// Gets time at which assembly was built
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="buildTime"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static bool TryGetBuildTime(this Assembly assembly, out DateTime buildTime)
    {
        buildTime = WasmFakeDate;

        // WASM
        if (string.IsNullOrWhiteSpace(assembly.Location))
            return false;


        const int peHeaderOffset = 60;
        const int linkerTimestampOffset = 8;

        try
        {
            var buffer = new byte[2048];
            using var fs = new System.IO.FileStream(assembly.Location, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            fs.ReadExactly(buffer, 0, buffer.Length);

            var headerOffset = BitConverter.ToInt32(buffer, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(buffer, headerOffset + linkerTimestampOffset);
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            buildTime = epoch.AddSeconds(secondsSince1970).ToLocalTime();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


}
