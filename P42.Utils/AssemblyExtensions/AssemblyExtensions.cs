using System.Diagnostics;
using System.Reflection;
using P42.Serilog.QuickLog;

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


    internal static Func<Assembly, Task<DateTime>>? WasmAssemblyDateTimeDelegate { get; set; }

    
    private static readonly Dictionary<Assembly, DateTime> BuildDateTimes = new();

    /// <param name="assembly"></param>
    extension(Assembly assembly)
    {
        /// <summary>
        /// Safe version of Assembly.GetName().Name
        /// </summary>
        /// <returns></returns>
        public string Name()
            => assembly.GetName().Name ?? string.Empty;

        internal bool IsSystemAssembly()
        {
            var name = assembly.Name();
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
        /// Gets time at which assembly was built
        /// </summary>
        /// <returns></returns>
        public DateTime GetBuildTime()
            => BuildDateTimes.TryGetValue(assembly, out var result)
                ? result 
                : Task.Run(async () => await assembly.GetBuildTimeAsync()).Result;

        /// <summary>
        /// Gets time at which assembly was built
        /// </summary>
        /// <returns>default DateTimeOffset upon failure</returns>
        public async Task<DateTime> GetBuildTimeAsync()
        {
            if (BuildDateTimes.TryGetValue(assembly, out var result))
                return result;

            result = await assembly.InnerGetBuildTimeAsync();
            BuildDateTimes.Add(assembly, result);
            return result;
        }

        private async Task<DateTime> InnerGetBuildTimeAsync()
        {
        
            if (OperatingSystem.IsBrowser())
                return await (WasmAssemblyDateTimeDelegate?.Invoke(assembly) ?? Task.FromResult(default(DateTime)));

            // WASM
            if (string.IsNullOrWhiteSpace(assembly.Location))
                return default;

            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;

            try
            {
                var buffer = new byte[2048];
                await using var fs = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read);
                await fs.ReadExactlyAsync(buffer, 0, buffer.Length);

                var headerOffset = BitConverter.ToInt32(buffer, peHeaderOffset);
                var secondsSince1970 = BitConverter.ToInt32(buffer, headerOffset + linkerTimestampOffset);
                var time = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                time = time.AddSeconds(secondsSince1970);
                return time;
            }
            catch (Exception e)
            {
                QLog.Error(e);
                return default;
            }
        }

        /// <summary>
        /// Doesn't really work!!!
        /// </summary>
        /// <param name="rootNamespace"></param>
        /// <returns></returns>
        public bool TryGetRootNamespace(out string? rootNamespace)
        {
            rootNamespace = assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .SingleOrDefault(a => a.Key == "RootNamespace")
                ?.Value;

            if (!string.IsNullOrEmpty(rootNamespace))
                return true;
        
            // If no custom attribute is found, fall back to the assembly name
            rootNamespace = assembly.GetName().Name;
            return !string.IsNullOrEmpty(rootNamespace);
        }
    }
}
