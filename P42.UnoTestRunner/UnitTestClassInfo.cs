using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.UnoTestRunner;

public class UnitTestClassInfo : IList<UnitTestMethodInfo>
{
    public UnitTestClassInfo(
        UnitTestAssemblyInfo assemblyInfo,
        Type type,
        List<MethodInfo>? tests = null,
        MethodInfo? initialize = null,
        MethodInfo? cleanup = null)
    {
        AssemblyInfo = assemblyInfo;
        Type = type;
        TestClassName = Type.Name;
        Tests = (tests ?? type.GetMethodsWithAttribute<TestMethodAttribute>()).Select(m => new UnitTestMethodInfo(this, m)).ToList();
        initialize ??= type.GetMethodsWithAttribute<TestInitializeAttribute>().FirstOrDefault();
        if (initialize is not null)
        {
            Initialize = new UnitTestMethodInfo(this, initialize);
            Tests.Remove(Initialize);
        }
        cleanup ??= type.GetMethodsWithAttribute<TestCleanupAttribute>().FirstOrDefault();
        if (cleanup is not null)
        {
            Cleanup = new UnitTestMethodInfo(this, cleanup);
            Tests.Remove(Cleanup);
        }
        
    }

    public UnitTestAssemblyInfo AssemblyInfo { get; }

    public string TestClassName { get; }

    public Type Type { get; }

    public List<UnitTestMethodInfo> Tests { get; }

    public UnitTestMethodInfo? Initialize { get; }

    public UnitTestMethodInfo? Cleanup { get; }

    public override string ToString() 
        => TestClassName;

    public int IndexOf(UnitTestMethodInfo item)
    {
        return ((IList<UnitTestMethodInfo>)Tests).IndexOf(item);
    }

    public void Insert(int index, UnitTestMethodInfo item)
    {
        ((IList<UnitTestMethodInfo>)Tests).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ((IList<UnitTestMethodInfo>)Tests).RemoveAt(index);
    }

    public void Add(UnitTestMethodInfo item)
    {
        ((ICollection<UnitTestMethodInfo>)Tests).Add(item);
    }

    public void Clear()
    {
        ((ICollection<UnitTestMethodInfo>)Tests).Clear();
    }

    public bool Contains(UnitTestMethodInfo item)
    {
        return ((ICollection<UnitTestMethodInfo>)Tests).Contains(item);
    }

    public void CopyTo(UnitTestMethodInfo[] array, int arrayIndex)
    {
        ((ICollection<UnitTestMethodInfo>)Tests).CopyTo(array, arrayIndex);
    }

    public bool Remove(UnitTestMethodInfo item)
    {
        return ((ICollection<UnitTestMethodInfo>)Tests).Remove(item);
    }

    public IEnumerator<UnitTestMethodInfo> GetEnumerator()
    {
        return ((IEnumerable<UnitTestMethodInfo>)Tests).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Tests).GetEnumerator();
    }

    object? _instance;
    public object Instance 
    { 
        get => _instance ??= Activator.CreateInstance(Type)!;  
    }

    public int Count => ((ICollection<UnitTestMethodInfo>)Tests).Count;

    public bool IsReadOnly => ((ICollection<UnitTestMethodInfo>)Tests).IsReadOnly;

    public UnitTestMethodInfo this[int index] { get => ((IList<UnitTestMethodInfo>)Tests)[index]; set => ((IList<UnitTestMethodInfo>)Tests)[index] = value; }
}
