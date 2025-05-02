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
using System.Collections.ObjectModel;

namespace P42.UnoTestRunner;

public class TestRun : INotifyPropertyChanged
{
    #region Properties
    TestRunState _state = TestRunState.Pending;
    public TestRunState State 
    { 
        get => _state;
        internal set => SetValue(ref _state, value); 
    }

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

    /*
    ConsoleOutputRedirector? _redirector;
    internal ConsoleOutputRedirector ConsoleOutputRedirector
    {
        get => _redirector ?? throw new Exception($"ConsoleRedirector not properly initilaized");
        set => _redirector = value;
    }

    void ConsoleRedirectorStart()
    {
        if (_redirector != null)
            throw new Exception("ConsoleRedirectorStop() not called");
        ConsoleOutputRedirector = new ConsoleOutputRedirector();
        ConsoleOutputRedirector.Start();
    }

    void ConsoleRedirectorStop()
    {
        ConsoleOutputRedirector?.Stop();
        ConsoleOutputRedirector?.Dispose();
        _redirector = null;
    }
    */


    public StringBuilder ResultLogBuilder = new();
    public string ResultLog => ResultLogBuilder.ToString();

    CancellationToken? _ct;
    public CancellationToken CancellationToken 
    {
        get => _ct ??= new CancellationToken();
        set
        {
            if (_ct is not null)
                throw new InvalidOperationException($"Cannot reset cancellation token after test start");
            _ct = value;
        }
    }
    #endregion


    #region Fields
    public readonly ObservableCollection<TestCaseResult> TestResults = [];
    #endregion


    #region Events
    public event PropertyChangedEventHandler? PropertyChanged;
    #endregion



    #region Public Methods

    void InitiateTests()
    {
        TestApplication.Instance.ConsoleOutputRedirector.Reset();
        State = TestRunState.Running;
        //ConsoleRedirectorStart();
    }

    void TerminateTests()
    {
        //ConsoleRedirectorStop();
        State = TestRunState.Completed;
    }

    public async Task ExecuteAsync()
    {
        InitiateTests();
        await TestRunner.ExecuteTestsAsync(this);
        TerminateTests();
    }

    public async Task ExecuteAsync(IEnumerable<UnitTestMethodInfo> testMethods)
    {
        InitiateTests();
        await TestRunner.ExecuteRequestedTestsAsync(testMethods, this);
        TerminateTests();
    }

    public async Task StopAsync()
    {
        while (State == TestRunState.Running)
        {
            await Task.Delay(500);
        }
    }

    internal void ReportTestResult(TestRun run, string testName, TimeSpan duration, TestResult testResult, Exception? error = null, string? message = null, string? consoleText = null)

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
        var line = GetTestResultIcon(testResult) + ' ' + testName + retriesText;
        System.Diagnostics.Debug.WriteLine(line);
        ResultLogBuilder.AppendLine(line);

        if (!string.IsNullOrWhiteSpace(message))
        { 
            line = " ... " + message;
            System.Diagnostics.Debug.WriteLine(line);
            ResultLogBuilder.AppendLine(line);
        }

        if (error is { })
        {
            var isFailed = testResult == TestResult.Failed || testResult == TestResult.Error;
            line = "EXECPTION> " + error.Message;
            System.Diagnostics.Debug.WriteLine(line);
            ResultLogBuilder.AppendLine(line);

            if (isFailed)
            {
                line = $"\t{testResult}: {testName} [{error.GetType()}] \n {error}\n\n";
                System.Diagnostics.Debug.WriteLine(line);
                ResultLogBuilder.AppendLine(line);
            }

        }

        if (!string.IsNullOrEmpty(consoleText))
        {
            line = $"OUT> {consoleText}";
            System.Diagnostics.Debug.WriteLine(line);
            //ResultLogBuilder.AppendLine(line);
        }

        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultLog)));
    }
    #endregion


    #region Helpers
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
    #endregion

}
;
