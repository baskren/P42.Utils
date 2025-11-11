using Windows.Foundation;

namespace P42.Utils.Uno;

public static class TaskExtensions
{
    public static void WatchForError(this IAsyncAction self) 
        => self.AsTask().WatchForError();

    public static void WatchForError<T>(this IAsyncOperation<T> self) 
        => self.AsTask().WatchForError();

    public static void WatchForError(this Task self)
    {
        var context = SynchronizationContext.Current;
        if (context == null)
            return;

        self.ContinueWith(
            t =>
            {
                if (t.Exception is null)
                    return;
                var exception = t.Exception.InnerExceptions.Count > 1 ? t.Exception : t.Exception.InnerException;
                context.Post(e =>
                {
                    if (e is Exception ex)
                        throw ex;
                }, exception);
            }, CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);
    }

}
