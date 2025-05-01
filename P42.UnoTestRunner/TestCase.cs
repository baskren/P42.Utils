using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Networking;


namespace P42.UnoTestRunner;

public record TestCase
{
    public object?[] Parameters { get; init; } = Array.Empty<object>();

    public PointerDeviceType? Pointer { get; init; }

    /// <inheritdoc />
    public override string ToString()
    {
        var result = $"({Parameters.Select(p => p?.ToString() ?? "<null>").JoinBy(", ")})";

        if (Pointer is { } pt)
        {
            result += $" [{pt}]";
        }

        return result;
    }

    public async Task Invoke(TestRun run, UnitTestMethodInfo test)
    {
        run.Run++;
        run.CurrentRepeatCount = 0;

        var sw = new Stopwatch();
        var canRetry = true;
        var testClassInfo = test.TestClassInfo;
        var instance = testClassInfo.Instance;
        var testName = testClassInfo.TestClassName + '.' + test.Name;
        var fullTestName = testName + ToString();

        while (canRetry)
        {
            canRetry = false;
            var cleanupActions = new List<Func<Task>> { TestRunner.GeneralCleanupAsync };

            try
            {
                if (test.RequiresFullWindow)
                {
                    await TestServices.WindowHelper.RootElementDispatcher.RunAsync(() =>
                    {
#if __ANDROID__
						// Hide the systray!
						Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
#endif

                        TestServices.WindowHelper.UseActualWindowRoot = true;
                        TestServices.WindowHelper.SaveOriginalWindowContent();
                    });
                    cleanupActions.Add(async () =>
                    {
                        await TestServices.WindowHelper.RootElementDispatcher.RunAsync(() =>
                        {
#if __ANDROID__
                            // Restore the systray!
                            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().ExitFullScreenMode();
#endif
                            TestServices.WindowHelper.RestoreOriginalWindowContent();
                            TestServices.WindowHelper.UseActualWindowRoot = false;
                        });
                    });
                }

                await TestRunner.GeneralInitAsync();

                object? returnValue = null;
                var methodArguments = Parameters;
                /*
                if (test.PassFiltersAsFirstParameter)
                {
                    var configFilters = config.Filters ??= Array.Empty<string>();
                    methodArguments = methodArguments.ToImmutableArray().Insert(0, string.Join(";", configFilters)).ToArray();
                }
                */

                if (test.RunsOnUIThread)
                {
                    var cts = new TaskCompletionSource<bool>();

                    _ = TestServices.WindowHelper.RootElementDispatcher.RunAsync(async () =>
                    {
                        try
                        {
                            if (instance is IInjectPointers pointersInjector)
                                pointersInjector.CleanupPointers();

                            if (Pointer is { } pt)
                            {
                                var ptSubscription = (instance as IInjectPointers ?? throw new InvalidOperationException("test class does not supports pointer selection.")).SetPointer(pt);

                                cleanupActions.Add(() =>
                                {
                                    ptSubscription.Dispose();
                                    return Task.CompletedTask;
                                });
                            }

                            sw.Start();
                            var initializeReturn = testClassInfo.Initialize?.Invoke(instance, Array.Empty<object>());
                            if (initializeReturn is Task initializeReturnTask)
                                await initializeReturnTask;

                            var methodParameters = test.Method.GetParameters();
                            if (methodParameters.Length > methodArguments.Length)
                                methodArguments = ExpandArgumentsWithDefaultValues(methodArguments, methodParameters);

                            returnValue = test.Method.Invoke(instance, methodArguments);

                            sw.Stop();

                            cts.TrySetResult(true);
                        }
                        catch (Exception e)
                        {
                            cts.TrySetException(e);
                        }
                    });

                    await cts.Task;
                }
                else
                {
                    if (Pointer is { } pt)
                    {
                        var ptSubscription = (instance as IInjectPointers ?? throw new InvalidOperationException("test class does not supports pointer selection.")).SetPointer(pt);
                        cleanupActions.Add(() =>
                        {
                            ptSubscription.Dispose();
                            return Task.CompletedTask;
                        });
                    }

                    sw.Start();

                    var initializeReturn = testClassInfo.Initialize?.Invoke(instance, Array.Empty<object>());
                    if (initializeReturn is Task initializeReturnTask)
                        await initializeReturnTask;

                    returnValue = test.Method.Invoke(instance, methodArguments);
                    sw.Stop();
                }

                if (test.Method.ReturnType == typeof(Task))
                {
                    var task = (Task)returnValue!;
                    var timeout = test.Timeout;
                    var timeoutTask = Task.Delay(timeout);

                    var resultingTask = await Task.WhenAny(task, timeoutTask);

                    if (resultingTask == timeoutTask)
                    {
                        throw new TimeoutException(
                            $"Test execution timed out after {timeout}");
                    }

                    // Rethrow exception if failed OR task cancelled if task **internally** raised
                    // a TaskCancelledException (we don't provide any cancellation token).
                    await resultingTask;
                }

                var consoleText = run.ConsoleOutputRedirector.GetContentAndReset();

                if (test.ExpectedException is null)
                {
                    run.Succeeded++;
                    run.ReportTestResult(run, fullTestName, sw.Elapsed, TestResult.Passed, consoleText: consoleText);
                }
                else
                {
                    run.Failed++;
                    run.ReportTestResult(run, fullTestName, sw.Elapsed, TestResult.Failed,
                        message: $"Test did not throw the excepted exception of type {test.ExpectedException.Name}",
                        consoleText: consoleText);
                }
            }
            catch (Exception e)
            {
                sw.Stop();

                if (e is AggregateException agg)
                    e = agg.InnerExceptions.First();

                if (e is TargetInvocationException tie)
                    e = tie.InnerException!;

                var consoleText = run.ConsoleOutputRedirector.GetContentAndReset();

                if (e is AssertInconclusiveException inconclusiveException)
                {
                    run.Ignored++;
                    run.ReportTestResult(run, fullTestName, sw.Elapsed, TestResult.Skipped, message: e.Message, consoleText: consoleText);
                }
                else if (test.ExpectedException is null || !test.ExpectedException.IsInstanceOfType(e))
                {
                    if (run.CurrentRepeatCount < run.RepeatLimit - 1)
                    {
                        run.CurrentRepeatCount++;
                        canRetry = true;

                        await RunCleanup(run, instance, testClassInfo, testName, test.RunsOnUIThread);
                    }
                    else
                    {
                        run.Failed++;
                        run.ReportTestResult(run, fullTestName, sw.Elapsed, TestResult.Failed, e, consoleText: consoleText);
                    }
                }
                else
                {
                    run.Succeeded++;
                    run.ReportTestResult(run, fullTestName, sw.Elapsed, TestResult.Passed, e, consoleText: consoleText);
                }
            }
            finally
            {
                foreach (var cleanup in cleanupActions)
                {
                    await cleanup();
                }
            }
        }

    }

