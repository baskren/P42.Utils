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


}
