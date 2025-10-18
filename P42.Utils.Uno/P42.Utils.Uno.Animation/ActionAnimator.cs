using Microsoft.UI.Xaml.Media.Animation;

namespace P42.Utils.Uno;

/// <summary>
/// Simple, From-To, 1D animation path generator
/// </summary>
/// <param name="from">Start value</param>
/// <param name="to">End value</param>
/// <param name="timeSpan">duration</param>
/// <param name="action">action where animation is to be applied</param>
/// <param name="easingFunction">easing function</param>
/// <param name="delta">true: pass change in easing function value to action(); false: pass current easing function value to action()</param>
// ReSharper disable once UnusedType.Global
public class ActionAnimator(
    double from,
    double to,
    TimeSpan timeSpan,
    Action<double> action,
    EasingFunctionBase? easingFunction = null,
    bool delta = false)
    : NormalizedActionAnimator(timeSpan, action, easingFunction, delta)
{
    /// <summary>
    /// Start value
    /// </summary>
    public double From { get; } = from;

    
    /// <summary>
    /// End value
    /// </summary>
    public double To { get; } = to;


    protected override double Value(double easedValue)
    {
        var value = From + easedValue * (To - From);
        return value;
    }
}
