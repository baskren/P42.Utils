using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace P42.Utils
{
    public static class DownloadCache
    {
        const string DownloadStorageFolderName = "P42.Utils.DownloadCache";

        static string FolderPath(string folderName)
        {
            if (!Directory.Exists(P42.Utils.Environment.ApplicationCachePath))
                Directory.CreateDirectory(P42.Utils.Environment.ApplicationCachePath);
            folderName = folderName ?? DownloadStorageFolderName;
            var folderPath = Path.Combine(P42.Utils.Environment.ApplicationCachePath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        static readonly object _locker = new object();
        static readonly Dictionary<string, Task<bool>> _downloadTasks = new Dictionary<string, Task<bool>>();

        public static string Download(string url, string folderName = null)
        {
            var task = Task.Run(() => DownloadAsync(url, folderName));
            return task.Result;
        }

        public static List<string> List(string folderName)
        {
            var folderPath = FolderPath(folderName);
            var files = System.IO.Directory.EnumerateFiles(folderPath);
            return files.ToList();
        }



        public static async Task<string> DownloadAsync(string url, string folderName = null)
        {
            return await GetDownloadAsync(url, folderName);
        }

        static string CachedPath(string url, string folderName = null)
        {
            var fileName = url.Trim().ToMd5HashString();
            var path = Path.Combine(FolderPath(folderName), fileName); ;
            //return System.IO.File.Exists(path) ? path : null;
            return path;
        }

        public static bool IsCached(string url, string folderName = null)
        {
            var path = CachedPath(url, folderName);
            return path != null && System.IO.File.Exists(path) && !_downloadTasks.ContainsKey(path);
        }

        public static bool Clear(string url = null, string folderName = null)
            => Clear(DateTime.MinValue, url, folderName);

        public static bool Clear(TimeSpan timeSpan, string url = null, string folderName = null)
            => Clear(DateTime.Now - timeSpan, url, folderName);

        public static bool Clear(DateTime dateTime, string url = null, string folderName = null)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                // complete clear
                var folderPath = FolderPath(folderName);
                if (System.IO.Directory.Exists(folderPath))
                {
                    var files = System.IO.Directory.EnumerateFiles(folderPath);
                    bool filesRemaining = false;
                    foreach (var file in files)
                    {
                        if (System.IO.File.Exists(file))
                        {
                            if (System.IO.File.GetLastWriteTime(file) < dateTime)
                                System.IO.File.Delete(file);
                            else
                                filesRemaining = true;
                        }
                        if (!filesRemaining)
                            System.IO.Directory.Delete(folderPath);
                        return true;
                    }
                }
                return false;
            }
            var path = CachedPath(url, folderName);
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                if (System.IO.File.GetLastWriteTime(path) < dateTime)
                {
                    System.IO.File.Delete(path);
                    return true;
                }
            }
            return false;
        }

        static async Task<string> GetDownloadAsync(string url, string folderName = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(url))
                    return null;
                var path = CachedPath(url, folderName);
                if (string.IsNullOrWhiteSpace(path))
                    return null;
                if (System.IO.File.Exists(path) && !_downloadTasks.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("DownloadCache: [" + url + "] exists as [" + path + "]");
                    return path;
                }

                var success = await GetDownloadCoreAsync(url, path);
                return success ? path : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }

        static Task<bool> GetDownloadCoreAsync(string url, string fileName)
        {
            lock (_locker)
            {
                //Task<bool> task = null;
                //if (_downloadTasks.TryGetValue(fileName, out task))
                if (_downloadTasks.TryGetValue(fileName, out Task<bool> task))
                    return task;

                _downloadTasks.Add(fileName, task = DownloadTaskAsync(url, fileName));
                return task;
            }
        }

        static async Task<bool> DownloadTaskAsync(string url, string path)
        {

            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    var data = await client.GetByteArrayAsync(url);
                    System.Diagnostics.Debug.WriteLine("DownloadTask: byte array downloaded for [" + url + "]");
                    var fileNamePaths = path.Split('\\');
                    if (System.IO.File.Exists(path))
                        System.Diagnostics.Debug.WriteLine("DownloadTask: FILE ALREADY EXISTS [" + path + "] [" + url + "]");
                    using (var stream = new FileStream(path, FileMode.Create))
                    //using (var stream = new IsolatedStorageFileStream(path, FileMode.Create, Storage))
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            return false;
        }
    }
}
