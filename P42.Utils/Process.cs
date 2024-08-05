namespace P42.Utils
{
    public static class Process
    {
        internal static IProcess PlatformProcess;

        public static ulong Memory => PlatformProcess?.Memory() ?? 0;

    }
}
