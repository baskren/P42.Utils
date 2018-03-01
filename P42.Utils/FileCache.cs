using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils
{
    public static class FileCache
    {
        const string FileCacheFolderName = "P42.Utils.FileCache";

        static string FolderPath(string folderName = null)
        {
            if (!Directory.Exists(P42.Utils.Environment.ApplicationCachePath))
                Directory.CreateDirectory(P42.Utils.Environment.ApplicationCachePath);
            folderName = folderName ?? FileCacheFolderName;
            var folderPath = Path.Combine(P42.Utils.Environment.ApplicationCachePath, folderName);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        static object _locker = new object();
        static Dictionary<string, Task<bool>> _downloadTasks = new Dictionary<string, Task<bool>>();


        public static string Cache(string sourceFilePath, string folderName=null)
        {
            var task = Task.Run(() => CacheAsync(sourceFilePath, folderName));
            return task.Result;
        }

        public static async Task<string> CacheAsync(string sourceFilePath, string folderName=null)
        {
            //var hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(sourceFilePath.Trim()));
            //var fileName = string.Join("", hash.Select(x => x.ToString("x2")));
            var fileName = sourceFilePath.Trim().ToMd5HashString();

            return await GetCacheAsync(sourceFilePath, fileName, folderName);
        }

        static async Task<string> GetCacheAsync(string sourceFilePath, string destFileName, string folderName=null)
        {
            try
            {
                var destPath = Path.Combine(FolderPath(folderName), destFileName);
                if (System.IO.File.Exists(destPath) && !_downloadTasks.ContainsKey(destPath))
                {
                    System.Diagnostics.Debug.WriteLine("DownloadCache: [" + destPath + "] exists as [" + destFileName + "]");
                    return destPath;
                }

                var success = await CacheFileAsync(sourceFilePath, destPath);
                return success ? destPath : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        static Task<bool> CacheFileAsync(string sourcePath, string destPath)
        {
            lock (_locker)
            {
                //Task<bool> task = null;
                //if (_downloadTasks.TryGetValue(fileName, out task))
                if (_downloadTasks.TryGetValue(sourcePath, out Task<bool> task))
                    return task;

                _downloadTasks.Add(sourcePath, task = CopyTask(sourcePath, destPath));
                return task;
            }
        }


        static async Task<bool> CopyTask(string sourcePath, string destPath)
        {
            try
            {
                using (var outFileStream = System.IO.File.Create(destPath))
                using (var sourceStream = System.IO.File.OpenRead(sourcePath))
                {
                    await sourceStream.CopyToAsync(outFileStream, 81920);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            if (System.IO.File.Exists(destPath))
                System.IO.File.Delete(destPath);
            return false;
        }
    }
}
