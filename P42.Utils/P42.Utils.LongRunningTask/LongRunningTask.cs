using Microsoft.VisualBasic.CompilerServices;

namespace P42.Utils;

/// <summary>
/// Long Running Background Task
/// </summary>
// ReSharper disable once UnusedType.Global
public static class LongRunningTask
{

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static Type? PlatformLongRunningTaskType { get; set; }
    
    private static ILongRunningTask Create()
    {
        if (PlatformLongRunningTaskType is not { } type)
            throw new IncompleteInitialization();
        return (ILongRunningTask)(Activator.CreateInstance(type) ?? throw new InvalidOperationException());
    }

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> Start<T>(Func<T> func, Func<T>? cancellationAction = null)
        => await Create().RunAsync(func, cancellationAction);
    
    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> Start<T>(Func<Task<T>> func, Func<T>? cancellationAction = null)
        => await Create().RunAsync(func, cancellationAction);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> Start<T>(Func<T> func, Func<Task<T>> cancellationAction)
        => await Create().RunAsync(func, cancellationAction);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T> Start<T>(Func<Task<T>> func, Func<Task<T>> cancellationAction)
    => await Create().RunAsync(func, cancellationAction);
    
    
    
    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationAction"></param>
    public static async Task Start(Action action, Action? cancellationAction = null)
        => await Create().RunAsync(action, cancellationAction);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationAction"></param>
    public static async Task Start(Action action, Func<Task> cancellationAction)
    => await Create().RunAsync(action, cancellationAction);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    public static async Task Start(Func<Task> func, Action? cancellationAction = null)
       => await Create().RunAsync(func, cancellationAction);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    public static async Task Start(Func<Task> func, Func<Task> cancellationAction)
        => await Create().RunAsync(func, cancellationAction);

}
