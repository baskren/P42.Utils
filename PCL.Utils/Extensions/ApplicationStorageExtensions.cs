using System.Reflection;
using System.IO;
using PCLStorage;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace PCL.Utils
{
    public static class ApplicationStorageExtensions
    {
        public static string EmbeddedStoredText(string resourceName, Assembly assembly = null)
        {
            string contents = null;
            assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                    using (var reader = new StreamReader(stream))
                    {
                        contents = reader.ReadToEnd();
                    }
            }
            return contents;
        }

        public static string JsonFromStoredFolder(string uid, string fileName, IFolder folder)
        {
            string contents = null;
            if (uid != null)
            {
                if (folder.CheckExists(uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder(uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder(uid);
            }
            if (folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
            {
                var file = folder.GetFile(fileName);
                contents = file?.ReadAllText();
            }
            return contents;
        }

        public static string LoadLocalStorageText(string uid, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            //System.Diagnostics.Debug.WriteLine("LocalFolder=" + folder?.Path);
            return folder == null ? null : JsonFromStoredFolder(uid, fileName, folder);
        }

        public static string LoadRoamingStorageText(string uid, string fileName)
        {
            var folder = FileSystem.Current.RoamingStorage;
            //System.Diagnostics.Debug.WriteLine("RoamingFolder="+folder?.Path);
            return folder == null ? null : JsonFromStoredFolder(uid, fileName, folder);
        }

        public static string LoadText(string uid, string fileName, Assembly assembly = null)
        {
            var result = LoadRoamingStorageText(uid, fileName);
            result = result ?? LoadLocalStorageText(uid, fileName);
            if (result == null)
            {
                assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
                result = EmbeddedStoredText(fileName, assembly);
            }
            return result;
        }

        public static void StoreText(string uid, string text, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            if (folder == null)
                throw new InvalidDataContractException("Hey, there should be a LocalStorage folder!");
            if (uid != null)
            {
                if (folder.CheckExists(uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder(uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder(uid);
            }
            IFile file = folder.CreateFile(fileName, CreationCollisionOption.ReplaceExisting);
            //file.WriteAllText(text);
            file.WriteAllTextAsync(text);
        }

        public static TType LoadSerializedResource<TType>(string uid, string resourceName, Assembly assembly = null, TType defaultValue = default(TType))
        {
            assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
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
            assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
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
                    //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    //ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize
                };
                string textToStore = JsonConvert.SerializeObject(obj, jsonSerializationSetings);
                if (string.IsNullOrEmpty(textToStore))
                    throw new InvalidDataContractException("Could not serialize object [" + obj + "]");
                StoreText(uid, textToStore, resourceName);
            }
        }

        public static StreamReader ResourceStreamReader(string uid, string resourceName, Assembly assembly = null, StreamSource source = StreamSource.Default)
        {
            StreamReader reader = null;
            if (source == StreamSource.Default || source == StreamSource.Roaming)
                reader = RoamingStreamReader(uid, resourceName);
            if (reader == null && (source == StreamSource.Default || source == StreamSource.Local))
                reader = LocalStreamReader(uid, resourceName);
            if (reader == null && (source == StreamSource.Default || source == StreamSource.EmbeddedResource))
            {
                assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
                reader = EmbeddedResourceStreamReader(resourceName, assembly);
            }
            return reader;
        }

        public static StreamReader EmbeddedResourceStreamReader(string resourceName, Assembly assembly = null)
        {
            assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
            var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream != null)
            {
                var streamReader = new StreamReader(stream);
                return streamReader;
            }
            return null;
        }

        public static StreamReader RoamingStreamReader(string uid, string fileName)
        {
            var folder = FileSystem.Current.RoamingStorage;
            return folder == null ? null : StreamReaderFromStoredFolder(uid, fileName, folder);
        }

        public static StreamReader LocalStreamReader(string uid, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            return folder == null ? null : StreamReaderFromStoredFolder(uid, fileName, folder);
        }

        public static StreamReader StreamReaderFromStoredFolder(string uid, string fileName, IFolder folder)
        {
            StreamReader streamReader = null;
            if (uid != null)
            {
                if (folder.CheckExists(uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder(uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder(uid);
            }
            //System.Diagnostics.Debug.WriteLine("Reading from ["+folder.Path+"]["+fileName+"]");
            if (folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
            {
                var file = folder.GetFile(fileName);
                var stream = file.Open(FileAccess.Read);
                streamReader = new StreamReader(stream);
            }
            return streamReader;
        }

        public static StreamWriter ResourceStreamWriter(string uid, string resourceName)
        {
            var streamWriter = RoamingStreamWriter(uid, resourceName);
            if (streamWriter == null)
                streamWriter = LocalStreamWriter(uid, resourceName);
            return streamWriter;
        }

        public static StreamWriter RoamingStreamWriter(string uid, string fileName)
        {
            var folder = FileSystem.Current.RoamingStorage;
            return folder == null ? null : StreamWriterFromStoredFolder(uid, fileName, folder);
        }

        public static StreamWriter LocalStreamWriter(string uid, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            return folder == null ? null : StreamWriterFromStoredFolder(uid, fileName, folder);
        }

        public static StreamWriter StreamWriterFromStoredFolder(string uid, string fileName, IFolder folder)
        {
            if (uid != null)
            {
                if (folder.CheckExists(uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder(uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder(uid);
            }
            System.Diagnostics.Debug.WriteLine("Writing to [" + folder.Path + "][" + fileName + "]");
            var file = folder.CreateFile(fileName, CreationCollisionOption.ReplaceExisting);
            var stream = file.Open(FileAccess.ReadAndWrite);
            var streamWriter = new StreamWriter(stream);
            return streamWriter;
        }

        public static List<string> ListResources(StreamSource source, Assembly assembly = null)
        {
            if (source == StreamSource.Default)
                throw new InvalidDataException("StreamSource.Default is too ambiguous");
            var result = new List<string>();
            if (source == StreamSource.Roaming || source == StreamSource.Local)
            {

                var files = PCLStorageExtensions.GetFiles(source == StreamSource.Roaming ? FileSystem.Current.RoamingStorage : FileSystem.Current.LocalStorage);
                if (files != null && files.Count > 0)
                    foreach (var file in files)
                        result.Add(file.Name);
            }
            else if (source == StreamSource.EmbeddedResource)
            {
                assembly = assembly ?? (Assembly)typeof(Assembly).GetTypeInfo().GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
                var resourceNames = assembly.GetManifestResourceNames();
                foreach (string resourceName in resourceNames)
                    result.Add(resourceName);
            }
            return result;
        }
    }
}

