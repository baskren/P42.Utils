using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using P42.Serilog.QuickLog;

namespace P42.Utils;

/// <summary>
/// Embedded Resource extension methods
/// </summary>
// ReSharper disable once UnusedType.Global
public static class EmbeddedResourceExtensions
{
    private static readonly Dictionary<Assembly, string[]> EmbeddedResourceNames = new();

    /// <summary>
    /// Does EmbeddedResource Exist?
    /// </summary>
    /// <param name="resourceId">ResourceId</param>
    /// <returns>true on success</returns>
    public static bool EmbeddedResourceExists(string resourceId)
        => FindAssembly(resourceId) is not null;

    /// <summary>
    /// Does EmbeddedResource Exist?
    /// </summary>
    /// <param name="assembly">target assembly</param>
    /// <param name="resourceId">ResourceId</param>
    /// <returns>true on success</returns>
    public static bool Exists(this Assembly assembly, string resourceId)
        => FindAssembly(resourceId, assembly) is not null;

    /// <summary>
    /// Test is embedded resource exists
    /// </summary>
    /// <param name="resourceId">ResourceId</param>
    /// <param name="assembly">optional, target assembly</param>
    /// <returns>true on success</returns>
    [Obsolete("Use Exists instead.", true)]
    public static bool EmbeddedResourceExists(string resourceId, Assembly? assembly = null)
        => FindAssembly(resourceId, assembly) is not null;
    
    
    /// <summary>
    /// Copy embedded resource to file at path
    /// </summary>
    /// <param name="assembly">Source assembly</param>
    /// <param name="resourceId"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool TryCopyResource(this Assembly assembly, string resourceId, string path)
    {
        if (FindStream(resourceId, assembly) is not { } stream)
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
        if (FindStream(resourceId, assembly) is not { } stream)
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

    public class EmbeddedResourceHandle(Stream stream, string resourceId, Assembly assembly)
    {
        public Stream DisposableStream { get; } = stream;
        public string ResourceId { get; } = resourceId;
        public Assembly Assembly { get; } = assembly;
    }

    /// <summary>
    /// Finds assembly and EmbeddedResource stream for resourceId
    /// </summary>
    /// <param name="resourceIdTail">Tail search is resourceIdTail starts with '.' character, otherwise exact match search</param>
    /// <param name="assembly"></param>
    /// <returns>null if not found</returns>
    public static EmbeddedResourceHandle? FindAssemblyResourceIdAndStream(string resourceIdTail, Assembly? assembly = null)
    {
        if (string.IsNullOrWhiteSpace(resourceIdTail))
            return null;

        resourceIdTail = resourceIdTail.Replace(" ", "_").Replace("/", ".").Replace("\\", ".");

        if (assembly == null)
        {
            if (resourceIdTail[0] != '.')
            {
                var index = resourceIdTail.IndexOf(".Resources.", StringComparison.Ordinal);
                if (index > 0)
                {
                    var assemblyName = resourceIdTail[..index];
                    if (AssemblyExtensions.GetAssemblyByName(assemblyName) is { } asmA
                        && FindAssemblyResourceIdAndStream(resourceIdTail, asmA) is {} resultA)
                        return resultA;
                }
            }
            
            assembly = AssemblyExtensions.GetApplicationAssembly();
            if (FindAssemblyResourceIdAndStream(resourceIdTail, assembly) is { } resultB)
                    return resultB;
              
            foreach (var asmB in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asmB.IsSystemAssembly() || asmB.IsDynamic)
                    continue;
                if (FindAssemblyResourceIdAndStream(resourceIdTail, asmB) is {} resultC)
                    return resultC;
            }
            
        }
        
        if (resourceIdTail[0] == '.')
        {
            if (EmbeddedResourceNames.TryGetValue(assembly, out var names))
                return names.FirstOrDefault(n => n.EndsWith(resourceIdTail)) is {} nameA && !string.IsNullOrWhiteSpace(nameA) && assembly.GetManifestResourceStream(nameA) is { } streamA
                    ? new EmbeddedResourceHandle(streamA, nameA, assembly) 
                    : null;
            
            names = assembly.GetManifestResourceNames();
            if (names.Length == 0)
                return null;

            EmbeddedResourceNames[assembly] = names;

            var nameB = names.FirstOrDefault(n => n.EndsWith(resourceIdTail));
            if (!string.IsNullOrWhiteSpace (nameB))
            {
                if (assembly.GetManifestResourceStream(nameB) is { } stream)
                    return new EmbeddedResourceHandle(stream, nameB, assembly);
            }

            return null;

                /*
            return names.FirstOrDefault(n => n.EndsWith(resourceId)) is {} nameB && !string.IsNullOrWhiteSpace(nameB) && assembly.GetManifestResourceStream(nameB) is {} streamB
                ? (streamB, assembly) 
                : null;
                */
        }
        else
        {

            if (EmbeddedResourceNames.TryGetValue(assembly, out var names))
                return names.Contains(resourceIdTail) && assembly.GetManifestResourceStream(resourceIdTail) is { } streamA
                    ? new EmbeddedResourceHandle(streamA, resourceIdTail, assembly) 
                    : null;
            
            names = assembly.GetManifestResourceNames();
            if (names.Length == 0)
                return null;

            EmbeddedResourceNames[assembly] = names;
            return names.Contains(resourceIdTail) && assembly.GetManifestResourceStream(resourceIdTail) is { } streamB
                ? new EmbeddedResourceHandle(streamB, resourceIdTail, assembly) 
                : null;
        }
        
    }

