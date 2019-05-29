using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils
{
    public class TextCache
    {
        const string DownloadStorageFolderName = "P42.Utils.TextCache";

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
        static readonly System.Security.Cryptography.MD5 _md5 = System.Security.Cryptography.MD5.Create();



        public static void Store(string text, string key, string folderName = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                return;

            var path = CachedPath(key, folderName);

            if (string.IsNullOrWhiteSpace(path))
                return;

            var guid = Guid.NewGuid().ToString();
            var tmpPath = CachedPath(guid, folderName);

            System.IO.File.WriteAllText(tmpPath, text);

            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);

            System.IO.File.Move(tmpPath, path);

            System.IO.File.Delete(tmpPath);
        }

        public static string Recall(string key, string folderName = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(key))
                    return null;

                var path = CachedPath(key, folderName);

                if (string.IsNullOrWhiteSpace(path))
                    return null;
                if (System.IO.File.Exists(path))
                    return System.IO.File.ReadAllText(path);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        static string CachedPath(string key, string folderName = null)
        {
            var fileName = key.Trim().ToMd5HashString();
            return Path.Combine(FolderPath(folderName), fileName); ;
        }

        public static bool IsCached(string key, string folderName = null)
        {
            var path = CachedPath(key, folderName);
            return path != null && System.IO.File.Exists(path);
        }

        public static bool Clear(string key = null, string folderName = null)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                // complete clear
                var folderPath = FolderPath(folderName);
                if (System.IO.Directory.Exists(folderPath))
                {
                    var files = System.IO.Directory.EnumerateFiles(folderPath);
                    foreach (var file in files)
                        System.IO.File.Delete(file);
                    System.IO.Directory.Delete(folderPath);
                    return true;
                }
                return false;
            }
            var path = CachedPath(key, folderName);
            if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;
        }

    }
}
