using System.Diagnostics;

namespace P42.Utils.Uno;
// ReSharper disable once UnusedType.Global
public static class FrameworkElementExtensions
{
    extension(FrameworkElement element)
    {
        /// <summary>
        /// Waits for Element.IsLoaded to be true
        /// </summary>
        /// <param name="timeOutMs"></param>
        /// <returns></returns>
        public async Task WaitForLoaded(long timeOutMs = 5000)
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

        /// <summary>
        /// Gets the first descendent of a FrameworkElement
        /// </summary>
        /// <returns></returns>
        public UIElement? GetFirstDescendent()
            => element.FindChildren<UIElement>().FirstOrDefault();
    }
}
