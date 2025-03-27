using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace P42.Utils.Uno;

/// <summary>
/// 
/// </summary>
public static class WorkaroundBindingExtensions
{
    
        #region Workaround Binding

    private static readonly DependencyProperty P42BindingsProperty = DependencyProperty.RegisterAttached("P42Bindings", typeof(WorkaroundBindingCollection), typeof(WorkaroundBindingExtensions), new PropertyMetadata(default));

    private static DependencyObject SetWorkaroundBindings(this DependencyObject dependencyObject, WorkaroundBindingCollection value)
    {
        dependencyObject.SetValue(P42BindingsProperty, value);
        return dependencyObject;
    }

    private static WorkaroundBindingCollection GetWorkaroundBindings(this DependencyObject dependencyObject)
    {
        if ((WorkaroundBindingCollection)dependencyObject.GetValue(P42BindingsProperty) is { } bindingCollection)
            return bindingCollection;

        bindingCollection = [];
        dependencyObject.SetValue(P42BindingsProperty, bindingCollection);
        return bindingCollection;
    }

        
    /// <summary>
    /// Unbind WorkaroundBinding
    /// </summary>
    /// <param name="target"></param>
    /// <param name="targetProperty"></param>
    /// <typeparam name="TBindable"></typeparam>
    /// <returns></returns>
    public static TBindable WUnbind<TBindable>( this TBindable target, DependencyProperty targetProperty ) where TBindable : DependencyObject
    {
        target.GetWorkaroundBindings().RemoveIf(b => b.TargetProperty == targetProperty);
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
        var bindings = target.GetWorkaroundBindings();
        if (bindings.FirstOrDefault(b => b.TargetProperty == targetProperty) is { } oldBinding)
            bindings.Remove(oldBinding);

        var binding = new WorkaroundBinding
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
        var bindings = target.GetWorkaroundBindings();
        if (bindings.FirstOrDefault(b => b.TargetProperty == targetProperty) is { } oldBinding)
            bindings.Remove(oldBinding);

        var binding = new WorkaroundBinding
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


    /*
    //const string bindingContextPath = Binding.SelfPath;

    /// <summary>Bind to a specified property</summary>
    internal static TBindable		Bind<TBindable>(
        this TBindable target,
        DependencyProperty targetProperty,
        object source,
        string path,
        BindingMode mode = BindingMode.OneWay,
        IValueConverter converter = null,
        object converterParameter = null,
        string converterLanguage = null,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object targetNullValue = null,
        object fallbackValue = null, [CallerFilePath] string filePath = null, [CallerLineNumber] int lineNumber = -1
    ) where TBindable : DependencyObject
    {
        var binding = new Binding
        {
            Source = source,
            Mode = mode,
            Converter = converter,
            ConverterParameter = converterParameter,
            UpdateSourceTrigger = updateSourceTrigger,
            TargetNullValue = targetNullValue,
            FallbackValue = fallbackValue
        };
        if (!string.IsNullOrWhiteSpace(converterLanguage))
            binding.ConverterLanguage = converterLanguage;
        if (!string.IsNullOrWhiteSpace(path))
            binding.Path = new PropertyPath(path);
#if !HAS_UNO

			if (target is FrameworkElement element)
			{
				//element.ClearValue();
				element.SetBinding(targetProperty, binding);
				return target;
			}
#endif
        target.ClearValue(targetProperty);
        BindingOperations.SetBinding(target, targetProperty, binding);
        return target;
    }

    /// <summary>Bind to a specified property with inline conversion</summary>
    internal static TBindable Bind<TBindable, TSource, TDest>(
        this TBindable target,
        DependencyProperty targetProperty,
        object source = null,
        string path = null,
        BindingMode mode = BindingMode.OneWay,
        Func<TSource, TDest> convert = null,
        Func<TDest, TSource> convertBack = null,
        object converterParameter = null,
        string converterLanguage = null,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object targetNullValue = null,
        object fallbackValue = null
    ) where TBindable : DependencyObject
    {
        var converter = new FuncConverter<TSource, TDest, object>(convert, convertBack);
        var binding = new Binding
        {
            Source = source,
            Mode = mode,
            Converter = converter,
            ConverterParameter = converterParameter,
            UpdateSourceTrigger = updateSourceTrigger,
            TargetNullValue = targetNullValue,
            FallbackValue = fallbackValue
        };
        if (!string.IsNullOrWhiteSpace(converterLanguage))
            binding.ConverterLanguage = converterLanguage;
        if (!string.IsNullOrWhiteSpace(path))
            binding.Path = new PropertyPath(path);
#if !HAS_UNO

			if (target is FrameworkElement element)
			{
				//element.ClearValue();
				element.SetBinding(targetProperty, binding);
				return target;
			}
#endif
        target.ClearValue(targetProperty);
        BindingOperations.SetBinding(target, targetProperty, binding);
        return target;
    }

    /// <summary>Bind to a specified property with inline conversion and conversion parameter</summary>
    internal static TBindable Bind<TBindable, TSource, TParam, TDest>(
        this TBindable target,
        DependencyProperty targetProperty,
        object source = null,
        string path = null,
        BindingMode mode = BindingMode.OneWay,
        Func<TSource, TParam, TDest> convert = null,
        Func<TDest, TParam, TSource> convertBack = null,
        object converterParameter = null,
        string converterLanguage = null,
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object targetNullValue = null,
        object fallbackValue = null
    ) where TBindable : DependencyObject
    {
        var converter = new FuncConverter<TSource, TDest, TParam>(convert, convertBack);
        var binding = new Binding
        {
            Source = source,
            Mode = mode,
            Converter = converter,
            ConverterParameter = converterParameter,
            UpdateSourceTrigger = updateSourceTrigger,
            TargetNullValue = targetNullValue,
            FallbackValue = fallbackValue
        };
        if (!string.IsNullOrWhiteSpace(converterLanguage))
            binding.ConverterLanguage = converterLanguage;
        if (!string.IsNullOrWhiteSpace(path))
            binding.Path = new PropertyPath(path);

#if !HAS_UNO

			if (target is FrameworkElement element)
            {
				//element.ClearValue();
				element.SetBinding(targetProperty, binding);
				return target;
            }
#endif
        target.ClearValue(targetProperty);
        BindingOperations.SetBinding(target, targetProperty, binding);
        return target;
    }
    */
    /*
    /// <summary>Bind to the default property</summary>
    public static TBindable Bind<TBindable>(
        this TBindable bindable,
        string path = bindingContextPath,
        BindingMode mode = BindingMode.OneWay,
        IValueConverter converter = null,
        object converterParameter = null,
        string stringFormat = null,
        object source = null,
        object targetNullValue = null,
        object fallbackValue = null
    ) where TBindable : DependencyObject
    {
        bindable.Bind(
            DefaultBindableProperties.GetFor(bindable),
            path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
        );
        return bindable;
    }

    /// <summary>Bind to the default property with inline conversion</summary>
    public static TBindable Bind<TBindable, TSource, TDest>(
        this TBindable bindable,
        string path = bindingContextPath,
        BindingMode mode = BindingMode.OneWay,
        Func<TSource, TDest> convert = null,
        Func<TDest, TSource> convertBack = null,
        object converterParameter = null,
        string stringFormat = null,
        object source = null,
        object targetNullValue = null,
        object fallbackValue = null
    ) where TBindable : DependencyObject
    {
        var converter = new FuncConverter<TSource, TDest, object>(convert, convertBack);
        bindable.Bind(
            DefaultBindableProperties.GetFor(bindable),
            path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
        );
        return bindable;
    }

    /// <summary>Bind to the default property with inline conversion and conversion parameter</summary>
    public static TBindable Bind<TBindable, TSource, TParam, TDest>(
        this TBindable bindable,
        string path = bindingContextPath,
        BindingMode mode = BindingMode.OneWay,
        Func<TSource, TParam, TDest> convert = null,
        Func<TDest, TParam, TSource> convertBack = null,
        object converterParameter = null,
        string stringFormat = null,
        object source = null,
        object targetNullValue = null,
        object fallbackValue = null
    ) where TBindable : DependencyObject
    {
        var converter = new FuncConverter<TSource, TDest, TParam>(convert, convertBack);
        bindable.Bind(
            DefaultBindableProperties.GetFor(bindable),
            path, mode, converter, converterParameter, stringFormat, source, targetNullValue, fallbackValue
        );
        return bindable;
    }

    /// <summary>Bind to the <typeparamref name="TBindable"/>'s default Command and CommandParameter properties </summary>
    /// <param name="parameterPath">If null, no binding is created for the CommandParameter property</param>
    public static TBindable BindCommand<TBindable>(
        this TBindable bindable,

        string path = bindingContextPath,
        object source = null,
        string parameterPath = bindingContextPath,
        object parameterSource = null
    ) where TBindable : DependencyObject
    {
        (var commandProperty, var parameterProperty) = DefaultBindableProperties.GetForCommand(bindable);

        bindable.SetBinding(commandProperty, new Binding(path: path, source: source));

        if (parameterPath != null)
            bindable.SetBinding(parameterProperty, new Binding(path: parameterPath, source: parameterSource));

        return bindable;
    }
    */


}
