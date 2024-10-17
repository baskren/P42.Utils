using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;

namespace P42.Utils.Uno;

/// <summary>
/// Pass exception back to calling thread when exception found 
/// </summary>
public static class MainThreadExtensions
{
    internal static void WatchForError(this IAsyncAction self) 
        => self.AsTask().WatchForError();

    internal static void WatchForError<T>(this IAsyncOperation<T> self) 
        => self.AsTask().WatchForError();

    internal static void WatchForError(this Task self)
    {
        if (SynchronizationContext.Current is not { } context)
            return;

        self.ContinueWith(t =>
            {
                if (t.Exception is null)
                    return;
                
                var exception = t.Exception.InnerExceptions.Count > 1 ? t.Exception : t.Exception.InnerException;
                context.Post(e => throw ((Exception)e!), exception);
            }, 
            CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }

}
