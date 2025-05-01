using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

public static class TreeViewExtensions
{
    public static TreeViewNode CreateTreeNode(this IDictionary map)
    {
        var node = new TreeViewNode();
        foreach (var key in map.Keys)
        {
            var value = map[key];
            if (value is null)
                continue;

            var child = CreateTreeNode(value);
            child.Content = key is Assembly asm
                ? asm.GetName().Name
                : key;

            if (key is Assembly)
                child.IsExpanded = true;

            node.Children.Add(child);
        }

        return node;
    }

    public static TreeViewNode CreateTreeNode(this IEnumerable collection)
    {
        var node = new TreeViewNode();
        foreach (var item in collection)
        {
            if (item is null)
                continue;

            var child = CreateTreeNode(item);
            node.Children.Add(child);
        }

        return node;
    }

    public static TreeViewNode CreateTreeNode(this object obj)
    {
        if (obj is IDictionary dict)
            return CreateTreeNode(dict);
        if (obj is IEnumerable ienum)
            return CreateTreeNode(ienum);
        return new TreeViewNode { Content = obj };
    }
    
}
