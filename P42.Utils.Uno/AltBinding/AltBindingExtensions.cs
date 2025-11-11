using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Data;

namespace P42.Utils.Uno;

/// <summary>
/// 
/// </summary>
public static class AltBindingExtensions
{
    
        #region Workaround Binding

    private static readonly DependencyProperty P42BindingsProperty = DependencyProperty.RegisterAttached("P42Bindings", typeof(AltBindingCollection), typeof(AltBindingExtensions), new PropertyMetadata(null));

    // ReSharper disable once UnusedMember.Local
    private static DependencyObject SetAltBindings(this DependencyObject dependencyObject, AltBindingCollection value)
    {
        dependencyObject.SetValue(P42BindingsProperty, value);
        return dependencyObject;
    }

    private static AltBindingCollection GetAltBindings(this DependencyObject dependencyObject)
    {
        if ((AltBindingCollection)dependencyObject.GetValue(P42BindingsProperty) is { } bindingCollection)
            return bindingCollection;

        bindingCollection = [];
        dependencyObject.SetValue(P42BindingsProperty, bindingCollection);
        return bindingCollection;
    }

        
    /// <summary>
    /// Unbind AltBinding
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetProperty"></param>
    /// <typeparam name="TBindable"></typeparam>
    /// <returns></returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static TBindable WUnbind<TBindable>( this TBindable target, DependencyProperty targetProperty ) where TBindable : DependencyObject
    {
        target.GetAltBindings().RemoveIf(b => b.TargetProperty == targetProperty);
        return target;
    }

    /// <summary>
    /// Work-around binding of a property of a DependencyObject to a property of a INotifiableProperty that is not a FrameworkElement
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetProperty"></param>
    /// <param name="source"></param>
    /// <param name="sourcePropertyName"></param>
    /// <param name="mode"></param>
    /// <param name="converter"></param>
    /// <param name="converterParameter"></param>
    /// <param name="converterLanguage"></param>
    /// <param name="updateSourceTrigger"></param>
    /// <param name="targetNullValue"></param>
    /// <param name="fallbackValue"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <typeparam name="TBindable"></typeparam>
    /// <returns></returns>
    public static TBindable WBind<TBindable>(
        this TBindable target,
        DependencyProperty targetProperty,
        INotifyPropertyChanged source,
        string sourcePropertyName,
        BindingMode mode = BindingMode.OneWay,
        IValueConverter? converter = null,
        object? converterParameter = null,
        string? converterLanguage = null,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object? targetNullValue = null,
        object? fallbackValue = null, 
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1
    ) where TBindable : DependencyObject
    {
        var bindings = target.GetAltBindings();
        if (bindings.FirstOrDefault(b => b.TargetProperty == targetProperty) is { } oldBinding)
            bindings.Remove(oldBinding);

        var binding = new AltBinding
        (
            target, targetProperty,
            source, sourcePropertyName,
            mode,
            converter, converterParameter, converterLanguage,
            updateSourceTrigger,
            targetNullValue, fallbackValue,
            filePath, lineNumber
        );

        bindings.Add(binding);
        return target;

    }

    /// <summary>
    /// Work-around binding of a property of a DependencyObject to a property of another DependencyObject
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetProperty"></param>
    /// <param name="source"></param>
    /// <param name="sourceProperty"></param>
    /// <param name="mode"></param>
    /// <param name="converter"></param>
    /// <param name="converterParameter"></param>
    /// <param name="converterLanguage"></param>
    /// <param name="updateSourceTrigger"></param>
    /// <param name="targetNullValue"></param>
    /// <param name="fallbackValue"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <typeparam name="TBindable"></typeparam>
    /// <returns></returns>
    public static TBindable WBind<TBindable>(
        this TBindable target,
        DependencyProperty targetProperty,
        DependencyObject source,
        DependencyProperty sourceProperty,
        BindingMode mode = BindingMode.OneWay,
        IValueConverter? converter = null,
        object? converterParameter = null,
        string? converterLanguage = null,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object? targetNullValue = null,
        object? fallbackValue = null, 
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1
    ) where TBindable : DependencyObject
    {
        var bindings = target.GetAltBindings();
        if (bindings.FirstOrDefault(b => b.TargetProperty == targetProperty) is { } oldBinding)
            bindings.Remove(oldBinding);

        var binding = new AltBinding
        (
            target, targetProperty,
            source, sourceProperty,
            mode,
            converter, converterParameter, converterLanguage,
            updateSourceTrigger,
            targetNullValue, fallbackValue,
            filePath, lineNumber
        );

        bindings.Add(binding);
        return target;
    }

    /// <summary>
    /// Work-around binding of a property of a DependencyObject to a property of another DependencyObject
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetProperty"></param>
    /// <param name="source"></param>
    /// <param name="sourceProperty"></param>
    /// <param name="mode"></param>
    /// <param name="convert"></param>
    /// <param name="convertBack"></param>
    /// <param name="converterParameter"></param>
    /// <param name="converterLanguage"></param>
    /// <param name="updateSourceTrigger"></param>
    /// <param name="targetNullValue"></param>
    /// <param name="fallbackValue"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <typeparam name="TBindable"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDest"></typeparam>
    /// <returns></returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static TBindable WBind<TBindable, TSource, TDest>(
        this TBindable target,
        DependencyProperty targetProperty,
        DependencyObject source,
        DependencyProperty sourceProperty,
        BindingMode mode = BindingMode.OneWay,
        Func<TSource?, TDest?>? convert = null,
        Func<TDest?, TSource?>? convertBack = null,
        object? converterParameter = null,
        string? converterLanguage = null,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object? targetNullValue = null,
        object? fallbackValue = null, 
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1
    ) where TBindable : DependencyObject
    {
        IValueConverter? converter = null;
        if (convert is not null || convertBack is not null)
            converter = new FuncConverter<TSource, TDest, object>(convert, convertBack, filePath, lineNumber);
        return WBind(target, targetProperty, source, sourceProperty, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
    }

    /// <summary>
    /// Work-around binding of a property of a DependencyObject to a property of a INotifiableProperty that is not a FrameworkElement
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetProperty"></param>
    /// <param name="source"></param>
    /// <param name="sourcePropertyName"></param>
    /// <param name="mode"></param>
    /// <param name="convert"></param>
    /// <param name="convertBack"></param>
    /// <param name="converterParameter"></param>
    /// <param name="converterLanguage"></param>
    /// <param name="updateSourceTrigger"></param>
    /// <param name="targetNullValue"></param>
    /// <param name="fallbackValue"></param>
    /// <param name="filePath"></param>
    /// <param name="lineNumber"></param>
    /// <typeparam name="TBindable"></typeparam>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDest"></typeparam>
    /// <returns></returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public static TBindable WBind<TBindable, TSource, TDest>(
        this TBindable target,
        DependencyProperty targetProperty,
        INotifyPropertyChanged source,
        string sourcePropertyName,
        BindingMode mode = BindingMode.OneWay,
        Func<TSource?, TDest?>? convert = null,
        Func<TDest?, TSource?>? convertBack = null,
        object? converterParameter = null,
        string? converterLanguage = null,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object? targetNullValue = null,
        object? fallbackValue = null, 
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1
    ) where TBindable : DependencyObject
    {
        IValueConverter? converter = null;
        if (convert is not null || convertBack is not null)
            converter = new FuncConverter<TSource, TDest, object>(convert, convertBack, filePath, lineNumber);
        return WBind(target, targetProperty, source, sourcePropertyName, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
    }

    #endregion

}
