using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.UnoTestRunner;

public partial class TestServices
{
    public static class WindowHelper
    {
        //public static XamlRoot? XamlRoot { get; set; }

        // public static bool IsXamlIsland { get; set; }

        //public static Microsoft.UI.Xaml.Window CurrentTestWindow { get; set; }
        public static Window CurrentTestWindow 
            => UnitTestsUIContentHelper.CurrentTestWindow;

        //public static (UIElement control, Func<UIElement> getContent, Action<UIElement> setContent) EmbeddedTestRoot { get; set; }
        public static (UIElement control, Func<UIElement?> getContent, Action<UIElement?> setContent) EmbeddedTestRoot 
            => UnitTestsUIContentHelper.EmbeddedTestRoot;

        //public static bool UseActualWindowRoot { get; set; }
        public static bool UseActualWindowRoot
        {  
            get => UnitTestsUIContentHelper.UseActualWindowRoot; 
            set => UnitTestsUIContentHelper.UseActualWindowRoot = value; 
        }
            

        /*
        public static UIElement WindowContent
        {
            get => UseActualWindowRoot
                ? (IsXamlIsland 
                    ? (GetXamlIslandRootContentControl()!.Content! as UIElement)! 
                    : CurrentTestWindow.Content!)
                : EmbeddedTestRoot.getContent!.Invoke();
            internal set
            {
                if (UseActualWindowRoot)
                {
                    if (IsXamlIsland)
                        GetXamlIslandRootContentControl()!.Content = value;
                    else
                        CurrentTestWindow.Content = value;
                }
                else if (EmbeddedTestRoot.setContent is { } setter)
                {
                    setter(value);
                }
                else
                {
                    Console.WriteLine("Failed to get test content control");
                }
            }
        }
        */

        #region Content Swapping
        //private static UIElement? _originalWindowContent;

        public static void SaveOriginalWindowContent() 
            => UnitTestsUIContentHelper.SaveOriginalContent();
            /*
        {
            _originalWindowContent = IsXamlIsland
                ? GetXamlIslandRootContentControl()?.Content as UIElement
                : CurrentTestWindow.Content;
            
        }
            */

        public static void RestoreOriginalWindowContent()
            => UnitTestsUIContentHelper.RestoreOriginalContent();
            /*
        {
            if (_originalWindowContent != null)
            {
                if (IsXamlIsland)
                {
                    if (GetXamlIslandRootContentControl() is ContentControl islandContentControl)
                        islandContentControl.Content = _originalWindowContent;
                }
                else
                    CurrentTestWindow.Content = _originalWindowContent;

                _originalWindowContent = null;
            }
        }

        private static ContentControl? GetXamlIslandRootContentControl()
        {
            var islandContentRoot = EmbeddedTestRoot.control.XamlRoot!.Content!;
            if (islandContentRoot is not ContentControl contentControl)
                contentControl = (VisualTreeUtils.FindVisualChildByType<ContentControl>(islandContentRoot))!;

            return contentControl;
        }
        */
        #endregion


        /*
        public static UIElement? RootElement => 
            UseActualWindowRoot 
            ? XamlRoot?.Content 
            : EmbeddedTestRoot.control;
        */

        // Dispatcher is a separate property, as accessing CurrentTestWindow.COntent when
        // not on the UI thread will throw an exception in WinUI.
        public static UnitTestDispatcherCompat RootElementDispatcher
            => UnitTestsUIContentHelper.RootElementDispatcher;
        /*
        => UseActualWindowRoot
            ? (CurrentTestWindow is { } 
                ? UnitTestDispatcherCompat.From(CurrentTestWindow) 
                : UnitTestDispatcherCompat.Instance)
            : UnitTestDispatcherCompat.From(EmbeddedTestRoot.control);
        */


