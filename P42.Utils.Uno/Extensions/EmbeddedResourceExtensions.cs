using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
//using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

#nullable enable

/// <summary>
/// Embedded Resource extension methods
/// </summary>
public static class EmbeddedResourceExtensions
{
    private static readonly Dictionary<Assembly, string[]> s_embeddedResourceNames = new();

    private static Assembly? s_entryAssembly;

    private static Assembly EntryAssembly
    {
        get
        {
            if (s_entryAssembly is not null)
                return s_entryAssembly;
            
            // the following works in:
            // - iOS
            // - WASM
            // - WinAppSdk (packaged)
            // doesn't work in:
            // - Android
            // - WinAppSdk (unpackaged)
            s_entryAssembly = Assembly.GetEntryAssembly();       
            if (s_entryAssembly is not null)
                return s_entryAssembly;
            
            var stackTrace = new StackTrace();
            System.Diagnostics.Debug.WriteLine("");
            var asms =  stackTrace.GetFrames().Select(f => f.GetMethod().DeclaringType.Assembly).Distinct();
            int index = 0;
            Assembly? lastCandidateAsm = null;
            foreach (var asm in asms)
            {
                var name = asm.GetName().Name;
                if (name.Equals("Mono.Android") || name.Equals("Uno.UI") || name.Equals("Microsoft.Maui") || name.Equals("Microsoft.Maui.Controls") )
                    continue;
                s_entryAssembly = asm; 
                System.Diagnostics.Debug.WriteLine($"{index++}: {asm.FullName} : {asm.Location} {asm.IsDynamic} {asm.EntryPoint} {asm.IsFullyTrusted}");
            }

            return s_entryAssembly;
        }
    }
    
    /// <summary>
    /// Finds the assembly that contains an embedded resource matching the resourceId
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static Assembly? FindAssemblyForResourceId(string resourceId, Assembly? assembly = null)
    {
        if (string.IsNullOrWhiteSpace(resourceId))
            return null;

        if (assembly == null)
        {
            if (resourceId[0] != '.')
            {
                var index = resourceId.IndexOf(".Resources.", StringComparison.Ordinal);
                if (index > 0)
                {
                    var assemblyName = resourceId[..index];
                    if (ReflectionExtensions.GetAssemblyByName(assemblyName) is { } asmA
                        && FindAssemblyForResourceId(resourceId, asmA) is not null)
                        return asmA;
                }
            }
            
            // assembly = Microsoft.UI.Xaml.Application.Current.GetType().Assembly;
            // the following works in:
            // - iOS
            // - WASM
            // - WinAppSdk
            // doesn't work in:
            // - Android
            assembly = EntryAssembly;
            Console.WriteLine($"ASSEMBLY: {assembly}");
            Debug.WriteLine($"ASSEMBLY: {resourceId}");
            return FindAssemblyForResourceId(resourceId, assembly) is null 
                ? ReflectionExtensions
                    .GetAssemblies()
                    .FirstOrDefault(asmB => !asmB.IsDynamic && FindAssemblyForResourceId(resourceId, asmB) is not null) 
                : assembly;
        }
        
        if (resourceId[0] == '.')
        {
            if (s_embeddedResourceNames.TryGetValue(assembly, out var names))
                return names.Any(n => n.EndsWith(resourceId)) 
                    ? assembly 
                    : null;
            
            names = assembly.GetManifestResourceNames();
            if (names.Length == 0)
                return null;

            s_embeddedResourceNames[assembly] = names;
            return names.Any(n => n.EndsWith(resourceId)) 
                ? assembly 
                : null;
        }
        else
        {

            if (s_embeddedResourceNames.TryGetValue(assembly, out var names))
                return names.Contains(resourceId) 
                    ? assembly 
                    : null;
            
            names = assembly.GetManifestResourceNames();
            if (names.Length == 0)
                return null;

            s_embeddedResourceNames[assembly] = names;
            return names.Contains(resourceId) 
                ? assembly 
                : null;
        }
        
    }

    public static Stream? FindStreamForResourceId(string resourceId, Assembly? assembly = null)
    {
        assembly ??= FindAssemblyForResourceId(resourceId);

        if (assembly == null)
            return null;

        if (!s_embeddedResourceNames.TryGetValue(assembly, out var names))
        {
            names = assembly.GetManifestResourceNames();
            s_embeddedResourceNames[assembly] = names;
        }

        if (names.Length == 0)
            return null;

        var name = resourceId[0] == '.' 
            ? names.FirstOrDefault(n => n.EndsWith(resourceId)) 
            : names.FirstOrDefault(n => n == resourceId);

        return string.IsNullOrEmpty(name) 
            ? null 
            : assembly.GetManifestResourceStream(name);
    }
}
