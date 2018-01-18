using System.Reflection;
using System.IO;
#if NETSTANDARD
#else
using PCLStorage;
#endif
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;


// Why is GetCallingAssembly commented out?  Because it doesn't work with UWP when .NET Native compile chain is enabled
// This might be addressed by bringing Forms9Patch.AssemblyExtensions into PCL.Extensions




namespace P42.Utils
{
    public static class ApplicationStorageExtensions
    {
        static string ApplicationStorageFolderName = "ApplicationStorage";
#if NETSTANDARD
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
#else
        static IFolder _folder;
        static IFolder Folder
        {
            get
            {
                _folder = _folder ?? FileSystem.Current.LocalStorage.CreateFolder(ApplicationStorageFolderName, CreationCollisionOption.OpenIfExists);
                return _folder;
            }
        }

        static string FolderPath => Folder.Path;
#endif


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

#if NETSTANDARD
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
#else
        public static string JsonFromStorage(string uid, string fileName, IFolder folder)
        {
            string contents = null;
            if (uid != null)
            {
                if (folder.CheckExists("uid-" + uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder("uid-" + uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder("uid-" + uid);
            }
            if (folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
            {
                var file = folder.GetFile(fileName);
                contents = file?.ReadAllText();
            }
            return contents;
        }
#endif

#if NETSTANDARD
        public static string LoadLocalStorageText(string uid, string fileName)
        {
            return JsonFromStorage(uid, fileName);
        }
#else
        public static string LoadLocalStorageText(string uid, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            //System.Diagnostics.Debug.WriteLine("LocalFolder=" + folder?.Path);
            return folder == null ? null : JsonFromStorage(uid, fileName, folder);
        }
#endif

        public static string LoadText(string uid, string fileName, Assembly assembly = null)
        {
            var result = LoadLocalStorageText(uid, fileName);
            if (result == null)
                result = EmbeddedStoredText(fileName, assembly);
            return result;
        }

#if NETSTANDARD
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
#else
        public static void StoreText(string uid, string text, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            if (folder == null)
                throw new InvalidDataContractException("Hey, there should be a LocalStorage folder!");
            if (uid != null)
            {
                if (folder.CheckExists("uid-" + uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder("uid-" + uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder("uid-" + uid);
            }
            var tempFileName = Guid.NewGuid().ToString();

            //IFile file = folder.CreateFile(fileName, CreationCollisionOption.ReplaceExisting);
            //file.WriteAllText(text);
            Task.Run(async () =>
            {
                IFile file = folder.CreateFile(tempFileName, CreationCollisionOption.ReplaceExisting);
                await file.WriteAllTextAsync(text);
                file.Rename(fileName, NameCollisionOption.ReplaceExisting);
            });
        }
#endif

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

#if NETSTANDARD
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
#else
        public static bool LocalResourceAvailable(string uid, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            if (uid != null)
            {
                if (folder.CheckExists("uid-" + uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder("uid-" + uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder("uid-" + uid);
            }
            return folder.CheckExists(fileName) == ExistenceCheckResult.FileExists;
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
                if (folder.CheckExists("uid-" + uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder("uid-" + uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder("uid-" + uid);
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
#endif

        public static StreamWriter ResourceStreamWriter(string uid, string resourceName)
        {
            var streamWriter = LocalStreamWriter(uid, resourceName);
            return streamWriter;
        }

#if NETSTANDARD
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
#else
        public static StreamWriter LocalStreamWriter(string uid, string fileName)
        {
            var folder = FileSystem.Current.LocalStorage;
            return folder == null ? null : StreamWriterFromStoredFolder(uid, fileName, folder);
        }

        public static StreamWriter StreamWriterFromStoredFolder(string uid, string fileName, IFolder folder)
        {
            if (uid != null)
            {
                if (folder.CheckExists("uid-" + uid) != ExistenceCheckResult.FolderExists)
                    folder.CreateFolder("uid-" + uid, CreationCollisionOption.FailIfExists);
                folder = folder.GetFolder("uid-" + uid);
            }
            System.Diagnostics.Debug.WriteLine("Writing to [" + folder.Path + "][" + fileName + "]");
            var file = folder.CreateFile(fileName, CreationCollisionOption.ReplaceExisting);
            var stream = file.Open(FileAccess.ReadAndWrite);
            var streamWriter = new StreamWriter(stream);
            return streamWriter;
        }
#endif

        public static List<string> ListResources(StreamSource source, Assembly assembly = null)
        {
            if (source == StreamSource.Default)
                throw new InvalidDataException("StreamSource.Default is too ambiguous");
            var result = new List<string>();
            if (source == StreamSource.Local)
            {
#if NETSTANDARD
                var fileNames = Directory.GetFiles(FolderPath);// Storage.GetFileNames();
                if (fileNames != null && fileNames.Any())
                    foreach (var fileName in fileNames)
                        result.Add(fileName);
#else
                var files = PCLStorageExtensions.GetFiles(FileSystem.Current.LocalStorage);
                if (files != null && files.Any())
                    foreach (var file in files)
                        result.Add(file.Name);
#endif
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

