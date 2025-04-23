using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.UnoTestRunner;

public static class TestRunner
{

    static Func<Task> _generalCleanupAsync = () => Task.CompletedTask;
    public static Func<Task> GeneralCleanupAsync 
    { 
        get => _generalCleanupAsync; 
        set => _generalCleanupAsync = value; 
    }

    static Func<Task> _generalInitAsync = () => Task.CompletedTask;
    public static Func<Task> GeneralInitAsync
    {
        get => _generalInitAsync;
        set => _generalInitAsync = value;
    }

    public static async Task Run(CancellationToken? ct = null)
    {
        var run = new TestRun();
        var unitClasses = Initialize();

        foreach (var unitClass in unitClasses)
        {
            if (ct?.IsCancellationRequested ?? false)
                return;
            await run.ExecuteTests(unitClass, ct ?? CancellationToken.None);
        }

    }

    static IEnumerable<UnitTestClassInfo> Initialize()
    {
        var testAssemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.GetName().Name!.EndsWith("tests", StringComparison.OrdinalIgnoreCase))
            .OrderBy(a => a.FullName);

        var testTypes = testAssemblies
            .SelectMany(t => t.GetTypes())
            .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null)
            .OrderBy(t => t.Name).ToArray();

        return testTypes
            .Select(t => new UnitTestClassInfo(t));
    }

    static async Task ExecuteTests(this TestRun run, UnitTestClassInfo unitClass, CancellationToken ct )
    {
        var tests = unitClass.Tests
            .Select(t => new UnitTestMethodInfo(unitClass, t))
            .ToArray();

        foreach (var test in tests)
        {
            if (ct.IsCancellationRequested)
                return;

            if (test.IsIgnored(out var ignoreMessage))
            {
                run.Ignored++;
                continue;
            }

            foreach (var testCase in test.GetCases())
            {
                if (ct.IsCancellationRequested)
                    return;

                await testCase.Invoke(run, test);
            }
        }

    }


}
