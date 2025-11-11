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

            var wasmFakeDateItem = LocalData.TagItem.For(nameof(WasmFakeDate), nameof(AssemblyExtensions), typeof(AssemblyExtensions).Assembly);
            if (wasmFakeDateItem.TryRecallValue<DateTime>(out var fakeDate))
                return fakeDate;

            fakeDate = DateTime.Now;
            wasmFakeDateItem.StoreValue(fakeDate);
            return fakeDate;
        }
    }


}