        /*
        internal static Page SetupSimulatedAppPage()
        {
            var spFrame = new Frame();
            var spRootFrameAsCC = spFrame as ContentControl;
            var spRootFrameAsUI = spRootFrameAsCC as UIElement;
            WindowContent = spRootFrameAsUI;

            spFrame.Navigate(typeof(Page));
            return (spRootFrameAsCC.Content as Page)!;
        }

        internal static async Task WaitForIdle()
        {
            await RootElementDispatcher.RunIdleAsync(_ => {  }); // Empty to wait for the idle queue to be reached 
            await RootElementDispatcher.RunIdleAsync(_ => {  }); // Empty to wait for the idle queue to be reached 
        }

        /// <summary>
        /// Waits for <paramref name="element"/> to be loaded and measured in the visual tree.
        /// </summary>
        /// <remarks>
        /// On UWP, <see cref="WaitForIdle"/> may not always wait long enough for the control to be properly measured.
        ///
        /// This method assumes that the control will have a non-zero size once loaded, so it's not appropriate for elements that are
        /// collapsed, empty, etc.
        /// </remarks>
        internal static async Task WaitForLoaded(FrameworkElement element, Func<FrameworkElement, bool>? isLoaded = null, int timeoutMS = 1000)
        {
            async Task Do()
            {
                bool IsLoaded()
                {
                    if (element.ActualHeight == 0 || element.ActualWidth == 0)
                        return false;

                    if (element is Control control && control.FindVisualChildByType<FrameworkElement>(includeCurrent: false) == null)
                        return false;

                    if (element is ListView listView && listView.Items.Count > 0 && listView.ContainerFromIndex(0) == null)
                        // If it's a ListView, wait for items to be populated
                        return false;

                    return true;
                }

                await WaitFor(
                    isLoaded is { } ? () => isLoaded(element) : IsLoaded,
                    message: $"Timeout waiting on {element} to be loaded",
                    timeoutMS: timeoutMS);
            }
#if __WASM__   // Adjust for re-layout failures in When_Inline_Items_SelectedIndex, When_Observable_ItemsSource_And_Added, When_Presenter_Doesnt_Take_Up_All_Space
				await Do();
#else
            var dispatcher = UnitTestDispatcherCompat.From(element);

            if (dispatcher.HasThreadAccess)
                await Do();
            else
            {
                TaskCompletionSource<bool> tcs = new();

                _ = dispatcher.RunAsync(() =>
                {
                    try
                    {

                        tcs.TrySetResult(true);
                    }
                    catch (Exception e)
                    {
                        tcs.TrySetException(e);
                    }
                });

                await tcs.Task;
            }
#endif
        }

        /*
        internal static async Task WaitForRelayouted(FrameworkElement frameworkElement)
        {
            var isRelayouted = false;

            void OnLayoutUpdated(object? _, object __)
            {
                frameworkElement.LayoutUpdated -= OnLayoutUpdated;
                isRelayouted = true;
            }

            frameworkElement.LayoutUpdated += OnLayoutUpdated;

            await WaitFor(() => isRelayouted, message: $"{frameworkElement} re-layouted");
        }

        internal static async Task WaitForEqual(double expected, Func<double> actualFunc, double tolerance = 1.0, int timeoutMS = 1000)
        {
            var actual = actualFunc();
            if (ApproxEquals(actual))
                return;

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMS)
            {
                await WaitForIdle();
                actual = actualFunc();
                if (ApproxEquals(actual))
                    return;

            }

            throw new AssertFailedException($"Timed out waiting for equality condition to be met. Expected {expected} but last received value was {actual}.");

            bool ApproxEquals(double actualValue) => Math.Abs(expected - actualValue) < tolerance;
        }

        internal static async Task WaitForResultEqual<T>(T expected, Func<T> actualFunc, int timeoutMS = 1000)
        {
            var actual = actualFunc();
            if (Equals(expected, actual))
                return;

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMS)
            {
                await WaitForIdle();
                actual = actualFunc();
                if (Equals(expected, actual))
                    return;

            }

            throw new AssertFailedException($"Timed out waiting for equality condition to be met. Expected {expected} but last received value was {actual}.");
        }

        /// <summary>
        /// Wait until a specified <paramref name="condition"/> is met. 
        /// </summary>
        /// <param name="timeoutMS">The maximum time to wait before failing the test, in milliseconds.</param>
        internal static async Task WaitFor(Func<bool> condition, int timeoutMS = 1000, string message = "", [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNumber = 0)
        {
            if (condition())
                return;

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMS)
            {
                await WaitForIdle();
                if (condition())
                    return;

            }

            message ??= $"{callerMemberName}():{lineNumber}";

            throw new AssertFailedException("Timed out waiting for condition to be met. " + message);
        }

        internal static async Task WaitFor<T>(
            Func<T> condition,
            T expected,
            Func<T, string>? messageBuilder = null,
            Func<T, T, bool>? comparer = null,
            int timeoutMS = 1000,
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            comparer ??= (v1, v2) => Equals(v1, v2);

            T value = condition();
            if (comparer(value, expected))
                return;

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMS)
            {
                await WaitForIdle();
                value = condition();
                if (comparer(value, expected))
                    return;

            }

            var customMsg = messageBuilder != null ? messageBuilder(value) : $"Got {value}, expected {expected}";
            var message = $"{callerMemberName}():{lineNumber} {customMsg}";

            throw new AssertFailedException("Timed out waiting for condition to be met. " + message);
        }

        internal static async Task<T> WaitForNonNull<T>(
            Func<T> getT,
            int timeoutMS = 1000,
            string message = "",
            [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int lineNumber = 0)
            where T : class
        {

            if (getT() is { } t)
                return t;

            var stopwatch = Stopwatch.StartNew();
            while (stopwatch.ElapsedMilliseconds < timeoutMS)
            {
                await WaitForIdle();

                if (getT() is { } t2)
                    return t2;

            }

            message ??= $"{callerMemberName}():{lineNumber} Never received non-null value";

            throw new AssertFailedException("Timed out waiting for condition to be met. " + message);
        }

#if DEBUG
        /// <summary>
        /// This will wait. Forever. Useful when debugging a runtime test if you wish to visually inspect or interact with a view added
        /// by the test. (To break out of the loop, just set 'shouldWait = false' via the Immediate Window.)
        /// </summary>
        internal static async Task WaitForever()
        {
            var shouldWait = true;
            while (shouldWait)
            {
                await Task.Delay(1000);
            }
        }
#endif

        internal static void ShutdownXaml() { }
        internal static void VerifyTestCleanup() { }

        internal static void SetWindowSizeOverride(object p) { }

        private static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5);

        public static async Task SynchronouslyTickUIThread(int ticks)
        {
            var tickCompleteEvent = new Microsoft.UI.Xaml.Tests.Enterprise.Event();

            for (var i = 0; i < ticks; i++)
            {
                await RunOnUIThread(() =>
                {
                    CompositionTarget.Rendering += OnCompositionTargetOnRendering;
                });

                void OnCompositionTargetOnRendering(object? sender, object o)
                {
                    CompositionTarget.Rendering -= OnCompositionTargetOnRendering;
                    tickCompleteEvent.Set();
                }

                await tickCompleteEvent.WaitFor(FiveMinutes);
            }
        }
        */


    }
}
