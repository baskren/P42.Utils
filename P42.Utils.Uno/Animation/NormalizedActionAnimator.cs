using Microsoft.UI.Xaml.Media.Animation;

namespace P42.Utils.Uno;

/// <summary>
/// Simple, Normalized, 1D animation path generator
/// </summary>
/// <param name="timeSpan">Duration</param>
/// <param name="action">action(x), called to apply animation</param>
/// <param name="easingFunction">x = f(t), applied to animation</param>
/// <param name="delta">true: pass change in easing function value to action(); false: pass current easing function value to action()</param>
public class NormalizedActionAnimator(
    TimeSpan timeSpan,
    Action<double> action,
    EasingFunctionBase? easingFunction = null,
    bool delta = false)
{
    /// <summary>
    /// How long should the animation last?
    /// </summary>
    public TimeSpan TimeSpan { get; } = timeSpan;

    /// <summary>
    /// What form should the animation path take
    /// </summary>
    public EasingFunctionBase? EasingFunction { get; } = easingFunction;

    /// <summary>
    /// Where the animation is applied
    /// </summary>
    public Action<double> Action { get; } = action;

    /// <summary>
    /// Time stamp of animation start
    /// </summary>
    protected DateTime StartTime;

    /// <summary>
    /// true: pass change in easing function value to action(); false: pass current easing function value to action()
    /// </summary>
    public bool Delta { get; } = delta;

    /// <summary>
    /// easing function value in previous iteration
    /// </summary>
    private double _lastValue;


    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        StartTime = DateTime.Now;
        double normalTime;
        _lastValue = 0;
        do
        {
            await Task.Delay(20, cancellationToken);
            normalTime = Math.Min((DateTime.Now - StartTime).TotalMilliseconds / TimeSpan.TotalMilliseconds,1.0);
            var easedValue = EasingFunction?.Ease(normalTime) ?? normalTime;
            var value = Value(easedValue);
            if (Delta)
            {
                Action(value - _lastValue);
                _lastValue = value;
            }
            else
                Action(value);
        }
        while (normalTime < 1.0 && !cancellationToken.IsCancellationRequested);
    }

    protected virtual double Value(double easedValue)
    {
        return easedValue;
    }
}
