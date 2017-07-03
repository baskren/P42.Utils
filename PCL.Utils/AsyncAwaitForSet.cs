﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace PCL.Utils
{
    public class AsyncAwaitForSet<T>
    {
        public AsyncAwaitForSet(Action onStartAction, Func<T, bool> onSetAction)
        {
            _onSetAction = onSetAction;
            onStartAction?.Invoke();
        }

        #region Async Constructor / Presenter / Result
        volatile TaskCompletionSource<T> m_tcs = new TaskCompletionSource<T>();

        AsyncAwaitForSet<T> CurrentInstance;
        Func<T, bool> _onSetAction;

        public Task<T> Result()
        {
            return m_tcs.Task;
        }


        public void Set(T result)
        {
            if (_onSetAction != null && !_onSetAction.Invoke(result))
                return;
            m_tcs.TrySetResult(result);
            CurrentInstance = null;
        }

        public void Reset()
        {
            while (true)
            {
                var tcs = m_tcs;
                if (!tcs.Task.IsCompleted ||
                    Interlocked.CompareExchange(ref m_tcs, new TaskCompletionSource<T>(), tcs) == tcs)
                    return;
            }
        }
        #endregion

    }
}
