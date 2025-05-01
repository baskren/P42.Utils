using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.Mime.MediaTypeNames;

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

    #region Find Tests
    public static Dictionary<Assembly, Dictionary<UnitTestClassInfo, IEnumerable<UnitTestMethodInfo>>> GetTestTree(string searchText = "")
    {
        var result = new Dictionary<Assembly, Dictionary<UnitTestClassInfo, IEnumerable<UnitTestMethodInfo>>>();

        foreach (var testAssembly in GetTestAssemblies())
        {
            foreach (var testClass in GetTestClasses(testAssembly))
            {
                var testMethods = GetTestMethods(testClass);

                if (!string.IsNullOrWhiteSpace(searchText))
                    if (!testClass.TestClassName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        testMethods = testMethods.Where(m => m.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                if (!testMethods.Any())
                    continue;

                if (!result.ContainsKey(testAssembly))
                    result[testAssembly] = [];

                result[testAssembly][testClass] = testMethods;

            }
        }

        return result;
    }

    public static IEnumerable<Assembly> GetTestAssemblies()
        => AppDomain.CurrentDomain.GetAssemblies();
            //.Where(a => a.GetName().Name!.EndsWith("tests", StringComparison.OrdinalIgnoreCase))
            //.OrderBy(a => a.FullName);

    public static IEnumerable<UnitTestClassInfo> GetTestClasses(Assembly asm)
    {
        var testTypes = asm.GetTypes()
            .Where(t => t.GetCustomAttribute<TestClassAttribute>() != null)
            .OrderBy(t => t.Name).ToArray();

        return testTypes.Select(t => new UnitTestClassInfo(t));
    }

    public static IEnumerable<UnitTestMethodInfo> GetTestMethods(UnitTestClassInfo testClass)
        => testClass.Tests
            .Select(t => new UnitTestMethodInfo(testClass, t))
            .ToArray();
    #endregion



    #region Execute Tests
    internal static async Task<TestRun> ExecuteTestsAsync(TestRun? run = null, CancellationToken? ct = null)
    {
        run ??= new TestRun();
        if (ct is CancellationToken xct)
            run.CancellationToken = xct;

        var testTree = GetTestTree();
        await testTree.ExecuteTestsAsync(run);
        return run;
    }

    internal static async Task ExecuteTestsAsync(this Dictionary<Assembly, Dictionary<UnitTestClassInfo, IEnumerable<UnitTestMethodInfo>>> testTree, TestRun run)
    {
        foreach (var asm in testTree.Keys)
        {
            foreach (var unitClassKvp in testTree[asm])
            {
                if (run.CancellationToken.IsCancellationRequested)
                    return;

                var testMethods = unitClassKvp.Value;
                await testMethods.ExecuteTestsAsync(run);
            }
        }


    }

    internal static async Task ExecuteTestsAsync(this UnitTestClassInfo unitClass, TestRun run)
    {
        var testMethods = GetTestMethods(unitClass);
        await testMethods.ExecuteTestsAsync(run);
    }

    internal static async Task ExecuteTestsAsync(this IEnumerable<UnitTestMethodInfo> testMethods, TestRun run)
    {
        foreach (var testMethod in testMethods)
            await testMethod.ExecuteTestAsync(run);
    }

    internal static async Task ExecuteTestAsync(this UnitTestMethodInfo testMethod, TestRun run)
    {
        if (run.CancellationToken.IsCancellationRequested)
            return;

        if (testMethod.IsIgnored(out var ignoreMessage))
        {
            run.Ignored++;
            return;
        }

        foreach (var testCase in testMethod.GetCases())
        {
            if (run.CancellationToken.IsCancellationRequested)
                return;

            await testCase.Invoke(run, testMethod);
        }

    }
    #endregion

}
