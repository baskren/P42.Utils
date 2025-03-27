using System;
using System.Threading.Tasks;

namespace P42.Utils;

/// <summary>
/// Platform delegates for LongRunningTask
/// </summary>
public interface ILongRunningTask
{
    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> RunAsync<T>(Func<T> func, Func<T>? cancellationAction = null);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> RunAsync<T>(Func<Task<T>> func, Func<T>? cancellationAction = null);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> RunAsync<T>(Func<T> func, Func<Task<T>> cancellationAction);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<T> RunAsync<T>(Func<Task<T>> func, Func<Task<T>> cancellationAction);


    
    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationAction"></param>
    Task RunAsync(Action action, Action? cancellationAction = null);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="action"></param>
    /// <param name="cancellationAction"></param>
    Task RunAsync(Action action, Func<Task> cancellationAction);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    Task RunAsync(Func<Task> action, Action? cancellationAction = null);

    /// <summary>
    /// Start long-running background process
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationAction"></param>
    Task RunAsync(Func<Task> action, Func<Task> cancellationAction);
}