    private static async Task RunCleanup(TestRun run, object instance, UnitTestClassInfo testClassInfo, string testName, bool runsOnUIThread)
    {
        async Task Run()
        {
            try
            {
                await WaitResult(testClassInfo.Cleanup?.Invoke(instance, Array.Empty<object>()) ?? new object(), "cleanup", CancellationToken.None);
            }
            catch (Exception e)
            {
                run.Failed++;
                run.ReportTestResult(run, testName + " Cleanup", TimeSpan.Zero, TestResult.Failed, e, consoleText: run.ConsoleOutputRedirector.GetContentAndReset());
            }
        }

        if (runsOnUIThread)
        {
            await ExecuteOnDispatcher(Run, CancellationToken.None); // No CT for cleanup!
        }
        else
        {
            await Run();
        }
    }

    private static async ValueTask WaitResult(object returnValue, string step, CancellationToken ct)
    {
        if (returnValue is Task asyncResult)
        {
            var timeoutTask = Task.Delay(UnitTestMethodInfo.DefaultTimeout, ct);
            var resultingTask = await Task.WhenAny(asyncResult, timeoutTask);

            if (resultingTask == timeoutTask)
                throw new TimeoutException($"Test {step} timed out after {UnitTestMethodInfo.DefaultTimeout}");

            // Rethrow exception if failed OR task cancelled if task **internally** raised
            // a TaskCancelledException (we don't provide any cancellation token).
            await resultingTask;
        }
    }

    private static async ValueTask ExecuteOnDispatcher(Func<Task> asyncAction, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<bool>();
        await TestServices.WindowHelper.RootElementDispatcher.RunAsync(async () =>
        {
            try
            {
                if (ct.IsCancellationRequested)
                    tcs.TrySetCanceled();


                using var ctReg = ct.Register(() => tcs.TrySetCanceled());
                await asyncAction();

                tcs.TrySetResult(default);
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }
        });

        await tcs.Task;
    }

    private static object?[] ExpandArgumentsWithDefaultValues(object?[] methodArguments, ParameterInfo[] methodParameters)
    {
        var expandedArguments = new List<object?>(methodParameters.Length);
        for (int i = 0; i < methodArguments.Length; i++)
            expandedArguments.Add(methodArguments[i]);

        // Try to get default values for the rest
        for (int i = 0; i < methodParameters.Length - methodArguments.Length; i++)
        {
            var parameter = methodParameters[methodArguments.Length + i];
            if (!parameter.HasDefaultValue)
                throw new InvalidOperationException("Missing parameter does not have default value");
            else if (parameter.DefaultValue != null)
                expandedArguments.Add(parameter.DefaultValue);

        }

        return [.. expandedArguments];
    }


}
