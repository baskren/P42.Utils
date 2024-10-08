using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace P42.Utils
{
    public static class BreadCrumbs
    {
        const string BreadCrumbFolderName = "P42.Utils.BreadCrumbs";

        static string _folderPath;
        static string FolderPath
        {
            get
            {
                if (_folderPath == null)
                {
                    DirectoryExtensions.AssureExists(Environment.ApplicationCachePath);
                    var folderPath = Path.Combine(P42.Utils.Environment.ApplicationCachePath, BreadCrumbFolderName);
                    DirectoryExtensions.AssureExists(folderPath);
                    _folderPath = folderPath;
                }
                return _folderPath;
            }
        }

        public static bool IsEnabled
        {
            get => _streamWriter != null;
            set
            {
                if (IsEnabled != value)
                {
                    if (_streamWriter != null)
                    {
                        _streamWriter.Flush();
                        _streamWriter.Close();
                        _streamWriter.Dispose();
                        _streamWriter = null;
                    }
                    else
                    {
                        var fileName = DateTime.Now.ToString("yyyyMMdd'T'HHmmss") + ".txt";
                        var filePath = Path.Combine(FolderPath, fileName);
                        _streamWriter = File.CreateText(filePath);
                    }
                }
            }
        }

        public static Action<string, string, string, int, string> EventAction;
        public static Action<Exception, string, string, string, int, string> ExceptionAction;

        static StreamWriter _streamWriter;


        public static void Add(Type type, object crumb = null, [CallerMemberName] string method = null, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0, [CallerFilePath] string path = null)
            => Add(type?.ToString(), crumb?.ToString(), method, lineNumber, path);

        public static void Add(string className, string crumb = null, [CallerMemberName] string method = null, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0, [CallerFilePath] string path = null)
        {
            if (IsEnabled)
            {
                _streamWriter.WriteLine("[" + DateTime.Now.ToString("o") + "] [" + className + "." + method + ":" + lineNumber + "] [" + crumb + "] ");
                _streamWriter.Flush();
            }
            EventAction?.Invoke(className, crumb, method, lineNumber, path);
        }

        public static void AddException(Exception e, Type type, object crumb = null, [CallerMemberName] string method = null, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0, [CallerFilePath] string path = null)
            => AddException(e, type?.ToString(), crumb?.ToString(), method, lineNumber, path);

        public static void AddException(Exception e, string className, string crumb = null, [CallerMemberName] string method = null, [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0, [CallerFilePath] string path = null)
        {
            if (IsEnabled)
            {
                _streamWriter.WriteLine("[" + DateTime.Now.ToString("o") + "] [" + className + "." + method + ":" + lineNumber + "] [" + crumb + "] ");
                _streamWriter.Flush();
            }
            ExceptionAction?.Invoke(e, className, crumb, method, lineNumber, path);
        }

        public static string[] List()
        {
            try
            {
                var files = Directory.GetFiles(FolderPath);
                Array.Sort(files);
                return files;
            }
            catch (Exception)
            {

            }
            return null;
        }

        public static bool Clear()
            => Clear(DateTime.MinValue.AddYears(1));

        public static bool Clear(TimeSpan timeSpan)
            => Clear(DateTime.Now - timeSpan);

        public static bool Clear(DateTime dateTime)
        {
            // complete clear
            if (System.IO.Directory.Exists(FolderPath))
            {
                var files = System.IO.Directory.EnumerateFiles(FolderPath);
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
                }
                return filesRemaining;
            }
            return false;
        }


        public static void SplitProperties(Dictionary<string, string> breadcrumbs, int max = 125)
        {
            var keys = breadcrumbs.Keys.ToArray();
            foreach (var key in keys)
            {
                string property = null;
                if (breadcrumbs?.TryGetValue(key, out property) ?? false)
                {
                    //if (!string.IsNullOrWhiteSpace(RedundantPath) && property.StartsWith(RedundantPath))
                    //    property = property.Substring(_redundantPathLength);
                    if (!string.IsNullOrWhiteSpace(property))
                    {
                        if (property.Length > max)
                        {
                            breadcrumbs[key] = property.Substring(0, max);
                            for (int i = 1; i * max < property.Length; i++)
                                breadcrumbs[key + "." + i] = property.Substring(i * max, Math.Min(max, property.Length - (i * max)));
                        }
                        else
                            breadcrumbs[key] = property;
                    }
                }
            }
        }

    }
}
