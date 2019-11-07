using System;
using P42.Utils.Interfaces;

namespace P42.Utils
{
    public static class DiskSpace
    {
        internal static IDiskSpace PlatformDiskSpace;

        public static ulong Free => PlatformDiskSpace?.Free ?? 0;

        public static ulong Size => PlatformDiskSpace?.Size ?? 0;

        public static ulong Used => PlatformDiskSpace?.Used ?? 0;

        private static readonly string[] suffixes = { " B", " KB", " MB", " GB", " TB", " PB" };

        public static string Humanize(double number, int precision = 2, bool si = false)
        {
            // unit's number of bytes
            int unit = si ? 1000 : 1024;

            // suffix counter
            int i = 0;
            // as long as we're bigger than a unit, keep going
            while (number > unit)
            {
                number /= unit;
                i++;
            }
            // apply precision and current suffix
            return Math.Round(number, precision) + suffixes[i] + (si ? "" : "i");
        }
    }
}
