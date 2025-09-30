using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace P42.Utils;

public static class Profile
{
    /// <summary>
    /// Enable profiler
    /// </summary>
    public static bool Enabled { get; set; }

    /// <summary>
    /// Stopwatch used by profiler
    /// </summary>
    public static readonly Stopwatch Stopwatch = new();
    
    private static readonly Dictionary<Thread, Stack<(string key, long start, long wait)>> LastTimeStamp = new();

    /// <summary>
    /// Enter block to be profiled
    /// </summary>
    /// <param name="callerName"></param>
    /// <param name="className"></param>
    public static void Enter([CallerMemberName] string callerName = "", string? className = null)
    {
        if (!Enabled)
            return;

        className ??= DebugExtensions.NameOfCallingClass();
        var key = $"{className}.{callerName}";
        var stack = !LastTimeStamp.TryGetValue(Thread.CurrentThread, out var value)
            ? LastTimeStamp[Thread.CurrentThread] = new Stack<(string key, long start, long wait)>()
            : value;
        stack.Push((key, Stopwatch.ElapsedMilliseconds, 0));
    }

    /// <summary>
    /// Exit block to be profiled
    /// </summary>
    /// <param name="mark"></param>
    /// <param name="callerName"></param>
    /// <param name="className"></param>
    /// <exception cref="Exception"></exception>
    public static void Exit(string mark = "EXIT", [CallerMemberName] string callerName = "", string? className = null)
    {
        if (!Enabled)
            return;

        className ??= DebugExtensions.NameOfCallingClass();
        var key = $"{className}.{callerName}";
        var stack = LastTimeStamp[Thread.CurrentThread];
        var start = stack.Pop();

        if (start.Item1 != key)
            throw new Exception($"Unmatched keys : Exit {key} != Enter {start.key}");
        var elapsed = Stopwatch.ElapsedMilliseconds - start.start;
        Debug.WriteLine($"{elapsed} ^ {start.wait} ^ {elapsed - start.wait} ^{key} : {mark}");

        if (!stack.Any())
            return;

        var parent = stack.Pop();
        parent.wait += elapsed;
        stack.Push(parent);
    }

    /// <summary>
    /// Mark intermediate profile point
    /// </summary>
    /// <param name="mark"></param>
    /// <param name="callerName"></param>
    public static void Mark(string mark, [CallerMemberName] string callerName = "")
    {
        if (!Enabled)
            return;

        var className = DebugExtensions.NameOfCallingClass();
        Exit(mark, callerName, className);
        Enter(callerName, className);
    }
}
