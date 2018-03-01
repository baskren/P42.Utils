using System.Reflection;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;


// Why is GetCallingAssembly commented out?  Because it doesn't work with UWP when .NET Native compile chain is enabled
// This might be addressed by bringing Forms9Patch.AssemblyExtensions into PCL.Extensions




namespace P42.Utils
{
    public static class ApplicationStorageExtensions
    {
        static string ApplicationStorageFolderName = "ApplicationStorage";

        static string FolderPath
        {
            get
            {
                var path = Path.Combine(P42.Utils.Environment.ApplicationDataPath, ApplicationStorageFolderName);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }


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
            var path = Path.Combine(FolderPath, fileName);
            if (uid != null)
            {
                if (!Directory.Exists("uid-" + uid))
                    return null;
                path = Path.Combine("uid-" + uid, fileName);
            }
            if (!File.Exists(path))
                return null;
            var contents = File.ReadAllText(path);
            return contents;
        }

        public static string LoadLocalStorageText(string uid, string fileName)
        {
            return JsonFromStorage(uid, fileName);
        }

        public static string LoadText(string uid, string fileName, Assembly assembly = null)
        {
            var result = LoadLocalStorageText(uid, fileName);
            if (result == null)
                result = EmbeddedStoredText(fileName, assembly);
            return result;
        }

        public static void StoreText(string uid, string text, string fileName)
        {
            var path = Path.Combine(FolderPath, fileName);
            if (uid != null)
            {
                if (!Directory.Exists("uid-" + uid))
                    Directory.CreateDirectory("uid-" + uid);
                path = Path.Combine("uid-" + uid, fileName);
            }
            File.WriteAllText(path, text);
        }

        public static TType LoadSerializedResource<TType>(string uid, string resourceName, Assembly assembly = null, TType defaultValue = default(TType))
        {
            var text = LoadText(uid, resourceName, assembly);
            if (!string.IsNullOrEmpty(text))
            {
                TType result = JsonConvert.DeserializeObject<TType>(text, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });
                return result;
            }
            return defaultValue;
        }

        public static TType LoadSerializedEmbeddedResource<TType>(string resourceName, Assembly assembly = null, TType defaultValue = default(TType))
        {
            var text = EmbeddedStoredText(resourceName, assembly);
            if (!string.IsNullOrEmpty(text))
            {
                TType result = JsonConvert.DeserializeObject<TType>(text, new JsonSerializerSettings
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
                var jsonSerializationSetings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented,
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                };
                string textToStore = JsonConvert.SerializeObject(obj, jsonSerializationSetings);
                if (string.IsNullOrEmpty(textToStore))
                    throw new InvalidDataContractException("Could not serialize object [" + obj + "]");
                StoreText(uid, textToStore, resourceName);
            }
        }

        public static bool ResourceAvailable(string uid, string resourceName, Assembly assembly = null, StreamSource source = StreamSource.Default)
        {
            bool result = false;
            if (!result && (source == StreamSource.Default || source == StreamSource.Local))
                result = LocalResourceAvailable(uid, resourceName);
            if (!result && (source == StreamSource.Default || source == StreamSource.EmbeddedResource))
                result = EmbeddedResourceAvailable(resourceName, assembly);
            return result;
        }

        public static StreamReader ResourceStreamReader(string uid, string resourceName, Assembly assembly = null, StreamSource source = StreamSource.Default)
        {
            StreamReader reader = null;
            if (reader == null && (source == StreamSource.Default || source == StreamSource.Local))
                reader = LocalStreamReader(uid, resourceName);
            if (reader == null && (source == StreamSource.Default || source == StreamSource.EmbeddedResource))
                reader = EmbeddedResourceStreamReader(resourceName, assembly);
            return reader;
        }

        public static bool EmbeddedResourceAvailable(string resourceId, Assembly assembly = null)
        {
            return EmbeddedResource.Available(resourceId, assembly);
        }

        public static StreamReader EmbeddedResourceStreamReader(string resourceId, Assembly assembly = null)
        {
            var stream = EmbeddedResource.GetStream(resourceId, assembly);
            if (stream == null)
                return null;
            return new StreamReader(stream);
        }

        public static bool LocalResourceAvailable(string uid, string fileName)
        {
            var path = Path.Combine(FolderPath, fileName);
            if (uid != null)
                path = Path.Combine("uid-" + uid, fileName);
            return File.Exists(path);
        }


        public static StreamReader LocalStreamReader(string uid, string fileName)
        {
            var path = Path.Combine(FolderPath, fileName);
            if (uid != null)
            {
                if (!Directory.Exists("uid-" + uid))
                    return null;
                path = Path.Combine("uid-" + uid, fileName);
            }
            if (!File.Exists(path))
                return null;
            return new StreamReader(new FileStream(path, FileMode.Open));
        }

        public static StreamWriter ResourceStreamWriter(string uid, string resourceName)
        {
            var streamWriter = LocalStreamWriter(uid, resourceName);
            return streamWriter;
        }

        public static StreamWriter LocalStreamWriter(string uid, string fileName)
        {
            var path = Path.Combine(FolderPath, fileName);
            if (uid != null)
            {
                if (!Directory.Exists("uid-" + uid))
                    Directory.CreateDirectory("uid-" + uid);
                path = Path.Combine("uid-" + uid, fileName);
            }
            return new StreamWriter(new FileStream(path, FileMode.Create));
        }

        public static List<string> ListResources(StreamSource source, Assembly assembly = null)
        {
            if (source == StreamSource.Default)
                throw new InvalidDataException("StreamSource.Default is too ambiguous");
            var result = new List<string>();
            if (source == StreamSource.Local)
            {
                var fileNames = Directory.GetFiles(FolderPath);// Storage.GetFileNames();
                if (fileNames != null && fileNames.Length > 0)
                    foreach (var fileName in fileNames)
                        result.Add(fileName);
            }
            else if (source == StreamSource.EmbeddedResource)
            {
                var resourceNames = assembly.GetManifestResourceNames();
                foreach (string resourceName in resourceNames)
                    result.Add(resourceName);
            }
            return result;
        }
    }
}

