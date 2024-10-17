using System;
using Microsoft.UI.Xaml.Media.Animation;

namespace P42.Utils.Uno;

public class ActionAnimator : NormalizedActionAnimator
{
    public double From { get; private set; }

    public double To { get; private set; }



    public ActionAnimator(double from, double to, TimeSpan timeSpan, Action<double> action, EasingFunctionBase easingFunction = null, bool delta = false)
        : base(timeSpan, action, easingFunction, delta)
    {
        From = from;
        To = to;
    }

    protected override double Value(double normalizedValue)
    {
        var value = From + normalizedValue * (To - From);
        return value;
    }
}