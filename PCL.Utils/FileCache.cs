using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PCLStorage;
using System.Text;
using System.Linq;
using System.IO;
using System.Diagnostics;

namespace PCL.Utils
{
    public static class FileCache
    {
        static IFolder _folder;
        static IFolder Folder
        {
            get
            {
                _folder = _folder ?? FileSystem.Current.LocalStorage.CreateFolder("Cache", CreationCollisionOption.OpenIfExists);
                return _folder;
            }
        }
        static object _locker = new object();
        static Dictionary<string, Task<bool>> _downloadTasks = new Dictionary<string, Task<bool>>();
        static MD5 _md5 = MD5.Create();

        public static async Task<string> Download(string url)
        {
            var hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(url.Trim()));
            var fileName = string.Join("", hash.Select(x => x.ToString("x2")));

            return await Download(url, fileName);
        }

        public static async Task<string> Download(Uri url, string email)
        {
            var hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(email.Trim()));
            var fileName = string.Join("", hash.Select(x => x.ToString("x2")));

            return await Download(url.AbsoluteUri, fileName);
        }

        public static async Task<string> Download(string url, string fileName)
        {
            try
            {
                var path = Path.Combine(Folder.Path, fileName);
                var exists = await Folder.CheckExistsAsync(fileName);
                if (exists == ExistenceCheckResult.FileExists && !_downloadTasks.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("FileCache: [" + url + "] exists as [" + fileName + "]");
                    return path;
                }

                var success = await GetDownload(url, path);
                return success ? path : "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return "";
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
                if (FileSystem.Current.LocalStorage.CheckExists(fileName) == ExistenceCheckResult.FileExists)
                    System.Diagnostics.Debug.WriteLine("DownloadTask: FILE ALREADY EXISTS [" + fileName + "] [" + url + "]");
                file = await FileSystem.Current.LocalStorage.CreateFileAsync(fileName,
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
    }
}
