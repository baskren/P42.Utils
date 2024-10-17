/*

using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;


// Why is GetCallingAssembly commented out?  Because it doesn't work with UWP when .NET Native compile chain is enabled
// This might be addressed by bringing Forms9Patch.AssemblyExtensions into PCL.Extensions


namespace P42.Utils;

public static class ApplicationStorageExtensions
{
    private const string ApplicationStorageFolderName = "ApplicationStorage";
    private static readonly string RootPath;

    static ApplicationStorageExtensions()
    {
        DirectoryExtensions.AssureExists(Environment.ApplicationDataPath);
        RootPath = Path.Combine(Environment.ApplicationDataPath, ApplicationStorageFolderName);
        DirectoryExtensions.AssureExists(RootPath);
    }
    
    private static string FolderPath(string? folderName = null)
    {

        if (string.IsNullOrWhiteSpace(folderName))
            return RootPath;

        var folderPath = Path.Combine(RootPath, folderName);
        DirectoryExtensions.AssureExists(folderPath);
        return folderPath;
    }
 
    public static string LocalPath(string uid, string fileName)
        => Path.Combine(FolderPath(string.IsNullOrWhiteSpace(uid)?null :$"uid-{uid}"), fileName);


    
    public static string EmbeddedStoredText(string resourceId, Assembly assembly = null)
    {
        string contents = null;
        using (Stream stream = EmbeddedResource.GetStream(resourceId, assembly))
        {
            if (stream != null)
                using (var reader = new StreamReader(stream))
                {
                    contents = reader.ReadToEnd();
                }
        }
        return contents;
    }

    public static string JsonFromStorage(string uid, string fileName)
    {
        var path = LocalPath(uid, fileName);
        if (File.Exists(path))
            return File.ReadAllText(path);
        return null;
    }

    public static string LoadLocalStorageText(string uid, string fileName)
        => JsonFromStorage(uid, fileName);
        

    public static string LoadText(string uid, string fileName, Assembly assembly = null)
    {
        var result = LoadLocalStorageText(uid, fileName);
        if (result == null)
            result = EmbeddedStoredText(fileName, assembly);
        return result;
    }

    public static void StoreText(string uid, string text, string fileName)
    {
        var path = LocalPath(uid, fileName);
        System.Diagnostics.Debug.WriteLine("StoreText: " + path);
        File.WriteAllText(path, text);
    }

    public static TType LoadSerializedResource<TType>(string uid, string resourceName, Assembly assembly = null, TType defaultValue = default)
    {
        var text = LoadText(uid, resourceName, assembly);
        if (!string.IsNullOrEmpty(text))
        {
            var result = JsonConvert.DeserializeObject<TType>(text, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return result;
        }
        return defaultValue;
    }

    public static TType LoadSerializedEmbeddedResource<TType>(string resourceName, Assembly assembly = null, TType defaultValue = default)
    {
        var text = EmbeddedStoredText(resourceName, assembly);
        if (!string.IsNullOrEmpty(text))
        {
            var result = JsonConvert.DeserializeObject<TType>(text, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return result;
        }
        return defaultValue;
    }


    public static void StoreSerializedResource<TType>(string uid, TType obj, string resourceName)
    {
        if (resourceName != null)
        {
            var jsonSerializationSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize
            };
            var textToStore = JsonConvert.SerializeObject(obj, jsonSerializationSettings);
            if (string.IsNullOrEmpty(textToStore))
                throw new InvalidDataContractException("Could not serialize object [" + obj + "]");
            StoreText(uid, textToStore, resourceName);
        }
    }

    public static bool ResourceAvailable(string uid, string resourceName, Assembly assembly = null, StreamSource source = StreamSource.Default)
    {
        var result = false;
        if (!result && (source == StreamSource.Default || source == StreamSource.Local))
            result = LocalResourceAvailable(uid, resourceName);
        if (!result && (source == StreamSource.Default || source == StreamSource.EmbeddedResource))
            result = EmbeddedResourceAvailable(resourceName, assembly);
        return result;
    }

    public static StreamReader ResourceStreamReader(string uid, string resourceName, Assembly assembly = null, StreamSource source = StreamSource.Default)
    {
        StreamReader reader = null;
        if (source == StreamSource.Default || source == StreamSource.Local)
            reader = LocalStreamReader(uid, resourceName);
        if (reader == null && (source == StreamSource.Default || source == StreamSource.EmbeddedResource))
            reader = EmbeddedResourceStreamReader(resourceName, assembly);
        return reader;
    }

    public static bool EmbeddedResourceAvailable(string resourceId, Assembly assembly = null)
        => EmbeddedResource.Available(resourceId, assembly);
        

    public static StreamReader EmbeddedResourceStreamReader(string resourceId, Assembly assembly = null)
    {
        if (EmbeddedResource.GetStream(resourceId, assembly) is Stream stream)
            return new StreamReader(stream);
        return null;
    }

    public static bool LocalResourceAvailable(string uid, string fileName)
        => File.Exists(LocalPath(uid, fileName));

    public static StreamReader? LocalStreamReader(string uid, string fileName)
    {
        var path = LocalPath(uid, fileName);
        return File.Exists(path) 
            ? new StreamReader(new FileStream(path, FileMode.Open)) 
            : null;
    }


    public static StreamWriter ResourceStreamWriter(string uid, string resourceName)
        => LocalStreamWriter(uid, resourceName);

    public static StreamWriter LocalStreamWriter(string uid, string fileName) 
        => new(new FileStream(LocalPath(uid, fileName), FileMode.Create));

    public static string[] ListResources(StreamSource source, Assembly? assembly = null)
    {
        if (source == StreamSource.Default)
            throw new InvalidDataException("StreamSource.Default is too ambiguous");
        var result = new List<string>();
        if (source == StreamSource.Local)
        {
            var fileNames = Directory.GetFiles(FolderPath);// Storage.GetFileNames();
            if (fileNames is { Length: > 0 }) 
                result.AddRange(fileNames);
        }
        else if (source == StreamSource.EmbeddedResource)
        {
            assembly ??= AssemblyExtensions.CallerApplicationAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            result.AddRange(resourceNames);
        }
        return result;
    }
}
*/
