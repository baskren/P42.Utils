namespace P42.Utils
{
    public static class Process
    {
        internal static IProcess? PlatformProcess { get; set; }

        public static ulong Memory => PlatformProcess?.Memory() ?? 0;

    }
}
