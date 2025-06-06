using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace P42.UnoTestRunner;

public class UnitTestAssemblyInfo : IList<UnitTestClassInfo>
{
    public Assembly Assembly { get; }

    public List<UnitTestClassInfo> Classes { get; }

    public int Count => ((ICollection<UnitTestClassInfo>)Classes).Count;

    public bool IsReadOnly => ((ICollection<UnitTestClassInfo>)Classes).IsReadOnly;

    public UnitTestClassInfo this[int index] { get => ((IList<UnitTestClassInfo>)Classes)[index]; set => ((IList<UnitTestClassInfo>)Classes)[index] = value; }

    public UnitTestAssemblyInfo(
        Assembly assembly,
        List<UnitTestClassInfo>? classes = null)
    {
        Assembly = assembly;
        Classes = classes ?? 
            assembly
            .GetTypesWithAttribute<TestClassAttribute>()
            .OrderBy(t => t.Name)
            .Select(c => new UnitTestClassInfo(this, c))
            .ToList();
    }

    public override string ToString()
        => Assembly.GetName()!.Name ?? string.Empty;

    public int IndexOf(UnitTestClassInfo item)
    {
        return ((IList<UnitTestClassInfo>)Classes).IndexOf(item);
    }

    public void Insert(int index, UnitTestClassInfo item)
    {
        ((IList<UnitTestClassInfo>)Classes).Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ((IList<UnitTestClassInfo>)Classes).RemoveAt(index);
    }

    public void Add(UnitTestClassInfo item)
    {
        ((ICollection<UnitTestClassInfo>)Classes).Add(item);
    }

    public void Clear()
    {
        ((ICollection<UnitTestClassInfo>)Classes).Clear();
    }

    public bool Contains(UnitTestClassInfo item)
    {
        return ((ICollection<UnitTestClassInfo>)Classes).Contains(item);
    }

    public void CopyTo(UnitTestClassInfo[] array, int arrayIndex)
    {
        ((ICollection<UnitTestClassInfo>)Classes).CopyTo(array, arrayIndex);
    }

    public bool Remove(UnitTestClassInfo item)
    {
        return ((ICollection<UnitTestClassInfo>)Classes).Remove(item);
    }

    public IEnumerator<UnitTestClassInfo> GetEnumerator()
    {
        return ((IEnumerable<UnitTestClassInfo>)Classes).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Classes).GetEnumerator();
    }
}
