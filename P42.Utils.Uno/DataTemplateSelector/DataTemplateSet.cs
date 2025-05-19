using System;
using System.Collections.Generic;
using Microsoft.UI;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

public interface IDataTemplateSet
{
    Type DataType { get; }
    Type TemplateType { get; }
    Func<UIElement> Constructor { get; }
    
    DataTemplate Template { get;  }
}

/// <summary>
/// Container for using a FrameworkElement's Type as DataTemplate, paired with a Type to be used for the DataBinding  
/// </summary>
/// <param name="dataType"></param>
/// <param name="templateType"></param>
/// <param name="constructor">recommended constructor lambda for improved performance</param>
public class DataTemplateSet(Type dataType, Type templateType, Func<UIElement> constructor) : IDataTemplateSet
{
    /// <summary>
    /// Type that will be bound to Template 
    /// </summary>
    public Type DataType { get; } = dataType;

    /// <summary>
    /// Type used for Template bound to Data
    /// </summary>
    public Type TemplateType { get; } = templateType;

    public readonly HashSet<UIElement> RecycleStore = [];

    /// <summary>
    /// Optional constructor, for better performance over Activator.CreateInstance()
    /// </summary>
    public Func<UIElement> Constructor { get; } = constructor;

    private DataTemplate? _dataTemplate;
    public DataTemplate Template
    {
        get => _dataTemplate ??= TemplateType.AsDataTemplate() ?? throw new Exception("Cannot generate data template for " + TemplateType);
        internal set => _dataTemplate = value;
    }
}

/// <summary>
/// Container for using a FrameworkElement's Type as DataTemplate, paired with a Type to be used for the DataBinding  
/// </summary>
/// <param name="constructor">>recommended constructor lambda for improved performance.
/// default: () => (FrameworkElement)(templateType is null ? new NullView() : Activator.CreateInstance(templateType))</param>
/// <typeparam name="TData">Type for Data</typeparam>
/// <typeparam name="TTemplate">Type of DataTemplate</typeparam>
public class DataTemplateSet<TData, TTemplate>(Func<TTemplate>? constructor = null) 
    : DataTemplateSet(typeof(TData), typeof(TTemplate), constructor ??= () => new())  
    where TTemplate : FrameworkElement, new();

public interface INullDataTemplateSet : IDataTemplateSet;

/// <summary>
/// 
/// Template set used to set view to be used with null data
/// </summary>
/// <param name="constructor">>recommended constructor lambda for improved performance.
/// default: () => (FrameworkElement)(templateType is null ? new NullView() : Activator.CreateInstance(templateType))</param>
/// <typeparam name="TTemplate">Type for DataTemplate</typeparam>
public class NullDataTemplateSet<TTemplate>(Func<TTemplate>? constructor = null)
    : DataTemplateSet<NullView, TTemplate>(constructor ??= () => new()), INullDataTemplateSet
    where TTemplate : FrameworkElement, new();

/// <summary>
/// Default DataTemplateSet for .NullTemplateSet and .NoMatchTemplateSet in DataTemplateSetSelector
/// </summary>
public class DefaultNullDataTemplateSet() : NullDataTemplateSet<NullView>(() => new NullView());

/// <summary>
/// Default view returned by DataTemplateSetSelector for null DataContext
/// </summary>
public partial class NullView : Microsoft.UI.Xaml.Controls.Grid
{
    public NullView()
        => Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Colors.Gray);
}
