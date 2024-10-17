using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

/// <summary>
/// Embedded Resource extension methods
/// </summary>
public static class EmbeddedResourceExtensions
{
    private static readonly Dictionary<Assembly, string[]> EmbeddedResourceNames = new();
    
    /// <summary>
    /// Test is embedded resource exists
    /// </summary>
    /// <param name="assembly">target assembly</param>
    /// <param name="resourceId">ResourceId</param>
    /// <returns>true on success</returns>
    public static bool EmbeddedResourceExists(this Assembly assembly, string resourceId)
        => FindAssemblyForResourceId(resourceId, assembly) is not null;

    /// <summary>
    /// Test is embedded resource exists
    /// </summary>
    /// <param name="resourceId">ResourceId</param>
    /// <param name="assembly">optional, target assembly</param>
    /// <returns>true on success</returns>
    public static bool EmbeddedResourceExists(string resourceId, Assembly? assembly = null)
        => FindAssemblyForResourceId(resourceId, assembly) is not null;
    
    
    /// <summary>
    /// Copy embedded resource to file at path
    /// </summary>
    /// <param name="assembly">Source assembly</param>
    /// <param name="resourceId"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool TryCopyResource(this Assembly assembly, string resourceId, string path)
    {
        if (FindStreamForResourceId(resourceId, assembly) is not { } stream)
            return false;

        try
        {
            using var destinationFileStream =
                new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            stream.CopyTo(destinationFileStream);
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return false;
        }
        finally
        {
            stream.Dispose();
        }
        return true;

    }
    
    
    /// <summary>
    /// Copy embedded resource to file at path
    /// </summary>
    /// <param name="assembly">Source assembly</param>
    /// <param name="resourceId"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static async Task<bool> TryCopyResourceAsync(this Assembly assembly, string resourceId, string path)
    {
        if (FindStreamForResourceId(resourceId, assembly) is not { } stream)
            return false;

        try
        {
            await using var destinationFileStream =
                new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            
            await stream.CopyToAsync(destinationFileStream);
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return false;
        }
        finally
        {
            await stream.DisposeAsync();
        }
        return true;

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
                    if (AssemblyExtensions.GetAssemblyByName(assemblyName) is { } asmA
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
            assembly = AssemblyExtensions.GetApplicationAssembly();
            Console.WriteLine($"ASSEMBLY: {assembly}");
            Debug.WriteLine($"ASSEMBLY: {resourceId}");
            return FindAssemblyForResourceId(resourceId, assembly) is null 
                ? AssemblyExtensions
                    .GetAssemblies()
                    .FirstOrDefault(asmB => !asmB.IsDynamic && FindAssemblyForResourceId(resourceId, asmB) is not null) 
                : assembly;
        }
        
        if (resourceId[0] == '.')
        {
            if (EmbeddedResourceNames.TryGetValue(assembly, out var names))
                return names.Any(n => n.EndsWith(resourceId)) 
                    ? assembly 
                    : null;
            
            names = assembly.GetManifestResourceNames();
            if (names.Length == 0)
                return null;

            EmbeddedResourceNames[assembly] = names;
            return names.Any(n => n.EndsWith(resourceId)) 
                ? assembly 
                : null;
        }
        else
        {

            if (EmbeddedResourceNames.TryGetValue(assembly, out var names))
                return names.Contains(resourceId) 
                    ? assembly 
                    : null;
            
            names = assembly.GetManifestResourceNames();
            if (names.Length == 0)
                return null;

            EmbeddedResourceNames[assembly] = names;
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

        if (!EmbeddedResourceNames.TryGetValue(assembly, out var names))
        {
            names = assembly.GetManifestResourceNames();
            EmbeddedResourceNames[assembly] = names;
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
