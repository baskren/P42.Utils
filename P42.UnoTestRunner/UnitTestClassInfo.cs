using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.UnoTestRunner;

public class UnitTestClassInfo
{
    public UnitTestClassInfo(
        Type type,
        MethodInfo[]? tests = null,
        MethodInfo? initialize = null,
        MethodInfo? cleanup = null)
    {
        Type = type;
        TestClassName = Type.Name;
        Tests = tests ?? type.GetMethodsWithAttribute<TestMethodAttribute>();
        Initialize = initialize ?? type.GetMethodsWithAttribute<TestInitializeAttribute>().FirstOrDefault();
        Cleanup = cleanup ?? type.GetMethodsWithAttribute<TestCleanupAttribute>().FirstOrDefault();
        //Instance = Activator.CreateInstance(Type)!;

    }

    public string TestClassName { get; }

    public Type Type { get; }

    public MethodInfo[] Tests { get; }

    public MethodInfo? Initialize { get; }

    public MethodInfo? Cleanup { get; }

    public override string ToString() => TestClassName;

    object? _instance;
    public object Instance 
    { 
        get => _instance ??= Activator.CreateInstance(Type)!;  
    }

}
