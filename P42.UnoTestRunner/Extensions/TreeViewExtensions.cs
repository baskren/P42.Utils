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
    /*
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
    */

    public static TreeViewNode CreateTreeNode(this IEnumerable collection, int depth = 0)
    {
        var node = new TreeViewNode
        {
            Content = collection,
            IsExpanded = depth < 2
        };
        foreach (var item in collection)
        {
            if (item is null)
                continue;

            var child = CreateTreeNode(item, depth+1);
            node.Children.Add(child);
        }

        return node;
    }

    public static TreeViewNode CreateTreeNode(this object obj, int depth = 0)
    {
        if (obj is IEnumerable ienum)
            return CreateTreeNode(ienum, depth);

        return new TreeViewNode 
        { 
            Content = obj,
        };
    }
    
    public static List<TreeViewNode> SelectedByDefault(this TreeViewNode nodes)
    {
        var results = new List<TreeViewNode>();
        foreach (var node in nodes.Children)
        {
            if (node.Content is UnitTestAssemblyInfo)
                results.AddRange(SelectedByDefault(node));
            else if (node.Content is UnitTestClassInfo)
                results.AddRange(SelectedByDefault(node));
            else if (node.Content is UnitTestMethodInfo methodInfo && methodInfo.IsSelectedByDefault())
                results.Add(node);
        }

        return results;
    }


}
