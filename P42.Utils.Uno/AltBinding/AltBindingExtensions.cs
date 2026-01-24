using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Data;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

/// <summary>
/// Alternative Binding (in case something wierd is going on with Uno Binding)
/// </summary>
public static class AltBindingExtensions
{
    
    
    public static List<string>? GetExcepts(object? except)
    {
        return except switch
        {
            null => null,
            string str => [str],
            IEnumerable<string> enumerable => [.. enumerable],
            _ => throw new Exception("GetTextExcepts: argument must be null, string, or IEnumerable<string>")
        };
    }


        #region Workaround Binding

    private static readonly DependencyProperty P42BindingsProperty = DependencyProperty.RegisterAttached("P42Bindings", typeof(AltBindingCollection), typeof(AltBindingExtensions), new PropertyMetadata(null));

    // ReSharper disable once UnusedMember.Local
    extension(DependencyObject dependencyObject)
    {
        private DependencyObject SetAltBindings(AltBindingCollection value)
        {
            dependencyObject.SetValue(P42BindingsProperty, value);
            return dependencyObject;
        }

        private AltBindingCollection GetAltBindings()
        {
            if ((AltBindingCollection)dependencyObject.GetValue(P42BindingsProperty) is { } bindingCollection)
                return bindingCollection;

            bindingCollection = [];
            dependencyObject.SetValue(P42BindingsProperty, bindingCollection);
            return bindingCollection;
        }
    }

    extension<TBindable>(TBindable target) where TBindable : DependencyObject
    {
        private void CheckArguments(DependencyProperty targetProperty, 
            object? source, 
            string? sourcePropertyName, 
            IValueConverter? converter, 
            object? converterParameter, 
            string? converterLanguage, 
            string filePath, 
            int lineNumber)
        {
            try
            {
                target.CheckProperty(targetProperty, filePath, lineNumber);


                if (source is null)
                {
                    //var msg = $"BIND: Source is null when Bind() is called.  Cannot check if Target Property type matches Source Property type.";
                    //Console.WriteLine(msg);
                    //Debug.WriteLine(msg);
                    return;
                }

                var targetPropertyType = target.GetValue(targetProperty)?.GetType();
                /*    
#if HAS_UNO
            var dependencyPropertyType = typeof(DependencyProperty);
            //var targetPropertyNameField = dependencyPropertyType.GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
            var targetPropertyValueTypeField = dependencyPropertyType.GetField("_propertyType", BindingFlags.NonPublic | BindingFlags.Instance);
            //var targetPropertyOwnerTypeField = dependencyPropertyType.GetField("_ownerType", BindingFlags.NonPublic | BindingFlags.Instance);

            var targetPropertyType = (Type)targetPropertyValueTypeField.GetValue(targetProperty);
#else
            var targetPropertyType = target.GetValue(targetProperty)?.GetType();
#endif
            */
                if (targetPropertyType is null)
                {
                    //var msg = $"BIND: Target Property is null when Bind() is called.  Cannot check if Target Property type matches Source Property type.";
                    //Console.WriteLine(msg);
                    //Debug.WriteLine(msg);
                    return;
                }

                var sourceClassType = source.GetType();
                var sourceValueType = sourceClassType;
                var sourceLabel = sourceClassType.ToString();
                if (sourcePropertyName is not null)
                {
                    sourceValueType = sourceClassType.GetProperties().FirstOrDefault(p => p.Name == sourcePropertyName)?.PropertyType;
                    if (sourceValueType is null)
                        throw new ArgumentNullException($"No property found at {sourceClassType}.{sourcePropertyName}.  {filePath}:{lineNumber}");
                    sourceLabel = $"{sourceClassType}.{sourcePropertyName}";
                }

                if (converter is not null)
                {
                    var sourceDefaultValue = sourceValueType.IsValueType
                        ? Activator.CreateInstance(sourceValueType)
                        : null;

                    var converterDefaultValue = converter.Convert(sourceDefaultValue, targetPropertyType, converterParameter, converterLanguage);
                    CheckTypeMatch(targetPropertyType, converterDefaultValue.GetType(), "TargetProperty", "Converter result", filePath, lineNumber);
                }
                else
                    CheckTypeMatch(targetPropertyType, sourceValueType, "TargetProperty", sourceLabel, filePath, lineNumber);
            }
            catch (Exception ex)
            {
                QLog.Debug(ex);
            }
        }

