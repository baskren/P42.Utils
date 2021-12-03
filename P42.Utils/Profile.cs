using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace P42.Utils
{
    public static class Profile
    {
        public static bool Enabled = false;

        public static Stopwatch Stopwatch;
        static Dictionary<Thread, Stack<(string key, long start, long wait)>> LastTimeStamp;

        public static void Enter([CallerMemberName] string callerName = "", string className = null)
        {
            if (Enabled)
            {
                className = className ?? DebugExtensions.NameOfCallingClass();
                var key = className + "." + callerName;
                LastTimeStamp = LastTimeStamp ?? new Dictionary<Thread, Stack<(string key, long start, long wait)>>();
                if (!(LastTimeStamp.TryGetValue(Thread.CurrentThread, out Stack<(string key, long start, long wait)> stack)))
                {
                    stack = new Stack<(string key, long start, long wait)>();
                    LastTimeStamp[Thread.CurrentThread] = stack;
                }
                Stopwatch = Stopwatch ?? Stopwatch.StartNew();
                stack.Push((key, Stopwatch.ElapsedMilliseconds, 0));
            }
        }

        public static void Exit(string mark = "EXIT", [CallerMemberName] string callerName = "", string className = null)
        {
            if (Enabled)
            {
                className = className ?? DebugExtensions.NameOfCallingClass();
                var key = className + "." + callerName;
                var stack = LastTimeStamp[Thread.CurrentThread];
                var start = stack.Pop();

                if (start.Item1 != key)
                    throw new Exception($"Unmatched keys : Exit {key} != Enter {start.key}");
                var elapsed = Stopwatch.ElapsedMilliseconds - start.start;
                System.Diagnostics.Debug.WriteLine($"{elapsed} ^ {start.wait} ^ {elapsed - start.wait} ^{key} : {mark}");

                if (stack.Any())
                {
                    var parent = stack.Pop();
                    parent.wait += elapsed;
                    stack.Push(parent);
                }
            }
        }

        public static void Mark(string mark, [CallerMemberName] string callerName = "")
        {
            if (Enabled)
            {
                var className = DebugExtensions.NameOfCallingClass();
                Exit(mark, callerName, className);
                Enter(callerName, className);
            }
        }
    }
}
