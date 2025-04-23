using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

public static class VisualTreeUtils
{
    public static T? FindVisualChildByType<T>(this DependencyObject element, bool includeCurrent = true)
        where T : DependencyObject
    {
        if (element == null)
            return default;

        if (includeCurrent && element is T t)
            return t;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)        
            if (VisualTreeHelper.GetChild(element, i) is T elementAsT)
                return elementAsT;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            if (VisualTreeHelper.GetChild(element, i).FindVisualChildByType<T>(true) is { } result)
                return result;

        return default;
    }

    public static T? FindElementOfTypeInSubtree<T>(this DependencyObject element, bool includeCurrent = true)
			where T : DependencyObject
        => FindVisualChildByType<T>(element, includeCurrent) ?? default;
    


    public static DependencyObject? FindVisualChildByName(this FrameworkElement parent, string name)
    {
        if (parent.Name == name)
            return parent;

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            if (VisualTreeHelper.GetChild(parent, i) is FrameworkElement childAsFE
                && FindVisualChildByName(childAsFE, name) is DependencyObject result)
                    return result;

        return null;
    }

    public static T? FindVisualParentByType<T>(this DependencyObject element, bool includeCurrent = true)
        where T : DependencyObject
    {
        if (element is null)
            return default;

        if (includeCurrent && element is T t)
            return t;

        return element is T elementAsT
            ? elementAsT
            : VisualTreeHelper.GetParent(element).FindVisualParentByType<T>(true);
    }

    public static List<T> FindVisualChildrenByType<T>(DependencyObject parent)
			where T : DependencyObject
    {
        List<T> children = [];

        if (parent is T parentAsT)
            children.Add(parentAsT);

        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            if (VisualTreeHelper.GetChild(parent, i) is DependencyObject childAsFE)
                children.AddRange(FindVisualChildrenByType<T>(childAsFE));

        return children;
    }

}
