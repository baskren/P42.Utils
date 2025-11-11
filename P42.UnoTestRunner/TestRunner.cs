using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.UnoTestRunner;

public static class TestRunner
{
    public static Func<Task> GeneralCleanupAsync { get; set; } = () => Task.CompletedTask;

    public static Func<Task> GeneralInitAsync { get; set; } = () => Task.CompletedTask;

    #region Find Tests

    public static List<UnitTestAssemblyInfo> GetTestTree()
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            //var testClasses = asms.Where(a => a.Types().Any(t => t.HasAttribute<TestClassAttribute>()));
            var testAsms = new List<Assembly>();
            var testAsmInfos = new List<UnitTestAssemblyInfo>();
            foreach (var asm in asms)
            {
                try
                {
                var types =  asm.GetTypes();
                if (types.Any(t => t.HasAttribute<TestClassAttribute>()))
                {
                    testAsms.Add(asm);
                    var info = new UnitTestAssemblyInfo(asm);
                    testAsmInfos.Add(info);
                }
                }
                catch (Exception e)
                {
                    // Console.WriteLine($"Can't query assembly [{asm.FullName}] [{e}]");
                }
            }
            
            return testAsmInfos;  
        } 
    #endregion


    #region Execute Tests
    internal static async Task<TestRun> ExecuteTestsAsync(TestRun? run = null, CancellationToken? ct = null)
    {
        run ??= new TestRun();
        if (ct is { } xct)
            run.CancellationToken = xct;

        var testTree = GetTestTree();
        await testTree.ExecuteTestsAsync(run);
        return run;
    }

    internal static async Task ExecuteRequestedTestsAsync(this IEnumerable<UnitTestMethodInfo> requestedTests, TestRun run)
    {
        var requestedTestMethods = requestedTests.ToArray();
        var testTree = GetTestTree();

        for (int a = testTree.Count - 1; a >= 0; a--) 
        {
            var utAsmInfo = testTree[a];
            for (int c = utAsmInfo.Count -1; c >= 0; c--)
            {
                var utClassInfo = utAsmInfo[c];
                for (int m = utClassInfo.Count - 1; m >= 0; m--)
                {
                    var testMethod = utClassInfo[m];
                    if (requestedTestMethods.Any(rm => rm.Method == testMethod.Method))
                        continue;
                    utClassInfo.RemoveAt(m);
                }

                if (utClassInfo.Count == 0)
                    utAsmInfo.Remove(utClassInfo);
            }

            if (utAsmInfo.Count == 0)
                testTree.Remove(utAsmInfo);
        }

        await testTree.ExecuteTestsAsync(run);
    }


    internal static async Task ExecuteTestsAsync(this List<UnitTestAssemblyInfo> testTree, TestRun run)
    {
        foreach (var utAsmInfo in testTree)
        {
            run.ResultLogBuilder.AppendLine($"======[{utAsmInfo.Assembly.GetName().Name}]======");
            foreach (var utClassInfo in utAsmInfo)
            {
                if (run.CancellationToken.IsCancellationRequested)
                    return;
                await utClassInfo.ExecuteTestsAsync(run);
            }
        }

    }

    internal static async Task ExecuteTestsAsync(this UnitTestClassInfo unitClass, TestRun run)
    {
        run.ResultLogBuilder.AppendLine($"------ {unitClass.TestClassName} ------");
        foreach (var testMethod in unitClass)
        {
            if (run.CancellationToken.IsCancellationRequested)
                return;
            await testMethod.ExecuteTestAsync(run);
        }
    }

    internal static async Task ExecuteTestAsync(this UnitTestMethodInfo testMethod, TestRun run)
    {
        if (testMethod.IsIgnored(out var _))
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
