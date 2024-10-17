using System;
using System.Threading.Tasks;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public static class LongRunningTask
{
    public static Task Start(Action action)
        => Start(action, null);

    public static Task Start(Func<Task> action)
        => Start(action, null);

    public static Task<T> Start<T>(Func<T> action)
        => Start(action, (Func<T>?)null);

    public static Task<T> Start<T>(Func<Task<T>> action)
        => Start(action, (Func<T>?)null);



    public static Task<T> Start<T>(Func<T> func, Func<T>? cancellationAction)
        => CreateLongRunningTask() is not { } task 
            ? Task.FromResult(func.Invoke()) 
            : task.RunAsync(func, cancellationAction);

    public static Task<T> Start<T>(Func<Task<T>> func, Func<T>? cancellationAction)
        => CreateLongRunningTask() is not { } task 
            ? func.Invoke() 
            : task.RunAsync(func, cancellationAction);

    public static Task<T> Start<T>(Func<T> func, Func<Task<T>>? cancellationAction)
        => CreateLongRunningTask() is not { } task 
            ? Task.FromResult(func.Invoke()) 
            : task.RunAsync(func, cancellationAction);
    
    public static Task<T> Start<T>(Func<Task<T>> func, Func<Task<T>>? cancellationAction)
        => CreateLongRunningTask() is not { } task 
            ? func.Invoke() 
            : task.RunAsync(func, cancellationAction);

    
    

    public static Task Start(Action action, Action? cancellationAction)
    {
        if (CreateLongRunningTask() is { } task)
            return task.RunAsync(action, cancellationAction);

        action.Invoke();
        return Task.CompletedTask;
    }

    public static Task Start(Action action, Func<Task>? cancellationAction)
    {
        if (CreateLongRunningTask() is { } task)
            return task.RunAsync(action, cancellationAction);

        action.Invoke();
        return Task.CompletedTask;
    }

    public static Task Start(Func<Task> func, Action? cancellationAction)
        => CreateLongRunningTask() is not { } task 
            ? func.Invoke() 
            : task.RunAsync(func, cancellationAction);

    public static Task Start(Func<Task> func, Func<Task>? cancellationAction)
        => CreateLongRunningTask() is not { } task 
            ? func.Invoke() 
            : task.RunAsync(func, cancellationAction);
    

    private static ILongRunningTask? CreateLongRunningTask()
    {
        if (Environment.PlatformLongRunningTaskType is not { } type)
        {
            QLog.Error("PlatformLongRunningTaskType not set.  Did you initiate P42.Utils.Environment.PlatformLongRunningTaskType?");
            return null;
        }

        if (Activator.CreateInstance(type) is ILongRunningTask task)
            return task;

        QLog.Error($"Cannot instantiate type {type} as a P42.Utils.Environment.PlatformLongRunningTaskType");
        return null;

    }
    
}
