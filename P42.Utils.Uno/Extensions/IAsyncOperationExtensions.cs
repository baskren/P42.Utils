using System.Runtime.CompilerServices;
using System.Threading;
using System;
using Windows.Foundation;

namespace P42.Utils.Uno
{
    public static class IAsyncOperationExtensions
    {
        public static void Forget<T>(this IAsyncOperation<T> operation, Action<Thread, Exception> onException = null, [CallerMemberName] string callingMethodName = "")
            => operation.AsTask().Forget(onException, callingMethodName);

    }
}