        private void CheckArguments(DependencyProperty targetProperty, 
            DependencyObject source, 
            DependencyProperty sourceProperty, 
            IValueConverter? converter, 
            object? converterParameter, 
            string? converterLanguage, 
            string filePath, 
            int lineNumber)
        {
            try
            {
                target.CheckProperty(targetProperty, filePath, lineNumber);
                source.CheckProperty(sourceProperty, filePath, lineNumber);

                var targetPropertyType = target.GetValue(targetProperty)?.GetType();
                /*    
#if HAS_UNO
            var dependencyPropertyType = typeof(DependencyProperty);
            //var targetPropertyNameField = dependencyPropertyType.GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
            var targetPropertyValueTypeField = dependencyPropertyType.GetField("_propertyType", BindingFlags.NonPublic | BindingFlags.Instance);
            //var targetPropertyOwnerTypeField = dependencyPropertyType.GetField("_ownerType", BindingFlags.NonPublic | BindingFlags.Instance);

            var targetPropertyType = (Type)targetPropertyValueTypeField.GetValue(targetProperty);
#else
            var targetPropertyType = target.GetValue(targetProperty)?.GetType();
#endif
            */
                if (targetPropertyType is null)
                {
                    //var msg = $"BIND: Target Property is null when Bind() is called.  Cannot check if Target Property type matches Source Property type.";
                    //Console.WriteLine(msg);
                    //Debug.WriteLine(msg);
                    return;
                }

                var sourceClassType = source.GetType();
                var sourceValueType = sourceClassType;
                var sourceLabel = sourceClassType.ToString();
            
                var sourcePropertyType = source.GetValue(sourceProperty)?.GetType();
                if (sourcePropertyType is null)
                    return;
            
                if (converter is not null)
                {
                    var sourceDefaultValue = sourcePropertyType.IsValueType
                        ? Activator.CreateInstance(sourceValueType)
                        : null;

                    var converterDefaultValue = converter.Convert(sourceDefaultValue, targetPropertyType, converterParameter, converterLanguage);
                    CheckTypeMatch(targetPropertyType, converterDefaultValue.GetType(), "TargetProperty", "Converter result", filePath, lineNumber);
                }
                else
                    CheckTypeMatch(targetPropertyType, sourcePropertyType, "TargetProperty", sourceLabel, filePath, lineNumber);
            }
            catch (Exception ex)
            {
                QLog.Debug(ex);
            }
        }
    }

#if !WINDOWS
    private static FieldInfo? _flagsAttachedField;
#endif

    private static void CheckProperty<TBindable>(
        this TBindable target, 
        DependencyProperty targetProperty, 
        string filePath, 
        int lineNumber) where TBindable : DependencyObject
    {
#if !WINDOWS
        _flagsAttachedField ??= typeof(DependencyProperty).GetField("_flags", BindingFlags.Instance | BindingFlags.NonPublic);
        if (_flagsAttachedField != null && _flagsAttachedField.GetValue(targetProperty) is int flags && flags % 2 == 1)
            return;
#endif
        if (CheckTypeMatch(target.GetType(), target, targetProperty, filePath, lineNumber))
            return;

        var msg = $"BIND: TargetProperty is not member of targetClass [{target.GetType()}]. This is ok if TargetProperty is an Attached Property.  {filePath}:{lineNumber}";
        Console.WriteLine(msg);
        System.Diagnostics.Debug.WriteLine(msg);
    }

    private static int LastLine([CallerLineNumber] int lineNumber = -1)
        => lineNumber < 0 ? -1 : lineNumber;

