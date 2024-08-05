using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media.Animation;

namespace P42.Utils.Uno
{
    public class NormalizedActionAnimator
    {
        public TimeSpan TimeSpan { get; private set; }

        public EasingFunctionBase EasingFunction { get; private set; }

        public Action<double> Action { get; private set; }

        protected DateTime StartTime;

        double lastValue;

        public bool Delta { get; private set; }

        public NormalizedActionAnimator(TimeSpan timeSpan, Action<double> action, EasingFunctionBase easingFunction = null, bool delta = false)
        {
            Delta = delta;
            TimeSpan = timeSpan;
            EasingFunction = easingFunction;
            Action = action;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            StartTime = DateTime.Now;
            double normalTime;
            lastValue = 0;
            do
            {
                await Task.Delay(20);
                normalTime = Math.Min((DateTime.Now - StartTime).TotalMilliseconds / TimeSpan.TotalMilliseconds,1.0);
                var normalValue = EasingFunction?.Ease(normalTime) ?? normalTime;
                var value = Value(normalValue);
                if (Delta)
                {
                    this?.Action(value - lastValue);
                    lastValue = value;
                }
                else
                    this?.Action(value);
            }
            while (normalTime < 1.0 && !cancellationToken.IsCancellationRequested);
        }

        protected virtual double Value(double normalValue)
        {
            return normalValue;
        }
    }
}
