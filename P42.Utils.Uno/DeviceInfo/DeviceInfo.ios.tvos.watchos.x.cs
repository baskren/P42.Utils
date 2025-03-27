#if __IOS__ || __MACCATALYST__
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UIKit;

namespace P42.Utils.Uno;

public static partial class DeviceInfo
{
    private static string _model = string.Empty;
    private static string GetModel()
    {
        if (!string.IsNullOrEmpty(_model))
            return _model;
        try
        {
            var model = GetSystemLibraryProperty("hw.machine");
            if (!string.IsNullOrEmpty(model))
                return _model = model;
        }
        catch (Exception)
        {
            Debug.WriteLine("Unable to query hardware model, returning current device model.");
        }
        return _model = UIDevice.CurrentDevice.Model;
    }

    private static string GetManufacturer() => "Apple";

    private static string GetDeviceName() => UIDevice.CurrentDevice.Name;

    
    private static bool GetIsEmulator()
    #if __MACCATALYST__
        => false;
    #else
        => ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR;
    #endif
    

    private static string? GetSystemLibraryProperty(string property)
    {
        var lengthPtr = Marshal.AllocHGlobal(sizeof(int));
        SysctlByName(property, IntPtr.Zero, lengthPtr, IntPtr.Zero, 0);

        var propertyLength = Marshal.ReadInt32(lengthPtr);

        if (propertyLength == 0)
        {
            Marshal.FreeHGlobal(lengthPtr);
            const string msg = "Unable to read length of iOS SystemLibraryProperty.";
            MainThread.InvokeAsync(() => throw new InvalidOperationException(msg));
            throw new InvalidOperationException(msg);
        }

        var valuePtr = Marshal.AllocHGlobal(propertyLength);
        SysctlByName(property, valuePtr, lengthPtr, IntPtr.Zero, 0);

        var returnValue = Marshal.PtrToStringAnsi(valuePtr);

        Marshal.FreeHGlobal(lengthPtr);
        Marshal.FreeHGlobal(valuePtr);

        return returnValue;
    }

    [DllImport(ObjCRuntime.Constants.SystemLibrary, EntryPoint = "sysctlbyname")]
    private static extern int SysctlByName([MarshalAs(UnmanagedType.LPStr)] string property, IntPtr output, IntPtr oldLen, IntPtr newp, uint newlen);


}
#endif
