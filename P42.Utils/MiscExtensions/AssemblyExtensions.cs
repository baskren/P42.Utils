using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class AssemblyExtensions
{
    private static Assembly? _applicationAssembly;

    /// <summary>
    /// Safe version of Assembly.GetName().Name
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string Name(this Assembly assembly)
        => assembly.GetName().Name ?? string.Empty;

    internal static bool IsSystemAssembly(this Assembly asm)
    {
        var name = asm.Name();
        return 
            name.Equals("Java.Interop") || 
            //name.Equals("Mono.Android") ||
            name.StartsWith("Mono.") ||
            //name.Equals("Uno.UI.BindingHelper.Android") || 
            //name.Equals("Uno.UI") ||
            name.StartsWith("Uno.") ||
            name.StartsWith("System.") ||
            //name.Equals("Microsoft.Maui") || 
            //name.Equals("Microsoft.Maui.Controls") ||
            name.StartsWith("Microsoft.") ||
            //name.Equals("Avalonia.Android") ||
            //name.Equals("Avalonia.Controls") ||
            //name.Equals("Avalonia.Base");
            name.StartsWith("Avalonia.");
        
    }
    
    /// <summary>
    /// What is the root  app assembly?
    /// </summary>
    /// <returns>root app assembly</returns>
    /// <exception cref="NullReferenceException"></exception>
    public static Assembly GetApplicationAssembly()
    {
        if (_applicationAssembly is not null)
            return _applicationAssembly;
            
        // the following works in:
        // - iOS
        // - WASM
        // - WinAppSdk (packaged)
        // doesn't work in:
        // - Android
        // - WinAppSdk (un-packaged)
        _applicationAssembly = Assembly.GetEntryAssembly();       
        if (_applicationAssembly is not null)
            return _applicationAssembly;
            
        // Android and WinAppSdk (un-packaged)
        var stackTrace = new StackTrace();
        //System.Diagnostics.Debug.WriteLine("");
        var frames = stackTrace.GetFrames();
        var assemblies =  frames.Select(f => f.GetMethod()?.DeclaringType?.Assembly).Distinct();
        //var index = 0;
        //Assembly? lastCandidateAsm = null;
        foreach (var asm in assemblies)
        {
            if (asm is null || asm.IsSystemAssembly())
                continue;
            _applicationAssembly = asm; 
            //System.Diagnostics.Debug.WriteLine($"{index++}: {asm.FullName} : {asm.Location} {asm.IsDynamic} {asm.EntryPoint} {asm.IsFullyTrusted}");
        }
        
        if (_applicationAssembly is not null)
            return _applicationAssembly;
        
        throw new NullReferenceException("Could not get application assembly.");
    }

    /// <summary>
    /// List all  assemblies in application's domain
    /// </summary>
    /// <returns>Assembly[]</returns>
    public static Assembly[] GetAssemblies()
        => AppDomain.CurrentDomain.GetAssemblies();

    /// <summary>
    /// Find assembly that matches name
    /// </summary>
    /// <param name="name">name</param>
    /// <returns>matching assembly or null</returns>
    public static Assembly? GetAssemblyByName(string name)
        => GetAssemblies().FirstOrDefault(asm => asm.Name() == name);
        
    /// <summary>
    /// Get assembly that contains type
    /// </summary>
    /// <param name="type">type</param>
    /// <returns>assembly</returns>
    public static Assembly GetAssembly(this Type type)
        => type.GetTypeInfo().Assembly;

    /// <summary>
    /// Gets time at which assembly was built
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static DateTime GetBuildTime(this Assembly assembly)
    {
        if (assembly.Location is not string filePath)
            throw new Exception($"Unable to get path to location of assembly [{assembly.FullName}].");
        
        const int peHeaderOffset = 60;
        const int linkerTimestampOffset = 8;

        var buffer = new byte[2048];
        using var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
        fs.ReadExactly(buffer, 0, buffer.Length);

        var headerOffset = BitConverter.ToInt32(buffer, peHeaderOffset);
        var secondsSince1970 = BitConverter.ToInt32(buffer, headerOffset + linkerTimestampOffset);
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return epoch.AddSeconds(secondsSince1970).ToLocalTime();
    }

}
