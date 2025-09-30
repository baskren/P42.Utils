using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System.Reflection;
using System.Runtime.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

// ReSharper disable CommentTypo
// ReSharper disable once InconsistentNaming
public static class UIElementExtensions
{
    /// <summary>
    /// Does the element have a prescribed width
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HasPrescribedWidth(this FrameworkElement element) => !double.IsNaN(element.Width) && element.Width >= 0;
    /// <summary>
    /// Does the element have a prescrib`ed height
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HasPrescribedHeight(this FrameworkElement element) => !double.IsNaN(element.Height) && element.Height >= 0;

    /// <summary>
    /// Does the element have a prescribed minimum width
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HasMinWidth(this FrameworkElement element) => !double.IsNaN(element.MinWidth) && element.MinWidth >= 0;
    /// <summary>
    /// Does the element have a prescribed minimum height
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HasMinHeight(this FrameworkElement element) => !double.IsNaN(element.MinHeight) && element.MinHeight >= 0;

    /// <summary>
    /// Does the element have a prescribed maximum width
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HasMaxWidth(this FrameworkElement element) => !double.IsNaN(element.MaxWidth) && element.MaxWidth >= 0;
    /// <summary>
    /// Does the element have a prescribed maximum height
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HasMaxHeight(this FrameworkElement element) => !double.IsNaN(element.MaxHeight) && element.MaxHeight >= 0;
    
    /// <summary>
    /// Get Bounds of FrameworkElement
    /// </summary>
    /// <param name="element"></param>
    /// <param name="relativeTo">default: AppWindow.Frame</param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    private static Rect GetBounds(this FrameworkElement element, UIElement? relativeTo = null)
    {
        relativeTo ??= Platform.Frame;
        var ttv = element.TransformToVisual(relativeTo);
        var location = ttv.TransformPoint(new Point(0, 0));
        return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
    }

    /// <summary>
    /// Get Bounds of UIElement
    /// </summary>
    /// <param name="element"></param>
    /// <param name="relativeTo">default: AppWindow.Frame</param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Local
    private static Rect GetBounds(this UIElement element, UIElement? relativeTo = null)
    {
        if (element is FrameworkElement fe)
            return GetBounds(fe, relativeTo);
        
        relativeTo ??= Platform.Frame;
        var ttv = element.TransformToVisual(relativeTo);
        var location = ttv.TransformPoint(new Point(0, 0));
        return new Rect(location, new Size(element.DesiredSize.Width, element.DesiredSize.Height));
    }
    
    /// <summary>
    /// Get Bounds of UIElement relative to another UIElement
    /// </summary>
    /// <param name="element"></param>
    /// <param name="relativeToElement"></param>
    /// <returns></returns>
    [Obsolete("Use GetBounds() instead.")]
    public static Rect GetBoundsRelativeTo(this FrameworkElement element, UIElement relativeToElement)
    {
        var ttv = element.TransformToVisual(relativeToElement);
        var location = ttv.TransformPoint(new Point(0, 0));
        return new Rect(location, new Size(element.ActualWidth, element.ActualHeight));
    }

    /// <summary>
    /// Find first ancestor of type T
    /// </summary>
    /// <param name="element"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? FindAncestor<T>(this UIElement element) where T : UIElement
    {
        var parent = VisualTreeHelper.GetParent(element); // as FrameworkElement;
        while (parent != null)
        {
            if (parent is T uiElement)
                return uiElement;
            
            parent = VisualTreeHelper.GetParent(parent);
        }
        
        return null;
    }
    
    /// <summary>
    /// Try to find first ancestor of type T
    /// </summary>
    /// <param name="element"></param>
    /// <param name="ancestor"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryFindAncestor<T>(this UIElement element, out T? ancestor) where T : UIElement
    {
        ancestor = element.FindAncestor<T>();
        return ancestor != null;
    }

    /// <summary>
    /// Convert a type into a DataTemplate Xaml string
    /// </summary>
    /// <param name="templateType"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static string AsDataTemplateXaml(this Type templateType)
    {
        if (templateType == null || !typeof(FrameworkElement).IsAssignableFrom(templateType))
            throw new Exception($"Cannot convert type [{templateType}] into DataTemplate");

        var markup = $"<DataTemplate \n\t xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:local=\"using:{templateType.Namespace}\"> \n\t\t<local:{templateType.Name} /> \n</DataTemplate>";
        //if (dataType.Namespace == typeof(Type).Namespace)
        //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:tlocal=\"using:{templateType.Namespace}\" xmlns:system=\"using:System\" x:DataType=\"system:{dataType.Name}\"><tlocal:{templateType.Name} /></DataTemplate>";
        //else
        //    markup = $"<DataTemplate xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" \n\t xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" \n\t xmlns:tlocal=\"using:{templateType.Namespace}\" \n\t xmlns:dlocal=\"using:{dataType.Namespace}\" \n\t x:DataType=\"dlocal:{dataType.Name}\"> \n\t\t<tlocal:{templateType.Name} /> \n</DataTemplate>";
        // System.Diagnostics.Debug.WriteLine("BcGroupView.GenerateDataTemplate: markup: " + markup);
        //template.
        return markup;
    }

    /// <summary>
    /// Convert type to a DataTemplate
    /// </summary>
    /// <param name="templateType"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static DataTemplate? AsDataTemplate(this Type templateType, [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
    {
        try
        {
            var markup = templateType.AsDataTemplateXaml();
            return (DataTemplate)XamlReader.Load(markup);
        }
        catch (Exception e)
        {
            QLog.Error(e, $"{filePath}:{lineNumber}");
        }
        return null;
    }

    /// <summary>
    /// Gets the first descendent of a FrameworkElement
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static UIElement? GetFirstDescendent(this FrameworkElement element)
        => element.FindChildren<UIElement>().FirstOrDefault();

    /// <summary>
    /// Find's child of a FrameworkElement by name
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="controlName"></param>
    /// <returns></returns>
    public static DependencyObject? FindChildByName(this DependencyObject parent, string controlName)
    {
        var count = VisualTreeHelper.GetChildrenCount(parent);

        for (var i = 0; i < count; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is FrameworkElement element && element.Name == controlName)
                return element;

            var findResult = FindChildByName(child, controlName);
            if (findResult != null)
                return findResult;
        }

        return null;
    }

    /// <summary>
    /// Gets the Children of a DependencyObject
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="strictTypeCheck">true: will not check for derived classes</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> FindChildren<T>(this DependencyObject parent, bool strictTypeCheck = true) where T : DependencyObject
    {
        var results = new List<T>();
        FindChildrenInternal(results, parent, strictTypeCheck);
        return results;
    }

    internal static void FindChildrenInternal<T>(List<T> results, DependencyObject? startNode, bool strictTypeCheck) where T : DependencyObject
    {
        //startNode ??= Window.Current?.Content;
        startNode ??= Platform.MainWindow?.Content;

        var count = VisualTreeHelper.GetChildrenCount(startNode);
        for (var i = 0; i < count; i++)
        {
            var current = VisualTreeHelper.GetChild(startNode, i);
            if (current is T || (!strictTypeCheck && current.GetType().GetTypeInfo().IsSubclassOf(typeof(T))))
            {
                var asType = (T)current;
                results.Add(asType);
            }
            FindChildrenInternal(results, current, strictTypeCheck);
        }
    }

}
