using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.Utils.Uno;
public static class FrameworkElementExtensions
{
    /// <summary>
    /// Waits for Element.IsLoaded to be true
    /// </summary>
    /// <param name="element"></param>
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
        stopWatch = null;
        throw new TimeoutException($"{nameof(WaitForLoaded)}: {element}");
    }

}
