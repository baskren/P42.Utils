using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace P42.Utils
{

    public static class Recursion
    {
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
                    _recursionCount.Clear();
                    IsMonitoringChanged?.Invoke(null, IsEnabled);
                }

            }
        }

        public static event EventHandler<bool> IsMonitoringChanged;
        public static event EventHandler RecursionDetected;

        const int _recursionLimit = 100;
        static readonly Dictionary<string, int> _recursionCount = new Dictionary<string, int>();

        public static void Enter(string className, string instanceId, [CallerMemberName] string method = null, [CallerFilePath] string path = null)
        {

            if (Recursion.IsEnabled)
            {
                var name = className + "." + method + " : " + path + ":" + instanceId;
                _recursionCount[name] = _recursionCount.ContainsKey(name) ? _recursionCount[name] + 1 : 1;
                if (_recursionCount[name] > _recursionLimit)
                {
                    if (!Directory.Exists(P42.Utils.Environment.ApplicationCachePath))
                        Directory.CreateDirectory(P42.Utils.Environment.ApplicationCachePath);
                    var folderName = "RecursionStackTraces";
                    var folderPath = Path.Combine(P42.Utils.Environment.ApplicationCachePath, folderName);
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);
                    var fileName = DateTime.Now.ToString("yyyyMMdd'T'HHmmss.txt");
                    var filePath = Path.Combine(folderPath, fileName);
                    var stackTrace = System.Environment.StackTrace;
                    System.IO.File.WriteAllText(filePath, stackTrace);
                    RecursionDetected?.Invoke(System.Environment.StackTrace, EventArgs.Empty);
                }
            }
        }

        public static void Exit(string className, string instanceId, [CallerMemberName] string method = null, [CallerFilePath] string path = null)
        {
            if (Recursion.IsEnabled)
            {
                var name = className + "." + method + " : " + path + ":" + instanceId;
                if (_recursionCount.ContainsKey(name))
                {
                    _recursionCount[name] = _recursionCount[name] - 1;
                    if (_recursionCount[name] < 0)
                        _recursionCount[name] = 0;
                }
            }
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
