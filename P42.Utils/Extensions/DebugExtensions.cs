using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class DebugExtensions
{
    
    #region Caller Info
    
    /// <summary>
    /// What's the name of the caller method?
    /// </summary>
    /// <param name="callerName">optional, method name</param>
    /// <returns>method name</returns>
    public static string CurrentCodeMethodName([CallerMemberName] string? callerName = null)
        => callerName ?? string.Empty;

    /// <summary>
    /// What's the path of the file that called this method?
    /// </summary>
    /// <param name="callerPath">optional, path of file</param>
    /// <returns>file path</returns>
    public static string CurrentCodeSourceFilePath([CallerFilePath] string? callerPath = null)
        => callerPath ?? string.Empty;

    /// <summary>
    /// What's the line number where this method was called?
    /// </summary>
    /// <param name="lineNumber">optional, line number</param>
    /// <returns>line number</returns>
    public static int CurrentCodeSourceFileLineNumber([CallerLineNumber] int lineNumber = -1)
        => lineNumber;
        
    /// <summary>
    /// What's the assembly in which this method was called?
    /// </summary>
    /// <returns>calling assembly</returns>
    public static Assembly? CurrentCodeAssembly()
    {
        var stackTrace = new StackTrace();
        return stackTrace.GetFrame(2)?.GetMethod()?.DeclaringType?.Assembly;
    }

    /// <summary>
    /// Pretty info on the current method 
    /// </summary>
    /// <param name="callerPath">optional, file path of caller</param>
    /// <param name="lineNumber">optional, line number of caller</param>
    /// <returns>[file path][line number] : [assembly]:[type.method]</returns>
    public static string CurrentCodeWaypoint([CallerFilePath] string? callerPath = null, [CallerLineNumber] int lineNumber = -1)
        => CodeWaypointAtDepth(2, callerPath, lineNumber);
    
    /// <summary>
    /// Pretty info on the parent method of this method
    /// </summary>
    /// <param name="callerPath"></param>
    /// <param name="lineNumber"></param>
    /// <returns></returns>
    public static string ParentCodeWaypoint([CallerFilePath] string? callerPath = null, [CallerLineNumber] int lineNumber = -1)
        => CodeWaypointAtDepth(3, callerPath, lineNumber);
    
    /// <summary>
    /// Pretty info on who called this method
    /// </summary>
    /// <param name="depth">stack depth</param>
    /// <param name="callerPath">optional, file path of caller</param>
    /// <param name="lineNumber">optional, line number of caller</param>
    /// <returns>[file path][line number] : [assembly]:[type.method]</returns>
    public static string CodeWaypointAtDepth(int depth, [CallerFilePath] string? callerPath = null, [CallerLineNumber] int lineNumber = -1)
    {
        if (new StackTrace().GetFrame(depth) is not { } frame 
            || frame.GetMethod() is not { DeclaringType: {} declaringType } method )
            return string.Empty;
        
        return $"[{callerPath}:{lineNumber}] : [{declaringType.Assembly.GetName().Name}.dll:{declaringType.FullName}.{method.Name}]";
    }
    
    #endregion
    
    
    private static DateTime _lastMessageDateTime = DateTime.MaxValue;

    public static Func<object, bool>? ConditionFunc { get; set; }

    public static bool IsMessagesEnabled { get; set; }

    public static bool IsCensusEnabled { get; set; }

    private static readonly ConcurrentDictionary<Guid, (string message, DateTime dateTime)> OpenTracks = new();

    static DebugExtensions()
    {
        Debug.IndentSize = 4;
        Timer.StartTimer(TimeSpan.FromSeconds(5), () =>
        {
            if (DateTime.Now - _lastMessageDateTime <= TimeSpan.FromSeconds(5))
                return true;

            Debug.WriteLine("");
            Debug.WriteLine($" ~~~~~~~~~~~~~ OPEN TRACKS : {OpenTracks.Count} ~~~~~~~~~~~~~");
            var openTracks = OpenTracks.Values.ToList();
            openTracks = openTracks.OrderBy(pair => pair.dateTime).ToList();
            foreach (var p in openTracks)
                Debug.WriteLine($"\t [{p.dateTime:HH':'mm':'ss'.'fffffffK}]{p.message}");
            Debug.WriteLine(" ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Debug.WriteLine("");
            return true;
        });
    }

    public static Guid? Message(bool enabled, string? message, Guid? guid = null)
    {
        if (!IsMessagesEnabled || !enabled)
            return null;

        var method = new StackTrace().GetFrame(1)?.GetMethod();
        return Message(message, guid, method);
    }


    public static Guid? Message(object obj, string? message, Guid? guid = null)
    {
        if (!IsMessagesEnabled || !(ConditionFunc?.Invoke(obj) ?? false))
            return null;

        var method = new StackTrace().GetFrame(1)?.GetMethod();
        return Message(message, guid, method);
    }

    public static Guid? Message(string? message, Guid? guid = null, MethodBase? method = null)
    {
        if (!IsMessagesEnabled)
            return null;

        method ??= new StackTrace().GetFrame(1)?.GetMethod();
        var callingMethod = $"{method?.ReflectedType}.{method?.Name}";

        Debug.IndentSize = 4;
        if (message?.Contains("ENTER") ?? false)
        {
            guid = Guid.NewGuid();
            ((IDictionary<Guid, (string message, DateTime dateTime)>)OpenTracks).Add(guid.Value, (callingMethod + ": " + message, DateTime.Now));
            if (Debug.IndentLevel == 0)
                Debug.WriteLine("=========================================================");
            Debug.IndentLevel = OpenTracks.Count;
        }
        Debug.WriteLine($"{callingMethod}: {message}");
        if (message?.Contains("EXIT") ?? false)
        {
            if (guid.HasValue)
                ((IDictionary<Guid, (string message, DateTime dateTime)>)OpenTracks).Remove(guid.Value);
            Debug.IndentLevel = OpenTracks.Count;
            if (Debug.IndentLevel == 0)
                Debug.WriteLine(
                    $"========================================================= OpenTracks.Count: {OpenTracks.Count}");
        }
        _lastMessageDateTime = DateTime.Now;
        //});
        return guid;
    }




    public static readonly ConcurrentDictionary<Type, long> Census = new ();

    public static void AddToCensus(this object obj)
    {
        if (!IsCensusEnabled)
            return;

        var type = obj.GetType();
        if (Census.TryGetValue(type, out var count))
            Census[type] = count + 1;
        else
            Census[type] = 1;
    }

    public static void RemoveFromCensus(this object obj)
    {
        if (!IsCensusEnabled)
            return;

        var type = obj.GetType();
        if (Census.TryGetValue(type, out var count))
            Census[type] = count - 1;
        else
            QLog.Error($"Debug.{CurrentCodeWaypoint()}:TYPE NOT FOUND!!!! [{type}]");
    }

    public static long CensusActiveCount
    {
        get
        {
            var actives = Census.Values.ToArray();
            return actives.Sum();
        }
    }

    /// <summary>
    /// Enable ability to email user for help when unusual crash is encountered
    /// </summary>
    public static bool IsRequestUserHelpEnabled;

    /// <summary>
    /// Email address to which to send requests
    /// </summary>
    public static string? RequestUserHelpEmailToAddress { get; set; }

    /// <summary>
    /// Title of PermissionPopup presented to user where "YES" would send an email asking for help with bug
    /// </summary>
    public static string RequestUserHelpMissiveTitle { get; set; } = "I believe I need your help ...";

    /// <summary>
    /// Body of PermissionPopup presented to user where "YES" would send an email asking for help with bug
    /// </summary>
    public static string RequestUserHelpMissiveMessage { get; set; } = "I am the developer of this application (or at least the part the just didn't work).  Unfortunately you have managed to trigger a bug that, for the life of me, I cannot reproduce - and therefore fix!  Would you be willing to email me so I can learn more about what just happened?";

    public static Func<Exception, string?, string?, int, string?, Task>? RequestHelpDelegate { get; set; }

    /// <summary>
    /// Used internally to communicate with user when perplexing exception is triggered;
    /// </summary>
    /// <param name="e">Exception thrown that prompted request for help</param>
    /// <param name="additionalInfo">Any additional info you would like shared with you </param>
    /// <param name="path">Path to file where this request was called</param>
    /// <param name="lineNumber">Linenumber of where this request was called</param>
    /// <param name="methodName">Name of method from which this request was called</param>
    /// <returns></returns>
    public static Task RequestUserHelp(
        this Exception e,
        string? additionalInfo = null,
        [CallerFilePath] string? path = null,
        [CallerLineNumber] int lineNumber = -1,
        [CallerMemberName] string? methodName = null)
    {
        if (IsRequestUserHelpEnabled && RequestHelpDelegate is { } d)
            return d(e, additionalInfo, path, lineNumber, methodName);
        
        return Task.CompletedTask;
    }

}
