using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;

namespace P42.UnoTestRunner;

internal class TestRun : INotifyPropertyChanged
{
    int _run;
    public int Run 
    { 
        get => _run; 
        set => SetValue(ref _run, value); 
    }

    int _ignored;
    public int Ignored 
    { 
        get => _ignored; 
        set => SetValue(ref _ignored, value); 
    }

    int _succeeded;
    public int Succeeded 
    { 
        get => _succeeded; 
        set => SetValue(ref _succeeded, value); 
    }

    int _failed;
    public int Failed 
    { 
        get => _failed; 
        set => SetValue(ref _failed, value); 
    }

    int _currentRepeatCount;
    public int CurrentRepeatCount 
    { 
        get => _currentRepeatCount; 
        set => SetValue(ref _currentRepeatCount, value); 
    }

    public int RepeatLimit { get; } = 3;

    public readonly List<TestCaseResult> TestResults = [];

    public event PropertyChangedEventHandler? PropertyChanged;

    public void ReportTestResult(TestRun run, string testName, TimeSpan duration, TestResult testResult, Exception? error = null, string? message = null, string? consoleText = null)

    {
        TestResults.Add(
            new TestCaseResult
            {
                TestName = testName,
                Duration = duration,
                TestResult = testResult,
                Message = error?.ToString() ?? message
            });

        var retriesText = run.CurrentRepeatCount != 0 ? $" (Retried {run.CurrentRepeatCount} time(s))" : "";
        System.Diagnostics.Debug.WriteLine(GetTestResultIcon(testResult) + ' ' + testName + retriesText);

        if (!string.IsNullOrWhiteSpace(message))
            System.Diagnostics.Debug.WriteLine("  ..." + message);

        if (error is { })
        {
            var isFailed = testResult == TestResult.Failed || testResult == TestResult.Error;
            System.Diagnostics.Debug.WriteLine("EXCEPTION> " + error.Message);

            if (isFailed)
                System.Diagnostics.Debug.WriteLine($"\t{testResult}: {testName} [{error.GetType()}] \n {error}\n\n");

        }

        if (!string.IsNullOrEmpty(consoleText))
        {
            System.Diagnostics.Debug.WriteLine($"OUT> {consoleText}");
        }

    }

    private string GetTestResultIcon(TestResult testResult)
    {
        switch (testResult)
        {
            default:
            case TestResult.Error:
            case TestResult.Failed:
                return "❌ (F)";

            case TestResult.Skipped:
                return "🚫 (I)";

            case TestResult.Passed:
                return "✔️ (S)";
        }
    }

    private Color GetTestResultColor(TestResult testResult)
    {
        switch (testResult)
        {
            case TestResult.Error:
            case TestResult.Failed:
            default:
                return Color.Red;

            case TestResult.Skipped:
                return Color.Orange;

            case TestResult.Passed:
                return Color.LightGreen;
        }
    }

    private bool SetValue<T>(ref T backer, T value, [CallerMemberName] string propertyName = "")
    {
        if (backer is null && value is null)
            return false;
        if (backer is null || value is null || !backer.Equals(value))
        {
            backer = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        return false;
    }


}
