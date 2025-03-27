using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

internal class WorkaroundBinding : IDisposable
{
    private WeakReference<DependencyObject>? _targetWeakReference;
    /// <summary>
    /// Dependency object to which the binding is attached
    /// </summary>
    public DependencyObject? TargetDependencyObject
    {
        get
        {
            if (!(_targetWeakReference?.TryGetTarget(out var target) ?? false))
                return null;
            return target;

        }
        init => _targetWeakReference = value is null 
            ? null 
            : new WeakReference<DependencyObject>(value); 
    }

    private WeakReference<object>? _sourceWeakReference;

    private DependencyObject? SourceDependencyObject
    {
        get
        {
            if (_sourceWeakReference?.TryGetTarget(out var source) ?? false)
                return source as DependencyObject;
            return null;
        }
        init => _sourceWeakReference = value is null 
            ? null 
            : new WeakReference<object>(value); 
    }

    private INotifyPropertyChanged? SourceNotifyPropertyChanged
    {
        get
        {
            if (_sourceWeakReference?.TryGetTarget(out var source) ?? false)
                return source as INotifyPropertyChanged;
            return null;
        }
        init => _sourceWeakReference = value is null 
            ? null 
            : new WeakReference<object>(value); 
    }

    /// <summary>
    /// DependencyProperty being targeted by the binding
    /// </summary>
    public DependencyProperty? TargetProperty { get; }
    private readonly Type? _targetPropertyType = null;

    private readonly DependencyProperty? _sourceProperty;
    private readonly Type? _sourcePropertyType;

    private readonly string _sourcePropertyName = string.Empty;
    private readonly PropertyInfo? _sourcePropertyInfo;

    private readonly long _onSourceDependencyPropertyChangedIndex = -1;
    private readonly long _onTargetDependencyPropertyChangedIndex = -1;

    private readonly IValueConverter? _valueConverter;
    private readonly object? _valueConverterParameter;
    private readonly string _valueConverterLanguage;

    private readonly object? _targetNullValue;
    private readonly object? _fallbackValue;

    private bool _isDisposed;

    private readonly string? _filePath;
    private readonly int _lineNumber;

