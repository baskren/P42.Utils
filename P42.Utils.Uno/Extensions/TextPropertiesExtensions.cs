using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

public static class TextPropertiesExtensions
{

    private static readonly Dictionary<Type, double> DefaultFontSizes = new();
    
    /// <summary>
    /// Get default FontSize for given type
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static double DefaultFontSize<T>() where T : new()
    {
        var type = typeof(T);
        if (DefaultFontSizes.TryGetValue(type, out var cachedSize))
            return cachedSize;

        var obj = new T();
        if (obj is null)
            throw new Exception($"Cannot create a default text object of type {type}");

        if (obj.TryGetPropertyValue("FontSize", out var fontSize) && fontSize is double size)
            return DefaultFontSizes[type] = size;
        
        throw new Exception($"Cannot obtain FontSize property value for give type {type}");
    }

    /// <summary>
    /// Get default FontSize for given UIElement, typically a TextBlock
    /// </summary>
    /// <param name="textElement"></param>
    /// <returns></returns>
    public static double DefaultFontSize<T>(this T textElement) where T : FrameworkElement, new()
        => DefaultFontSize<T>();
}
