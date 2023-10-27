﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace P42.Utils
{

    public static class Recursion
    {

        const string RecursionFolderName = "P42.Utils.RecursionStackTraces";

        static string _folderPath;
        static string FolderPath
        {
            get
            {
                if (_folderPath == null)
                {
                    if (!Directory.Exists(P42.Utils.Environment.ApplicationCachePath))
                        Directory.CreateDirectory(P42.Utils.Environment.ApplicationCachePath);
                    var folderPath = Path.Combine(P42.Utils.Environment.ApplicationCachePath, RecursionFolderName);
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);
                    _folderPath = folderPath;
                }
                return _folderPath;
            }
        }


        static bool _enabled;
        public static bool IsEnabled
        {
            get => _enabled;
            set
            {

                if (_enabled != value)
                {
                    _enabled = value;
                    //_monitoringCount = 0;
                    RecursionCount.Clear();
                    IsMonitoringChanged?.Invoke(null, IsEnabled);
                }

            }
        }

        public static event EventHandler<bool> IsMonitoringChanged;
        public static event EventHandler RecursionDetected;

        const int _recursionLimit = 100;
        public static readonly ConcurrentDictionary<string, int> RecursionCount = new ConcurrentDictionary<string, int>();

        //static readonly Dictionary<string, DateTime> _recursionTime = new Dictionary<string, DateTime>();

        public static void Enter(Type type, object instanceId, [CallerMemberName] string method = null, [CallerFilePath] string path = null)
            => Enter(type?.ToString(), instanceId?.ToString(), method, path);

        public static void Enter(string className, string instanceId, [CallerMemberName] string method = null, [CallerFilePath] string path = null)
        {

            if (Recursion.IsEnabled)
            {
                var name = className + "." + method + " : " + path + ":" + instanceId;
                //if (!_recursionTime.ContainsKey(name))
                //    _recursionTime[name] = DateTime.Now;
                RecursionCount[name] = RecursionCount.ContainsKey(name) ? RecursionCount[name] + 1 : 1;
                if (RecursionCount[name] > _recursionLimit)
                {
                    var fileName = DateTime.Now.ToString("yyyyMMdd'T'HHmmss") + ".txt"; ;
                    var filePath = Path.Combine(FolderPath, fileName);
                    try
                    {
                        var stackTrace = new System.Diagnostics.StackTrace();
                        var traceText = stackTrace.ToString();
                        System.IO.File.WriteAllText(filePath, traceText);
                        RecursionDetected?.Invoke(traceText, EventArgs.Empty);
                    }
                    catch (Exception) { }
                }
            }
        }

        public static void Exit(Type type, object instanceId, [CallerMemberName] string method = null, [CallerFilePath] string path = null)
            => Exit(type?.ToString(), instanceId?.ToString(), method, path);

        public static void Exit(string className, string instanceId, [CallerMemberName] string method = null, [CallerFilePath] string path = null)
        {
            if (Recursion.IsEnabled)
            {
                var name = className + "." + method + " : " + path + ":" + instanceId;
                if (RecursionCount.ContainsKey(name))
                {
                    RecursionCount[name] = RecursionCount[name] - 1;
                    if (RecursionCount[name] < 0)
                        RecursionCount[name] = 0;
                    /*
                    if (RecursionCount[name] == 0 && _recursionTime.ContainsKey(name))
                    {
                        var delta = DateTime.Now - _recursionTime[name];
                        _recursionTime.Remove(name);
                        Console.WriteLine("RecursionTime: " + name + " [" + delta.TotalMilliseconds + "ms]");
                    }
                    */
                }

            }
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



        public static void Test()
        {
            Enter(typeof(Recursion).ToString(), "0");
            Test();
            Exit(typeof(Recursion).ToString(), "0");
        }
    }


    public class RecursionException : Exception
    {
        public RecursionException() { }

        protected RecursionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public RecursionException(string message) : base(message) { }

        public RecursionException(string message, Exception innerException) : base(message, innerException) { }

    }



}
