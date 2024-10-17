using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using AsyncAwaitBestPractices;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class Recursion
{

    private const string RecursionFolderName = "P42.Utils.RecursionStackTraces";

    private static string? _folderPath;
    private static string FolderPath
    {
        get
        {
            if (_folderPath != null)
                return _folderPath;

            DirectoryExtensions.GetOrCreateDirectory(Environment.ApplicationCachePath);
            var folderPath = Path.Combine(Environment.ApplicationCachePath, RecursionFolderName);
            DirectoryExtensions.GetOrCreateDirectory(folderPath);
            _folderPath = folderPath;
            return _folderPath;
        }
    }


    static bool _enabled;
    /// <summary>
    /// Enable or Disable Threading
    /// </summary>
    public static bool IsEnabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value)
                return;

            _enabled = value;
            //_monitoringCount = 0;
            RecursionCount.Clear();
            WeakIsMonitoringChanged.RaiseEvent(null, IsEnabled, nameof(IsMonitoringChanged));
        }
    }

    private static readonly WeakEventManager<bool> WeakIsMonitoringChanged = new ();
    /// <summary>
    /// Event fired when IsEnabled is toggled
    /// </summary>
    public static event EventHandler<bool> IsMonitoringChanged
    {
        add => WeakIsMonitoringChanged.AddEventHandler(value);
        remove => WeakIsMonitoringChanged.RemoveEventHandler(value);
    }
    
    private static readonly WeakEventManager WeakRecursionDetected = new ();
    /// <summary>
    /// Event fired when recursion threshold has been crossed
    /// </summary>
    public static event EventHandler RecursionDetected
    {
        add => WeakRecursionDetected.AddEventHandler(value);
        remove => WeakRecursionDetected.RemoveEventHandler(value);
    }

    private const int RecursionLimit = 100;
    private static readonly Dictionary<string, int> RecursionCount = new ();

    private static readonly Dictionary<string, DateTime> RecursionTime = new ();

    /// <summary>
    /// Method to be called at entry of monitored method
    /// </summary>
    /// <param name="type"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Enter(Type type, object? instanceId, [CallerMemberName] string? method = null, [CallerFilePath] string? path = null)
        => Enter(type.ToString(), instanceId?.ToString(), method, path);

    /// <summary>
    /// Method to be called at entry of monitored method
    /// </summary>
    /// <param name="className"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Enter(string className, string? instanceId, [CallerMemberName] string? method = null, [CallerFilePath] string? path = null)
    {
        if (!IsEnabled)
            return;

        var name = $"{className}.{method} : {path}:{instanceId}";
        if (!RecursionTime.ContainsKey(name))
            RecursionTime[name] = DateTime.Now;
        
        RecursionCount[name] = RecursionCount.TryGetValue(name, out var value) 
            ? value + 1 
            : 1;

        if (RecursionCount[name] <= RecursionLimit)
            return;

        var fileName = $"{DateTime.Now:yyyyMMdd'T'HHmmss}.txt";
        var filePath = Path.Combine(FolderPath, fileName);
        var stackTrace = System.Environment.StackTrace;
        File.WriteAllText(filePath, stackTrace);
        WeakRecursionDetected.RaiseEvent(System.Environment.StackTrace, EventArgs.Empty, nameof(RecursionDetected));
    }

    /// <summary>
    /// Method to be called at exit of monitored method
    /// </summary>
    /// <param name="type"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Exit(Type type, object? instanceId, [CallerMemberName] string? method = null, [CallerFilePath] string? path = null)
        => Exit(type.ToString(), instanceId?.ToString(), method, path);

    /// <summary>
    /// Method to be called at exit of monitored method
    /// </summary>
    /// <param name="className"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Exit(string className, string? instanceId, [CallerMemberName] string? method = null, [CallerFilePath] string? path = null)
    {
        if (!IsEnabled)
            return;

        var name = $"{className}.{method} : {path}:{instanceId}";
        if (!RecursionCount.TryGetValue(name, out var value))
            return;

        RecursionCount[name] = value - 1;
        if (RecursionCount[name] < 0)
            RecursionCount[name] = 0;
        if (RecursionCount[name] != 0 || !RecursionTime.Remove(name, out var value1))
            return;

        Console.WriteLine($"RecursionTime: {name} [{(DateTime.Now - value1).TotalMilliseconds}ms]");
    }

    /// <summary>
    /// List content of recursion log
    /// </summary>
    /// <returns></returns>
    public static string[] List()
    {
        try
        {
            var files = Directory.GetFiles(FolderPath);
            Array.Sort(files);
            return files;
        }
        catch (Exception ex)
        {
            QLog.Error(ex);
        }
        return [];
    }


    /// <summary>
    /// Clear recursion log
    /// </summary>
    /// <returns></returns>
    public static bool Clear()
        => Clear(DateTime.MinValue.AddYears(1));

    /// <summary>
    /// Clear entries older than timeSpan from recursion log
    /// </summary>
    /// <param name="timeSpan"></param>
    /// <returns></returns>
    public static bool Clear(TimeSpan timeSpan)
        => Clear(DateTime.Now - timeSpan);

    /// <summary>
    /// Clear entries older than dateTime from recursion log
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns></returns>
    public static bool Clear(DateTime dateTime)
    {
        // complete clear
        if (!Directory.Exists(FolderPath))
            return false;

        var files = Directory.EnumerateFiles(FolderPath);
        var filesRemaining = false;
        foreach (var file in files)
        {
            if (!File.Exists(file))
                continue;

            if (File.GetLastWriteTime(file) < dateTime)
                File.Delete(file);
            else
                filesRemaining = true;
        }
        return filesRemaining;
    }



    /*
    public static void Test()
    {
        Enter(typeof(Recursion).ToString(), "0");
        Test();
        Exit(typeof(Recursion).ToString(), "0");
    }
    */
}

/*
public class RecursionException : Exception
{
    public RecursionException() { }

    //protected RecursionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public RecursionException(string message) : base(message) { }

    public RecursionException(string message, Exception innerException) : base(message, innerException) { }

}
*/
