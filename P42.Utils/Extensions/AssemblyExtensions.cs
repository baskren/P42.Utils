using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
namespace P42.Utils;

public static class AssemblyExtensions
{
        
    private static Assembly? _applicationAssembly;
    
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
            if (asm is null)
                continue;
            
            var name = asm.GetName().Name ?? string.Empty;
            if (
                name.Equals("Mono.Android") || 
                name.Equals("Uno.UI") || 
                name.Equals("Microsoft.Maui") || 
                name.Equals("Microsoft.Maui.Controls") || 
                name.Equals("Uno.UI.BindingHelper.Android") || 
                name.Equals("Java.Interop") || 
                name.Equals("Avalonia.Controls") 
            )
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
        => GetAssemblies().FirstOrDefault(asm => asm.GetName().Name == name);
        
    /// <summary>
    /// Get assembly that contains type
    /// </summary>
    /// <param name="type">type</param>
    /// <returns>assembly</returns>
    public static Assembly GetAssembly(this Type type)
        => type.GetTypeInfo().Assembly;

}
