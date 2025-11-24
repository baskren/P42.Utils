namespace P42.Utils;

// ReSharper disable once UnusedType.Global
internal static class AssemblyBuildTimeExtensions
{
    // ReSharper disable once UnusedMember.Local
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
