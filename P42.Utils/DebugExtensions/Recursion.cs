using System.Runtime.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class Recursion
{
    private const string RecursionFolderName = "P42.Utils.RecursionStackTraces";

    private static string? _folderPath;

    /// <summary>
    /// Where to store the recursion log
    /// </summary>
    private static string FolderPath
    {
        get
        {
            if (_folderPath != null)
                return _folderPath;

#pragma warning disable CS0618 // Type or member is obsolete
            DirectoryExtensions.GetOrCreateDirectory(Platform.ApplicationLocalCacheFolderPath);
            var folderPath = Path.Combine(Platform.ApplicationLocalCacheFolderPath, RecursionFolderName);
#pragma warning restore CS0618 // Type or member is obsolete
            DirectoryExtensions.GetOrCreateDirectory(folderPath);
            _folderPath = folderPath;
            return _folderPath;
        }
    }


    private static bool _enabled;
    /// <summary>
    /// Is recursion logging enabled?
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
            IsMonitoringChanged?.Invoke(null, IsEnabled);

        }
    }

    /// <summary>
    /// Fires when recursion logging is toggled
    /// </summary>
    // ReSharper disable once EventNeverSubscribedTo.Global
    public static event EventHandler<bool>? IsMonitoringChanged;
    
    /// <summary>
    /// Fired when a recursion is detected
    /// </summary>
    // ReSharper disable once EventNeverSubscribedTo.Global
    public static event EventHandler? RecursionDetected;

    private const int RecursionLimit = 100;
    private static readonly Dictionary<string, int> RecursionCount = new();
    private static readonly Dictionary<string, DateTime> RecursionTime = new();

    /// <summary>
    /// Mark the entry into a method
    /// </summary>
    /// <param name="type"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Enter(Type type, object instanceId, [CallerMemberName] string method = "", [CallerFilePath] string path = "")
        => Enter(type.ToString(), instanceId.ToString() ?? string.Empty, method, path);

    /// <summary>
    /// Mark the entry into a method
    /// </summary>
    /// <param name="className"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Enter(string className, string instanceId, [CallerMemberName] string method = "", [CallerFilePath] string path = "")
    {
        if (!IsEnabled)
            return;

        var name = $"{className}.{method} : {path}:{instanceId}";
        if (RecursionCount.TryAdd(name, 1))
            RecursionTime[name] = DateTime.Now;
        else
            RecursionCount[name]++;

        if (RecursionCount[name] <= RecursionLimit)
            return;

        var fileName = $"{DateTime.Now:yyyyMMdd'T'HHmmss}.txt";
        var filePath = Path.Combine(FolderPath, fileName);
        var stackTrace = Environment.StackTrace;
        DirectoryExtensions.GetOrCreateParentDirectory(fileName);
        File.WriteAllText(filePath, stackTrace);

        if (RecursionDetected is null)
            throw new RecursionException(name);
        
        RecursionDetected.Invoke(Environment.StackTrace, EventArgs.Empty);
    }

    /// <summary>
    /// Mark the exit from a method
    /// </summary>
    /// <param name="type"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Exit(Type type, object instanceId, [CallerMemberName] string method = "", [CallerFilePath] string path = "")
        => Exit(type.ToString(), instanceId.ToString() ?? string.Empty, method, path);

    /// <summary>
    /// Mark the exit from a method
    /// </summary>
    /// <param name="className"></param>
    /// <param name="instanceId"></param>
    /// <param name="method"></param>
    /// <param name="path"></param>
    public static void Exit(string className, string instanceId, [CallerMemberName] string method = "", [CallerFilePath] string path = "")
    {
        if (!IsEnabled)
            return;

        var name = $"{className}.{method} : {path}:{instanceId}";
        if (!RecursionCount.ContainsKey(name))
            return;

        RecursionCount[name] -= 1;
        if (RecursionCount[name] < 0)
            RecursionCount[name] = 0;
        if (RecursionCount[name] != 0) 
            return;
        if (!RecursionTime.TryGetValue(name, out var time))
            return;
        
        var delta = DateTime.Now - time;
        RecursionTime.Remove(name);
        Console.WriteLine($"RecursionTime: {name} [{delta.TotalMilliseconds}ms]");
    }

    /// <summary>
    /// List the currently tracked recursions
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
    /// Clear the recursion log
    /// </summary>
    /// <returns></returns>
    public static bool Clear()
        => Clear(DateTime.MinValue.AddYears(1));

    /// <summary>
    /// Clear the recursion log
    /// </summary>
    /// <param name="timeSpan">Max age to remove</param>
    /// <returns></returns>
    public static bool Clear(TimeSpan timeSpan)
        => Clear(DateTime.Now - timeSpan);

    /// <summary>
    /// Clear the recursion log
    /// </summary>
    /// <param name="dateTime">Newest log entry DateTime to remove</param>
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

}

/// <summary>
/// Exception thrown when recursion is detected
/// </summary>
public class RecursionException : Exception
{
    /// <summary>
    /// Constructor
    /// </summary>
    public RecursionException() { }

    //protected RecursionException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message"></param>
    public RecursionException(string message) : base(message) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public RecursionException(string message, Exception innerException) : base(message, innerException) { }

}
