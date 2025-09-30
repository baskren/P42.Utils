using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace P42.Utils;

/// <summary>
/// Debug extensions
/// </summary>
public static class DebugExtensions
{
    
    #region Caller Info

    
    /// <summary>
    /// What's the name of the caller method?
    /// </summary>
    /// <param name="callerName">optional, method name</param>
    /// <returns>method name</returns>
    public static string CurrentCodeMethodName([CallerMemberName] string callerName = "")
        => callerName;

    /// <summary>
    /// What's the path of the file that called this method?
    /// </summary>
    /// <param name="callerPath">optional, path of file</param>
    /// <returns>file path</returns>
    public static string CurrentCodeSourceFilePath([CallerFilePath] string callerPath = "")
        => callerPath;

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
        
        return $"[{callerPath}:{lineNumber}] : [{declaringType.Assembly.Name()}.dll:{declaringType.FullName}.{method.Name}]";
    }
    
    #endregion
    
    private static DateTime LastMessageDateTime { get; set; } = DateTime.MaxValue;

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    /// <summary>
    /// Conditional test for debug output
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static Func<object, bool>? ConditionFunc { get; set; }

    /// <summary>
    /// Toggles on/off DebugExtensions messaging
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public static bool IsMessagesEnabled { get; set; } = false;

    /// <summary>
    /// Is Census (tracking) enabled
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public static bool IsCensusEnabled { get; set; } = false;

    private static readonly ConcurrentDictionary<Guid, (string message, DateTime dateTime)> OpenTracks = new();

    static DebugExtensions()
    {
        Debug.IndentSize = 4;
        PeriodicTimer.StartTimer(TimeSpan.FromSeconds(5), () =>
        {
            if (DateTime.Now - LastMessageDateTime <= TimeSpan.FromSeconds(5))
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

    /// <summary>
    /// Send a message
    /// </summary>
    /// <param name="enabled"></param>
    /// <param name="message"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    public static Guid? Message(bool enabled, string message, Guid? guid = null)
    {
        if (!IsMessagesEnabled || !enabled)
            return null;

        return Message(message, guid, NameOfCallingClassAndMethod());
    }

    /// <summary>
    /// Send a message using object to test
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="message"></param>
    /// <param name="guid"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static Guid? Message(object obj, string message, Guid? guid = null)
    {
        if (!IsMessagesEnabled || !(ConditionFunc?.Invoke(obj) ?? false))
            return null;

        return Message(message, guid, NameOfCallingClassAndMethod());
    }

    /// <summary>
    /// Send a message (optional, add: "ENTER" to start new nesting, "EXIT" to finish a nesting)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="guid">ID created when message contains "ENTER", used for indentation and census</param>
    /// <param name="callingMethod"></param>
    /// <returns></returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static Guid? Message(string message, Guid? guid = null, string callingMethod = "")
    {
        if (!IsMessagesEnabled)
            return null;

        if (string.IsNullOrWhiteSpace(callingMethod))
            callingMethod = NameOfCallingClassAndMethod();
        
        Debug.IndentSize = 4;
        if (message.Contains("ENTER"))
        {
            guid = Guid.NewGuid();
            ((IDictionary<Guid, (string message, DateTime dateTime)>)OpenTracks).Add(guid.Value, (
                $"{callingMethod}: {message}", DateTime.Now));
            //OpenTracks.GetOrAdd(guid.Value, (callingMethod + ": " + message, DateTime.Now));
            if (Debug.IndentLevel == 0)
                Debug.WriteLine("=========================================================");
            Debug.IndentLevel = OpenTracks.Count;
        }
        
        Debug.WriteLine($"{callingMethod}: {message}");
        if (message.Contains("EXIT"))
        {
            if (guid.HasValue)
                ((IDictionary<Guid, (string message, DateTime dateTime)>)OpenTracks).Remove(guid.Value);
            Debug.IndentLevel = OpenTracks.Count;
            if (Debug.IndentLevel == 0)
                Debug.WriteLine(
                    $"========================================================= OpenTracks.Count: {OpenTracks.Count}");
        }

        LastMessageDateTime = DateTime.Now;
        return guid;
    }


    private static string NameOfCallingClassAndMethod()
    {
        string fullName;
        Type declaringType;
        var skipFrames = 2;
        do
        {
            if (new StackFrame(skipFrames, false).GetMethod() is not { } method)
                return string.Empty;

            if (method.DeclaringType == null)
                return $"{method.ReflectedType}.{method.Name}";
            
            declaringType = method.DeclaringType;
            fullName = $"{declaringType.FullName}.{method.Name}";
            skipFrames++;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        return fullName;
    }
    
    // ReSharper disable once UnusedMember.Local
    internal static string NameOfCallingClass()
    {
        string fullName;
        Type declaringType;
        var skipFrames = 2;
        do
        {
            if (new StackFrame(skipFrames, false).GetMethod() is not { } method)
                return string.Empty;

            if (method.DeclaringType == null)
                return method.Name;

            skipFrames++;
            fullName = method.DeclaringType.FullName ?? string.Empty;
            declaringType = method.DeclaringType;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        return fullName;
    }


    /// <summary>
    /// Tracking object
    /// </summary>
    public static readonly ConcurrentDictionary<Type, long> Census = new();

    /// <summary>
    /// Add an object to the Census
    /// </summary>
    /// <param name="obj"></param>
    public static void AddToCensus(this object obj)
    {
        if (!IsCensusEnabled)
            return;

        var type = obj.GetType();
        if (!Census.TryAdd(type, 1))
            Census[type] += 1;
    }

    /// <summary>
    /// Remove an object from the census
    /// </summary>
    /// <param name="obj"></param>
    public static void RemoveFromCensus(this object obj)
    {
        if (!IsCensusEnabled)
            return;

        var type = obj.GetType();
        if (Census.ContainsKey(type))
            Census[type] -= 1;
        else
        {
            var msg = $"Debug.{CurrentCodeWaypoint()}:TYPE NOT FOUND!!!! [{type}]";
            Debug.WriteLine(msg);
            Console.WriteLine(msg);
        }
    }

    /// <summary>
    /// How many objects are in Census?
    /// </summary>
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
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static bool IsRequestUserHelpEnabled{ get; set; }

    /// <summary>
    /// Email address to which to send requests
    /// </summary>
    public static string RequestUserHelpEmailToAddress { get; set; } = string.Empty;

    /// <summary>
    /// Title of PermissionPopup presented to user where "YES" would send an email asking for help with bug
    /// </summary>
    public static string RequestUserHelpMissiveTitle { get; set; } = "I believe I need your help ...";

    /// <summary>
    /// Body of PermissionPopup presented to user where "YES" would send an email asking for help with bug
    /// </summary>
    public static string RequestUserHelpMissiveMessage { get; set; } = "I am the developer of this application (or at least the part the just didn't work).  Unfortunately you have managed to trigger a bug that, for the life of me, I cannot reproduce - and therefore fix!  Would you be willing to email me so I can learn more about what just happened?";

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static Func<Exception, string, string, int, string, Task>? RequestHelpDelegate { get; set; } 

    /// <summary>
    /// Used internally to communicate with user when perplexing exception is triggered;
    /// </summary>
    /// <param name="e">Exception thrown that prompted request for help</param>
    /// <param name="additionalInfo">Any additional info you would like shared with you </param>
    /// <param name="path">Path to file where this request was called</param>
    /// <param name="lineNumber">Linenumber of where this request was called</param>
    /// <param name="methodName">Name of method from which this request was called</param>
    /// <returns></returns>
    [Obsolete("Please report if using this method")]
    public static async Task RequestUserHelp(
        this Exception e,
        string additionalInfo = "",
        [CallerFilePath] string path = "",
        [CallerLineNumber] int lineNumber = -1,
        [CallerMemberName] string methodName = "")
    {
        if (IsRequestUserHelpEnabled && RequestHelpDelegate is { } d)
        {
            //Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            await d(e, additionalInfo, path, lineNumber, methodName);
        }
    }

}
