using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if NETSTANDARD
#else
using PCLStorage;
#endif
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace P42.Utils
{
    static public class EmbeddedResourceCache
    {
        //const string LocalStorageFolderName = "EmbeddedResourceCache";

#if NETSTANDARD
        static string _folderPath;
        static string FolderPath
        {
            get
            {
                if (_folderPath==null)
                {
                    //_folderPath = Path.Combine(P42.Utils.Environment.ApplicationDataPath, LocalStorageFolderName);
                    _folderPath = P42.Utils.Environment.ApplicationDataPath;
                    if (!Directory.Exists(_folderPath))
                        Directory.CreateDirectory(_folderPath);
                }
                return _folderPath;
            }
        }            
#else
        static IFolder _folder;
        static IFolder Folder
        {
            get
            {
                if (_folder == null)
                {
                    var x = PCLStorage.FileSystem.Current.LocalStorage.Path;
                    //var existance = FileSystem.Current.LocalStorage.CheckExists(LocalStorageFolderName);
                    //if (existance == ExistenceCheckResult.NotFound)
                    //    _folder = FileSystem.Current.LocalStorage.CreateFolder(LocalStorageFolderName, CreationCollisionOption.OpenIfExists);
                    //else if (existance == ExistenceCheckResult.FolderExists)
                    //    _folder = FileSystem.Current.LocalStorage.GetFolder(LocalStorageFolderName);
                    //else
                    //    throw new InvalidDataException("A file was found that matches the expected LocalStorage folder named [" + LocalStorageFolderName + "] ");
                    _folder = FileSystem.Current.LocalStorage;
                }
                //_folder = _folder ?? FileSystem.Current.LocalStorage.CreateFolder(LocalStorageFolderName, CreationCollisionOption.OpenIfExists);
                return _folder;
            }
        }
        static string FolderPath => Folder.Path;
#endif

        static object _locker = new object();
        static Dictionary<string, Task<bool>> _cacheTasks = new Dictionary<string, Task<bool>>();
        //static MD5 _md5 = MD5.Create();

        public static string LocalStorageSubPathForEmbeddedResource(string resourceId, Assembly assembly)
        {
            var task = Task.Run(() => LocalStorageSubPathForEmbeddedResourceAsync(resourceId, assembly));
            return task.Result;
        }

        public static async Task<string> LocalStorageSubPathForEmbeddedResourceAsync(string resourceId, Assembly assembly)
        {
            //var hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(assembly.GetName().Name + ";" + resourceId.Trim()));
            //var fileName = string.Join("", hash.Select(x => x.ToString("x2")));
            var fileName = resourceId;

            try
            {
                var path = Path.Combine(FolderPath, fileName);
                System.Diagnostics.Debug.WriteLine("PATH= " + path);
#if NETSTANDARD
                var exists = System.IO.File.Exists(path);
#else
                var exists = Folder.CheckExists(fileName) == ExistenceCheckResult.FileExists;
#endif
                if (exists && !_cacheTasks.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("EmbeddedResourceCache: [" + assembly.GetName().Name + ";" + resourceId + "] exists as [" + path + "]");
                    //return Path.Combine(LocalStorageFolderName, fileName);
                    return fileName;
                }

                var success = await CacheEmbeddedResource(resourceId, assembly, path);
                //return success ? Path.Combine(LocalStorageFolderName,fileName) : null;
                return success ? fileName : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        static Task<bool> CacheEmbeddedResource(string resourceId, Assembly assembly, string fileName)
        {
            lock (_locker)
            {
                //Task<bool> task = null;
                //if (_downloadTasks.TryGetValue(fileName, out task))
                if (_cacheTasks.TryGetValue(fileName, out Task<bool> task))
                    return task;

                _cacheTasks.Add(fileName, task = CacheTask(resourceId, assembly, fileName));
                return task;
            }
        }

#if NETSTANDARD
#pragma warning disable CS1998
        static async Task<bool> CacheTask(string resourceId, Assembly assembly, string path)
#pragma warning restore CS1998
        {
            try
            {
                using (var stream = EmbeddedResource.GetStream(resourceId, assembly))
                {
                    if (stream != null)
                    {
                        //var fileNamePaths = path.Split('\\');
                        //path = fileNamePaths[fileNamePaths.Length - 1];
                        if (System.IO.File.Exists(path))
                            System.Diagnostics.Debug.WriteLine("DownloadTask: FILE ALREADY EXISTS [" + path + "] [" + assembly.GetName().Name + ";" + resourceId + "]");

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fileStream);
                            fileStream.Flush(true);
                           

                            var length = fileStream.Length;
                            System.Diagnostics.Debug.WriteLine("DownloadTask: file written [" + path + "] [" + assembly.GetName().Name + ";" + resourceId + "] length=["+length+"] name=["+fileStream.Name+"] pos=["+fileStream.Position+"]");
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            if (File.Exists(path))
                System.IO.File.Delete(path);
            return false;
        }
#else
        static async Task<bool> CacheTask(string resourceId, Assembly assembly, string fileName)
        {
            IFile file = null;
            try
            {
                using (var stream = EmbeddedResource.GetStream(resourceId, assembly))
                {
                    if (stream != null)
                    {
                        var fileNamePaths = fileName.Split('\\');
                        fileName = fileNamePaths[fileNamePaths.Length - 1];
                        if (Folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
                            System.Diagnostics.Debug.WriteLine("DownloadTask: FILE ALREADY EXISTS [" + fileName + "] [" + assembly.GetName().Name + ";" + resourceId + "]");
                        file = await Folder.CreateFileAsync(fileName,
                            CreationCollisionOption.ReplaceExisting);
                        using (var fileStream = await file.OpenAsync(FileAccess.ReadAndWrite))
                        {
                            //fileStream.Write(data, 0, data.Length);
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fileStream);

                            fileStream.Flush();
                            System.Diagnostics.Debug.WriteLine("DownloadTask: file written [" + file.Path + "] [" + assembly.GetName().Name + ";" + resourceId + "] length=[" + fileStream.Length + "]  pos=[" + fileStream.Position + "]");
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            if (file != null)
                await file.DeleteAsync();
            return false;
        }
#endif
    }
}
