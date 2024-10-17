using System.Runtime.CompilerServices;
using System.Threading;
using System;
using Windows.Foundation;

namespace P42.Utils.Uno;

public static class IAsyncOperationExtensions
{
    /// <summary>
    /// Fire and Forget for AsyncOperations
    /// </summary>
    /// <param name="operation"></param>
    /// <param name="onException"></param>
    /// <param name="callingMethodName"></param>
    /// <typeparam name="T"></typeparam>
    public static void Forget<T>(this IAsyncOperation<T> operation, Action<Thread, Exception> onException = null, [CallerMemberName] string callingMethodName = "")
        => operation.AsTask().Forget(onException, callingMethodName);

}
