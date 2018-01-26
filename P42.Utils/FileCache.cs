using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#if NETSTANDARD
#else
using PCLStorage;
#endif

namespace P42.Utils
{
    public static class FileCache
    {
        const string FileCacheFolderName = "FileCache";
#if NETSTANDARD
        static string _folderPath;
        static string FolderPath
        {
            get
            {
                if (_folderPath == null)
                {
                    _folderPath = Path.Combine(P42.Utils.Environment.ApplicationCachePath, FileCacheFolderName);
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
                _folder = _folder ?? FileSystem.Current.LocalStorage.CreateFolder(FileCacheFolderName, CreationCollisionOption.OpenIfExists);
                return _folder;
            }
        }
                
        static string FolderPath => Folder.Path;
#endif
        static object _locker = new object();
        static Dictionary<string, Task<bool>> _downloadTasks = new Dictionary<string, Task<bool>>();

#if NETSTANDARD

        public static string Cache(string sourceFilePath)
        {
            var task = Task.Run(() => CacheAsync(sourceFilePath));
            return task.Result;
        }

        public static async Task<string> CacheAsync(string sourceFilePath)
        {
            //var hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(sourceFilePath.Trim()));
            //var fileName = string.Join("", hash.Select(x => x.ToString("x2")));
            var fileName = sourceFilePath.Trim().ToMd5HashString();

            return await CacheAsync(sourceFilePath, fileName);
        }

        public static async Task<string> CacheAsync(string sourceFilePath, string destFileName)
        {
            try
            {
                var destPath = Path.Combine(FolderPath, destFileName);
                var exists = System.IO.File.Exists(destPath);
                if (exists && !_downloadTasks.ContainsKey(destPath))
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

#else
        public static string Cache(IFile file)
        {
            var task = Task.Run(() => CacheAsync(file));
            return task.Result;
        }

        public static async Task<string> CacheAsync(IFile file)
        {
            var hash = _md5.ComputeHash(Encoding.UTF8.GetBytes(file.Path.Trim()));
            var fileName = string.Join("", hash.Select(x => x.ToString("x2")));

            return await CacheAsync(file, fileName);
        }

        public static async Task<string> CacheAsync(IFile sourceFile, string destFileName)
        {
            try
            {
                var path = Path.Combine(FolderPath, destFileName);
                var exists = await Folder.CheckExistsAsync(destFileName);
                if (exists == ExistenceCheckResult.FileExists && !_downloadTasks.ContainsKey(path))
                {
                    System.Diagnostics.Debug.WriteLine("DownloadCache: [" + path + "] exists as [" + destFileName + "]");
                    return path;
                }

                var success = await CacheFileAsync(sourceFile, path);
                return success ? path : null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        static Task<bool> CacheFileAsync(IFile sourceFile, string fileName)
        {
            lock (_locker)
            {
                //Task<bool> task = null;
                //if (_downloadTasks.TryGetValue(fileName, out task))
                if (_downloadTasks.TryGetValue(fileName, out Task<bool> task))
                    return task;

                _downloadTasks.Add(fileName, task = CopyTask(sourceFile, fileName));
                return task;
            }
        }
#endif

#if NETSTANDARD
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
#else
        static async Task<bool> CopyTask(IFile sourceFile, string destFileName)
        {
            IFile destFile = null;
            try
            {
                destFile = Folder.CreateFile(destFileName, CreationCollisionOption.ReplaceExisting);

                using (var outFileStream = await destFile.OpenAsync(FileAccess.ReadAndWrite))
                using (var sourceStream = await sourceFile.OpenAsync(FileAccess.Read))
                {
                    await sourceStream.CopyToAsync(outFileStream, 81920);
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            if (destFile != null)
                await destFile.DeleteAsync();
            return false;
        }
#endif
    }
}
