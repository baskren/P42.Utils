using System.Diagnostics;

namespace P42.Utils.Uno;
// ReSharper disable once UnusedType.Global
public static class FrameworkElementExtensions
{
    /// <summary>
    /// Waits for Element.IsLoaded to be true
    /// </summary>
    /// <param name="element"></param>
    /// <param name="timeOutMs"></param>
    /// <returns></returns>
    public static async Task WaitForLoaded(this FrameworkElement element, long timeOutMs = 5000)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        while (stopWatch.ElapsedMilliseconds < timeOutMs)
        {
            if (element.IsLoaded)
                return;
            await Task.Delay(100);
        }
        stopWatch.Stop();
        throw new TimeoutException($"{nameof(WaitForLoaded)}: {element}");
    }

}
