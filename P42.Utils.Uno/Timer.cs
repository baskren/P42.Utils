using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

internal class Timer : IPlatformTimer
{
    private class MainThreadTimer
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly Func<bool> _func;

        public MainThreadTimer(TimeSpan span, Func<bool> func)
        {
            _func = func;
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = span;
            _dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object? sender, object e)
        {
            if (!_func.Invoke())
            {
                _dispatcherTimer.Stop();
            }
        }
    }

    private class AsyncMainThreadTimer
    {
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly Func<Task<bool>> _task;

        public AsyncMainThreadTimer(TimeSpan span, Func<Task<bool>> task)
        {
            this._task = task;
            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += DispatcherTimer_Tick;
            _dispatcherTimer.Interval = span;
            _dispatcherTimer.Start();
        }

        private async void DispatcherTimer_Tick(object? sender, object e)
        {
            if (!await _task.Invoke())
                _dispatcherTimer.Stop();
                
        }
    }

    public void StartTimer(TimeSpan timeSpan, Func<bool> func)
        => _ = new MainThreadTimer(timeSpan, func);

    public void StartTimer(TimeSpan timeSpan, Func<Task<bool>> task)
        => _ = new AsyncMainThreadTimer(timeSpan, task);

}