    internal WorkaroundBinding(DependencyObject target, DependencyProperty targetProperty, 
        DependencyObject source, DependencyProperty sourceProperty, 
        BindingMode bindingMode, 
        IValueConverter? valueConverter, object? valueConverterProperty, string? valueConverterLanguage, 
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object? targetNullValue = null,
        object? fallbackValue = null, 
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        if (updateSourceTrigger is UpdateSourceTrigger.Explicit or UpdateSourceTrigger.LostFocus)
            throw new ArgumentException($"UpdateSourceTrigger.Explicit and UpdateSourceTrigger.LostFocus are not supported in WorkaroundBinding. FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");
        
        TargetDependencyObject = target;
        TargetProperty = targetProperty;

        SourceDependencyObject = source;
        _sourceProperty = sourceProperty;

        _valueConverter = valueConverter;
        _valueConverterParameter = valueConverterProperty;
        _valueConverterLanguage = valueConverterLanguage ?? string.Empty;
        
        _targetNullValue = targetNullValue;
        _fallbackValue = fallbackValue;
        
        _filePath = filePath;
        _lineNumber = lineNumber;

        OnSourceDependencyPropertyChanged(source, sourceProperty);

        switch (bindingMode)
        {
            case BindingMode.OneWay:
                _onSourceDependencyPropertyChangedIndex = source.RegisterPropertyChangedCallback(sourceProperty, OnSourceDependencyPropertyChanged);
                break;
            case BindingMode.TwoWay:
                _onSourceDependencyPropertyChangedIndex = source.RegisterPropertyChangedCallback(sourceProperty, OnSourceDependencyPropertyChanged);
                _onTargetDependencyPropertyChangedIndex = target.RegisterPropertyChangedCallback(targetProperty, OnTargetDependencyPropertyChanged);
                break;
            case BindingMode.OneTime:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bindingMode), bindingMode, null);
        }

    }
    
    internal WorkaroundBinding(DependencyObject target, DependencyProperty targetProperty, 
        INotifyPropertyChanged source, string  sourcePropertyName, 
        BindingMode bindingMode, 
        IValueConverter? valueConverter, object? valueConverterProperty, string? valueConverterLanguage, 
        UpdateSourceTrigger updateSourceTrigger = UpdateSourceTrigger.Default,
        object? targetNullValue = null,
        object? fallbackValue = null, 
        [CallerFilePath] string? filePath = null, [CallerLineNumber] int lineNumber = -1)
    {
        if (updateSourceTrigger is UpdateSourceTrigger.Explicit or UpdateSourceTrigger.LostFocus)
            throw new ArgumentException($"UpdateSourceTrigger.Explicit and UpdateSourceTrigger.LostFocus are not supported in WorkaroundBinding. FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");
        
        if (source.GetProperty(sourcePropertyName) is not { } sourcePropertyInfo)
        {
            var msg = $"Property [{sourcePropertyName}] not found in class [{source}]. FilePath: [{_filePath}], LineNumber: [{_lineNumber}]";
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
            throw new ArgumentException(msg);
        }
        
        _sourcePropertyInfo = sourcePropertyInfo;
        
        if (!_sourcePropertyInfo.CanRead)
        {
            var msg = $"Property [{sourcePropertyName}], in class [{source}], cannot be read. FilePath: [{_filePath}], LineNumber: [{_lineNumber}]";
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
            throw new ArgumentException(msg);
        }
        
        TargetDependencyObject = target;
        TargetProperty = targetProperty;

        SourceNotifyPropertyChanged = source;
        _sourcePropertyName = sourcePropertyName;
        _sourcePropertyType = sourcePropertyInfo.PropertyType;

        _valueConverter = valueConverter;
        _valueConverterParameter = valueConverterProperty;
        _valueConverterLanguage = valueConverterLanguage ?? string.Empty;

        _targetNullValue = targetNullValue;
        _fallbackValue = fallbackValue;

        _filePath = filePath;
        _lineNumber = lineNumber;

        OnSourceNotifyPropertyChanged(source, new PropertyChangedEventArgs(sourcePropertyName));

        switch (bindingMode)
        {
            case BindingMode.OneTime:
                break;
            case BindingMode.OneWay:
                source.PropertyChanged += OnSourceNotifyPropertyChanged;
                break;
            case BindingMode.TwoWay:

                if (!_sourcePropertyInfo.CanWrite)
                    throw new ArgumentException($"Property [{sourcePropertyName}], in class [{source}], cannot be written to. FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");

                source.PropertyChanged += OnSourceNotifyPropertyChanged;
                _onTargetDependencyPropertyChangedIndex = target.RegisterPropertyChangedCallback(targetProperty, OnTargetDependencyPropertyChanged);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(bindingMode), bindingMode, $"FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");
        }

    }

    private void OnTargetDependencyPropertyChanged(DependencyObject? sender, DependencyProperty dp)
    {
        if (TargetDependencyObject is not { } target)
            return;
        if (TargetProperty is not { } targetProperty)
            return;

        try
        {
            var value = target.GetValue(targetProperty) ?? _targetNullValue;

            if (_valueConverter is { } converter)
                value = converter.ConvertBack(value, _sourcePropertyType, _valueConverterParameter, _valueConverterLanguage);

            try
            {
                if (_sourceProperty is { } sourceProperty)
                    SourceDependencyObject?.SetValue(sourceProperty, value);

                if (SourceNotifyPropertyChanged is { } source)
                    _sourcePropertyInfo?.SetValue(source, value);
            }            
            catch (Exception e)
            {
                // We get here because the INotifiableProperty source is no longer valid
                _sourceFailed = true;
                Dispose();
                QLog.Error(e, $"FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");
            }
        }
        catch (Exception e)
        {
            // We get here because the TargetDependencyObject or ValueConverter is no longer valid
            _targetFailed = true;
            Dispose();
            QLog.Error(e, $"FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");
        }
    }

    private void OnSourceDependencyPropertyChanged(DependencyObject? sender, DependencyProperty dp)
    {
        if (SourceDependencyObject is null)
            return;
        if (!ReferenceEquals(sender, SourceDependencyObject))
            return;
        if (dp != _sourceProperty)
            return;
        if (TargetDependencyObject is null)
            return;
        if (TargetProperty is null)
            return;

        try
        {
            var value = _fallbackValue;
            try
            {
                value =  SourceDependencyObject.GetValue(_sourceProperty);
            }
            catch (Exception)
            {
                // ignored
            }

            if (_valueConverter != null)
                value = _valueConverter.Convert(value, _targetPropertyType, _valueConverterParameter, _valueConverterLanguage);

            try
            {
                if (value is null)
                    TargetDependencyObject.ClearValue(TargetProperty);
                else
                    TargetDependencyObject.SetValue(TargetProperty, value);
            }            
            catch (Exception e)
            {
                // We get here because the TargetDependencyObject is no longer valid
                _targetFailed = true;
                Dispose();
                QLog.Error(e, $"FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");
            }
        }
        catch (Exception e)
        {
            // We get here because the SourceDependencyObject or ValueConverter is no longer valid
            _sourceFailed = true;
            Dispose();
            QLog.Error(e, $"FilePath: [{_filePath}], LineNumber: [{_lineNumber}]");
        }
    }

    private void OnSourceNotifyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != _sourcePropertyName)
            return;
        if (TargetDependencyObject is null)
            return;
        if (TargetProperty is null)
            return;
        if (SourceNotifyPropertyChanged is not { } source)
            return;
        if (_sourcePropertyInfo is null)
            return;

        try
        {
            var value = _fallbackValue;
            try
            {
                value = _sourcePropertyInfo.GetValue(source, null);
            }
            catch (Exception)
            {
                // ignored
            }
            
            if (_valueConverter != null)
                value = _valueConverter.Convert(value, _targetPropertyType, _valueConverterParameter, _valueConverterLanguage);

            try
            {
                if (value is null)
                    TargetDependencyObject.ClearValue(TargetProperty);
                else
                    TargetDependencyObject.SetValue(TargetProperty, value);
            }            
            catch (Exception)
            {
                // We get here because the SourceDependencyObject is no longer valid
                _targetFailed = true;
                Dispose();
            }
        }
        catch (Exception)
        {
            // We get here because the SourceDependencyObject is no longer valid
            _sourceFailed = true;
            Dispose();
        }
    }

    private bool _sourceFailed;
    private bool _targetFailed;
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (!disposing)
            return;

        _isDisposed = true;
        
        if (!_sourceFailed)
        {
            if (_onSourceDependencyPropertyChangedIndex > -1)
                SourceDependencyObject?.UnregisterPropertyChangedCallback(_sourceProperty, _onSourceDependencyPropertyChangedIndex);
            if (SourceNotifyPropertyChanged is { } source)
                source.PropertyChanged -= OnSourceNotifyPropertyChanged;
        }

        if (!_targetFailed)
        {
            if (_onTargetDependencyPropertyChangedIndex > -1) 
                TargetDependencyObject?.UnregisterPropertyChangedCallback(TargetProperty, _onTargetDependencyPropertyChangedIndex);
            if (TargetProperty is not null)
                TargetDependencyObject?.WUnbind(TargetProperty);
        }


        _sourceWeakReference = null;
        _targetWeakReference = null;

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~WorkaroundBinding()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }
}

/*
internal enum WorkaroundBindingSourceType
{
    Value,
    DependencyProperty,
    INotifyPropertyChanged,
}
*/
