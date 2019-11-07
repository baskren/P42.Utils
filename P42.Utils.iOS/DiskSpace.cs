using System;
using System.Runtime.InteropServices;
using Foundation;
using ObjCRuntime;
using P42.Utils.Interfaces;

namespace P42.Utils.iOS
{
    public class DiskSpace : IDiskSpace
    {
        [DllImport(Constants.FoundationLibrary)]
        public static extern IntPtr NSHomeDirectory();

        public static string NSHomeDir
        {
            get
            {
                return ((NSString)Runtime.GetNSObject(NSHomeDirectory())).ToString();
            }
        }
        public ulong Free
        {
            get
            {
                var attributes = NSFileManager.DefaultManager.GetFileSystemAttributes(NSHomeDir);
                var freeSpace = attributes.FreeSize; //attributes[NSFileManager.SystemFreeSize] as Int64;
                return freeSpace;
            }
        }

        public ulong Size
        {
            get
            {
                var attributes = NSFileManager.DefaultManager.GetFileSystemAttributes(NSHomeDir);
                var size = attributes.Size; //attributes[NSFileManager.SystemFreeSize] as Int64;
                return size;
            }
        }

        public ulong Used => Size - Free;
    }
}
