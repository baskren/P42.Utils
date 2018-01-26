using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if NETSTANDARD
#else
using PCLStorage;
#endif
using System.Text;
using System.IO;
using System.Diagnostics;

namespace P42.Utils
{
    public static class DownloadCache
    {
        const string DownloadStorageFolderName = "DownloadCache";

#if NETSTANDARD
        static string _folderPath;
        static string FolderPath
        {
            get
            {
                if (_folderPath == null)
                {
                    _folderPath = Path.Combine(P42.Utils.Environment.ApplicationCachePath, DownloadStorageFolderName);
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
                _folder = _folder ?? FileSystem.Current.LocalStorage.CreateFolder(DownloadStorageFolderName, CreationCollisionOption.OpenIfExists);
                return _folder;
            }
        }

        static string FolderPath => Folder.Path;
#endif
        static object _locker = new object();
        static Dictionary<string, Task<bool>> _downloadTasks = new Dictionary<string, Task<bool>>();
        static System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();

        public static string Download(string url)
        {
            var task = Task.Run(() => DownloadAsync(url));
            return task.Result;
        }

        

        public static async Task<string> DownloadAsync(string url)
        {
            var fileName = url.Trim().ToMd5HashString();
            return await DownloadAsync(url, fileName);
        }

        public static bool IsCached(string url)
        {
            //var hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(url.Trim()));
            //var fileName = string.Join("", hash.Select(x => x.ToString("x2")));
            var fileName = url.Trim().ToMd5HashString();
            var path = Path.Combine(FolderPath, fileName);
#if NETSTANDARD
            return System.IO.File.Exists(path) && !_downloadTasks.ContainsKey(path);
#else
            var exists = Folder.CheckExists(fileName);
            return (exists == ExistenceCheckResult.FileExists && !_downloadTasks.ContainsKey(path));
#endif
        }

        public static async Task<string> DownloadAsync(string url, string fileName)
        {
            try
            {
                var path = Path.Combine(FolderPath, fileName);
#if NETSTANDARD
                if (System.IO.File.Exists(path) && !_downloadTasks.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("DownloadCache: [" + url + "] exists as [" + path + "]");
                    return path;
                }
#else
                var exists = await Folder.CheckExistsAsync(fileName);
                if (exists == ExistenceCheckResult.FileExists && !_downloadTasks.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("DownloadCache: [" + url + "] exists as [" + path + "]");
                    return path;
                }
#endif

                var success = await GetDownload(url, path);
                return success ? path : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        static Task<bool> GetDownload(string url, string fileName)
        {
            lock (_locker)
            {
                //Task<bool> task = null;
                //if (_downloadTasks.TryGetValue(fileName, out task))
                if (_downloadTasks.TryGetValue(fileName, out Task<bool> task))
                    return task;

                _downloadTasks.Add(fileName, task = DownloadTask(url, fileName));
                return task;
            }
        }

#if NETSTANDARD
        static async Task<bool> DownloadTask(string url, string path)
        {

            try
            {
                var client = new System.Net.Http.HttpClient();
                var data = await client.GetByteArrayAsync(url);
                System.Diagnostics.Debug.WriteLine("DownloadTask: byte array downloaded for [" + url + "]");
                var fileNamePaths = path.Split('\\');
                path = fileNamePaths[fileNamePaths.Length - 1];
                if (System.IO.File.Exists(path))
                    System.Diagnostics.Debug.WriteLine("DownloadTask: FILE ALREADY EXISTS [" + path + "] [" + url + "]");
                using (var stream = new FileStream(path, FileMode.Create))
                //using (var stream = new IsolatedStorageFileStream(path, FileMode.Create, Storage))
                {
                    stream.Write(data, 0, data.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            return false;
        }
#else
        static async Task<bool> DownloadTask(string url, string fileName)
        {
            IFile file = null;
            try
            {
                var client = new System.Net.Http.HttpClient();
                var data = await client.GetByteArrayAsync(url);
                System.Diagnostics.Debug.WriteLine("DownloadTask: byte array downloaded for [" + url + "]");
                var fileNamePaths = fileName.Split('\\');
                fileName = fileNamePaths[fileNamePaths.Length - 1];
                if (Folder.CheckExists(fileName) == ExistenceCheckResult.FileExists)
                    System.Diagnostics.Debug.WriteLine("DownloadTask: FILE ALREADY EXISTS [" + fileName + "] [" + url + "]");
                file = await Folder.CreateFileAsync(fileName,
                    CreationCollisionOption.ReplaceExisting);
                using (var fileStream = await file.OpenAsync(FileAccess.ReadAndWrite))
                {
                    fileStream.Write(data, 0, data.Length);
                    System.Diagnostics.Debug.WriteLine("DownloadTask: file written [" + fileName + "] [" + url + "]");
                }
                return true;
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
