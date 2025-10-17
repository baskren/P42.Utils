using System.Diagnostics;
using System.Reflection;

namespace P42.Utils;

public static class AssemblyExtensions
{
    private static Assembly? _applicationAssembly;

    //private static readonly LocalData.TagItem WasmFakeDateItem = LocalData.TagItem.Get(nameof(WasmFakeDate), nameof(AssemblyExtensions), Environment.Assembly);



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
        
        return _applicationAssembly ?? throw new NullReferenceException("Could not get application assembly.");
    }

    /// <summary>
    /// List all  assemblies in application's domain
    /// </summary>
    /// <returns>Assembly[]</returns>
    [Obsolete("Use AppDomain.CurrentDomain.GetAssemblies(), instead")]
    public static Assembly[] GetAssemblies()
        => AppDomain.CurrentDomain.GetAssemblies();

    /// <summary>
    /// Find assembly that matches name
    /// </summary>
    /// <param name="name">name</param>
    /// <returns>matching assembly or null</returns>
    public static Assembly? GetAssemblyByName(string name)
        => AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(asm => asm.Name() == name);


}