    /// <summary>
    /// Finds the assembly that contains an embedded resource matching the resourceId
    /// </summary>
    /// <param name="resourceIdTail">Tail search is resourceIdTail starts with '.' character, otherwise exact match search</param>
    /// <param name="assembly">optional</param>
    /// <returns></returns>
    public static Assembly? FindAssembly(string resourceIdTail, Assembly? assembly = null)
    {
        var result = FindAssemblyResourceIdAndStream(resourceIdTail, assembly);
        result?.DisposableStream?.Dispose();
        return result?.Assembly;
    }

    /// <summary>
    /// Finds the ResourceId that matches the (possible) tail 
    /// </summary>
    /// <param name="resourceIdTail">Tail search is resourceIdTail starts with '.' character, otherwise exact match search</param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string? FindResourceId(string resourceIdTail, Assembly? assembly = null)
    {
        var result = FindAssemblyResourceIdAndStream(resourceIdTail, assembly);
        result?.DisposableStream?.Dispose();
        return result?.ResourceId;
    }

    /// <summary>
    /// Finds a stream for EmbeddedResourceId
    /// </summary>
    /// <param name="resourceIdTail">Tail search is resourceIdTail starts with '.' character, otherwise exact match search</param>
    /// <param name="assembly">optional</param>
    /// <returns></returns>
    public static Stream? FindStream(string resourceIdTail, Assembly? assembly = null)
        => FindAssemblyResourceIdAndStream(resourceIdTail, assembly)?.DisposableStream;    
    
    /// <summary>
    /// Attempts to get text from matching EmbeddedResourceId
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryGetText(out string value, string resourceId, Assembly? assembly = null)
    {
        value = string.Empty;
        using var stream = FindStream(resourceId, assembly);
        if (stream == null)
            return false;

        try
        {
            using var reader = new StreamReader(stream);
            value = reader.ReadToEnd();
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return false;
        }
    }

    public static bool TryGetBytes(out byte[] value,  string resourceId, Assembly? assembly = null)
    {
        value = [];
        using var stream = FindStream(resourceId, assembly);
        if (stream == null)
            return false;

        try
        {
            value = stream.ReadBytes();
            return true;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
            return false;
        }
    }

    /// <summary>
    /// Attempt to get and deserialize EmbeddedResource as type T
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <param name="value"></param>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryGetSerialized<T>(out T? value, string resourceId, Assembly? assembly = null, T? defaultValue = default)
    {
        value = defaultValue;
        if (!TryGetText(out var json, resourceId, assembly) || string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            value = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return true;
        }
        catch (Exception e)
        {
            QLog.Error(e);
            return false;
        }
    }
    
    /// <summary>
    /// Gets StreamReader (disposable) for EmbeddedResourceId
    /// </summary>
    /// <param name="resourceId"></param>
    /// <param name="assembly"></param>
    /// <returns>null if not found</returns>
    public static StreamReader? FindStreamReader(string resourceId, Assembly? assembly = null)
    {
        if (FindStream(resourceId, assembly) is { } stream)
            return new StreamReader(stream);
        return null;
    }

}
