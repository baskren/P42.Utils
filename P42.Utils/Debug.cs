using System;
using System.Diagnostics;
using System.Reflection;

namespace P42.Utils
{
    public static class Debug
    {
        static Debug()
        {
            System.Diagnostics.Debug.IndentSize = 4;
        }

        public static Func<object, bool> ConditionFunc;

        public static void Message(object obj, string message, [System.Runtime.CompilerServices.CallerMemberName] string callingMethod = null)
        {
            if (ConditionFunc?.Invoke(obj) ?? false)
            {
                var callingClass = NameOfCallingClass();

                System.Diagnostics.Debug.IndentSize = 4;
                if (message?.Contains("ENTER") ?? false)
                {
                    if (System.Diagnostics.Debug.IndentLevel == 0)
                        System.Diagnostics.Debug.WriteLine("=========================================================");
                }
                if (message?.Contains("EXIT") ?? false)
                {
                    System.Diagnostics.Debug.Unindent();
                }
                System.Diagnostics.Debug.WriteLine(callingClass + "." + callingMethod + ": " + message);
                if (message?.Contains("ENTER") ?? false)
                    System.Diagnostics.Debug.Indent();
                if (message?.Contains("EXIT") ?? false)
                {
                    if (System.Diagnostics.Debug.IndentLevel == 0)
                        System.Diagnostics.Debug.WriteLine("=========================================================");
                }
            }
        }

        static string NameOfCallingClass()
        {
            string fullName;
            Type declaringType;
            int skipFrames = 2;
            do
            {
                MethodBase method = new StackFrame(skipFrames, false).GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    return method.Name;
                }
                skipFrames++;
                fullName = declaringType.FullName;
            }
            while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return fullName;
        }
    }
}
