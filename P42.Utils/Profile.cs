using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace P42.Utils;

public static class Profile
{
    public static bool Enabled { get; set; }

    private static Stopwatch? _stopWatch;
    public static Stopwatch Stopwatch => _stopWatch ??= new Stopwatch();

    private static readonly Dictionary<Thread, Stack<(string key, long start, long wait)>> LastTimeStamp = new();

    /// <summary>
    /// Method to be called at entry of measured method
    /// </summary>
    /// <param name="callerName"></param>
    /// <param name="className"></param>
    public static void Enter([CallerMemberName] string callerName = "", string? className = null)
    {
        if (!Enabled)
            return;

        className ??= NameOfCallingClass();
        var key = $"{className}.{callerName}";
        if (!LastTimeStamp.TryGetValue(Thread.CurrentThread, out var stack))
        {
            stack = new Stack<(string key, long start, long wait)>();
            LastTimeStamp[Thread.CurrentThread] = stack;
        }
        stack.Push((key, Stopwatch.ElapsedMilliseconds, 0));
    }

    /// <summary>
    /// Method to be called at exit of measured method
    /// </summary>
    /// <param name="mark"></param>
    /// <param name="callerName"></param>
    /// <param name="className"></param>
    /// <exception cref="Exception"></exception>
    public static void Exit(string mark = "EXIT", [CallerMemberName] string callerName = "", string? className = null)
    {
        if (!Enabled)
            return;

        className ??= NameOfCallingClass();
        var key = $"{className}.{callerName}";
        var stack = LastTimeStamp[Thread.CurrentThread];
        var start = stack.Pop();

        if (start.Item1 != key)
            throw new Exception($"Unmatched keys : Exit {key} != Enter {start.key}");
        
        var elapsed = Stopwatch.ElapsedMilliseconds - start.start;
        Debug.WriteLine($"{elapsed} ^ {start.wait} ^ {elapsed - start.wait} ^{key} : {mark}");

        if (stack.Count == 0)
            return;

        var parent = stack.Pop();
        parent.wait += elapsed;
        stack.Push(parent);
    }

    /// <summary>
    /// Method to be called at milestone in measured method
    /// </summary>
    /// <param name="mark"></param>
    /// <param name="callerName"></param>
    public static void Mark(string mark, [CallerMemberName] string callerName = "")
    {
        if (!Enabled)
            return;

        var className = NameOfCallingClass();
        Exit(mark, callerName, className);
        Enter(callerName, className);
    }
    
    internal static string NameOfCallingClass()
    {
        string fullName;
        Type? declaringType;
        var skipFrames = 2;
        do
        {
            var stackTrace = new StackFrame(skipFrames, false);
            if (stackTrace.GetMethod() is not { } method)
                return string.Empty;
            declaringType = method.DeclaringType;
            if (declaringType == null)
                return method.Name;

            skipFrames++;
            fullName = declaringType.FullName ?? string.Empty;
        }
        while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

        return fullName;
    }
    
}
