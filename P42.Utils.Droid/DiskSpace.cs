using System;
using Android.OS;
using P42.Utils.Interfaces;

namespace P42.Utils.Droid
{
    public class DiskSpace : IDiskSpace
    {
        static Java.IO.File[] FileRoots => P42.Utils.Droid.Settings.Context.GetExternalFilesDirs(null);

        public static string PhoneRootPath => FileRoots[0].AbsolutePath; // PhoneMemory
        public static string SDCardPath = FileRoots[1].AbsolutePath; // SCCard (if available)
        public static string UsbPath = FileRoots[2].AbsolutePath; // USB (if available)

        public ulong Free => FreeMemory(PhoneRootPath);

        public ulong Size => TotalMemory(PhoneRootPath);

        public ulong Used => UsedMemory(PhoneRootPath);


        public static ulong TotalMemory(string path)
        {
            using (StatFs statFs = new StatFs(path))
            {
                if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.JellyBeanMr2) //Build.VERSION_CODES.JellyBeanMr2)
                    //noinspection deprecation
                    return (ulong)(statFs.BlockCount * statFs.BlockSize);
                return (ulong)(statFs.BlockCountLong * statFs.BlockSizeLong);
            }
        }

        public static ulong FreeMemory(string path)
        {
            using (StatFs statFs = new StatFs(path))
            {
                if (Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.JellyBeanMr2) //Build.VERSION_CODES.JellyBeanMr2)
                    //noinspection deprecation
                    return (ulong)(statFs.AvailableBlocks * statFs.BlockSize);
                return (ulong)(statFs.AvailableBlocksLong * statFs.BlockSizeLong);
            }
        }

        public static ulong UsedMemory(string path)
        {
            var total = TotalMemory(path);
            var free = FreeMemory(path);
            return total - free;
        }

    }
}
