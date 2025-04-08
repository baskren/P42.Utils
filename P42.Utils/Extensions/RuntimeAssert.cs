using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils
{
    public static class RuntimeAssert
    {

        public static void TestConstruction<T>(this Func<T> func) where T : new()
        {
            if (!(func.Invoke() is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            //var constructors = typeof(T).GetConstructors();
            //var constructor = typeof(T).GetConstructor(Type.EmptyTypes);  // this doesn't work for an enum, even thought Activator.CreateInstance does!
            //if (constructor == null)
            //    QLog.Error($"no constructor for type [{typeof(T)}] that takes no arguments");

            if (!(Activator.CreateInstance<T>() is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg, T>(this Func<TArg, T> func, TArg arg)
        {
            if (!(func.Invoke(arg) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArgs1, T>(this Func<TArg0, TArgs1, T> func, TArg0 arg0, TArgs1 arg1) 
        {
            if (!(func.Invoke(arg0, arg1) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] {arg0, arg1}) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArg1, TArg2, T>(this Func<TArg0, TArg1, TArg2, T> func, TArg0 arg0, TArg1 arg1, TArg2 arg2) 
        {
            if (!(func.Invoke(arg0, arg1, arg2) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg0, arg1, arg2 }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArg1, TArg2, TArg3, T>(this Func<TArg0, TArg1, TArg2, TArg3, T> func, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3) 
        {
            if (!(func.Invoke(arg0, arg1, arg2, arg3) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg0, arg1, arg2, arg3 }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArg1, TArg2, TArg3, TArg4, T>(this Func<TArg0, TArg1, TArg2, TArg3, TArg4, T> func, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4) 
        {
            if (!(func.Invoke(arg0, arg1, arg2, arg3, arg4) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg0, arg1, arg2, arg3, arg4 }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, T>(this Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, T> func, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5) 
        {
            if (!(func.Invoke(arg0, arg1, arg2, arg3, arg4, arg5) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg0, arg1, arg2, arg3, arg4, arg5 }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, T>(this Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, T> func, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6) 
        {
            if (!(func.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, T>(this Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, T> func, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7) 
        {
            if (!(func.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }

        public static void TestConstruction<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, T>(this Func<TArg0, TArg1, TArg2, TArg3, TArg4, TArg5, TArg6, TArg7, TArg8, T> func, TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3, TArg4 arg4, TArg5 arg5, TArg6 arg6, TArg7 arg7, TArg8 arg8) 
        {
            if (!(func.Invoke(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) is T))
                QLog.Error($"ConstructorFailed: {func.ToString()}");

            if (!(Activator.CreateInstance(typeof(T), new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }) is T))
                QLog.Error($"ActivatorFailed: {typeof(T)}");
        }


    }
}