    private static bool CheckTypeMatch<TBindable>(
        Type targetType, 
        TBindable target, 
        DependencyProperty targetProperty, 
        string filePath, 
        int lineNumber) where TBindable : DependencyObject
    {
        var lastLine = -1;

        try
        {
            lastLine = LastLine();
            var properties = targetType.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            lastLine = LastLine();
            foreach (var property in properties)
            {
                lastLine = LastLine();
                if (property.PropertyType != typeof(DependencyProperty))
                    continue;

                lastLine = LastLine();
#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                if (property.GetValue(target) == targetProperty)
                    return true;
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            }

            lastLine = LastLine();
            var fields = targetType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            lastLine = LastLine();
            foreach (var field in fields)
            {
                lastLine = LastLine();
                if (field.FieldType != typeof(DependencyProperty))
                    continue;

#pragma warning disable CS0252 // Possible unintended reference comparison; left hand side needs cast
                lastLine = LastLine();
                if (field.GetValue(target) == targetProperty)
                    return true;
#pragma warning restore CS0252 // Possible unintended reference comparison; left hand side needs cast
            }

            lastLine = LastLine();
            if (targetType == typeof(DependencyObject))
                return false;

            if (targetType.BaseType is null)
                return false;
            
            lastLine = LastLine();
            return CheckTypeMatch(targetType.BaseType, target, targetProperty, filePath, lineNumber);
        }
        catch (Exception ex)
        {
            var msg = $"BIND.CheckTypeMatch({targetType}, {target}, {targetProperty}, {filePath}, {lineNumber}) : LastLine [{lastLine}] Exception: [{ex.Message}][{ex.Source}[{ex.StackTrace}]]";
            Console.WriteLine(msg);
            System.Diagnostics.Debug.WriteLine(msg);

            return false;
        }
    }

    private static void CheckTypeMatch(
        Type targetPropertyType, 
        Type sourceType, 
        string targetLabel, 
        string sourceLabel,  
        string filePath, 
        int lineNumber)
    {
        if (targetPropertyType == typeof(SolidColorBrush) && sourceType == typeof(Brush))
            return;

        if (targetPropertyType.IsAssignableFrom(sourceType))
            return;

        var msg = $"BIND: {targetLabel} type [{targetPropertyType}] is not assignable from the type [{sourceType}] found at {sourceLabel}.  This can be a false detection in Windows platform apps.  {filePath}:{lineNumber}";
        Console.WriteLine(msg);
        System.Diagnostics.Debug.WriteLine(msg);
    }


    /// <param name="target"></param>
    /// <typeparam name="TBindable"></typeparam>
    extension<TBindable>(TBindable target) where TBindable : DependencyObject
    {
        /// <summary>
        /// Unbind AltBinding
        /// </summary>
        /// <param name="targetProperty"></param>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public TBindable AltUnbind(DependencyProperty targetProperty )
        {
            target.GetAltBindings().RemoveIf(b => b.TargetProperty == targetProperty);
            return target;
        }

        /// <summary>
        /// Work-around binding of a property of a DependencyObject to a property of a INotifiableProperty that is not a FrameworkElement
        /// </summary>
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
        /// <returns></returns>
        public TBindable AltBind(DependencyProperty targetProperty,
            INotifyPropertyChanged source,
            string sourcePropertyName,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter? converter = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null, 
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            target.CheckArguments(targetProperty, source, sourcePropertyName, converter, converterParameter, converterLanguage, filePath, lineNumber);
        
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
        /// <returns></returns>
        public TBindable AltBind(DependencyProperty targetProperty,
            DependencyObject source,
            DependencyProperty sourceProperty,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter? converter = null,
            object? converterParameter = null,
            string? converterLanguage = null,
            UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
            object? targetNullValue = null,
            object? fallbackValue = null, 
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            target.CheckArguments(targetProperty, source, sourceProperty, converter, converterParameter, converterLanguage, filePath, lineNumber);
        
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
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public TBindable AltBind<TSource, TDest>(DependencyProperty targetProperty,
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
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            IValueConverter? converter = null;
            if (convert is not null || convertBack is not null)
                converter = new FuncConverter<TSource, TDest, object>(convert, convertBack, filePath, lineNumber);
            return target.AltBind(targetProperty, source, sourceProperty, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
        }

        /// <summary>
        /// Work-around binding of a property of a DependencyObject to a property of a INotifiableProperty that is not a FrameworkElement
        /// </summary>
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
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDest"></typeparam>
        /// <returns></returns>
        // ReSharper disable once UnusedMethodReturnValue.Global
        public TBindable AltBind<TSource, TDest>(DependencyProperty targetProperty,
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
            [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = -1
        )
        {
            IValueConverter? converter = null;
            if (convert is not null || convertBack is not null)
                converter = new FuncConverter<TSource, TDest, object>(convert, convertBack, filePath, lineNumber);
            return target.AltBind(targetProperty, source, sourcePropertyName, mode, converter, converterParameter, converterLanguage, updateSourceTrigger, targetNullValue, fallbackValue, filePath, lineNumber);
        }
    }

    #endregion

}
