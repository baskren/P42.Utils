using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.UI.Xaml;
using P42.Serilog.QuickLog;
using Windows.UI.Notifications;

namespace P42.Utils.Uno;

/// <summary>
/// A DataTemplateSelector that uses DataTemplateSets in order to enable a little bit more function and, optionally, performance
/// </summary>
// ReSharper disable once UnusedType.Global
public class DataTemplateSetSelector : Microsoft.UI.Xaml.Controls.DataTemplateSelector, IDictionary<Type, IDataTemplateSet>
{
    #region Properties

    /// <summary>
    /// DataTemplate to be used if the data item is null
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public INullDataTemplateSet NullTemplateSet { get; set; } = new DefaultNullDataTemplateSet();

    /// <summary>
    /// DataTemplate to be used if there isn't a matching DataTemplateSet stored in this DataTEemplateSelector
    /// </summary>
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public INullDataTemplateSet? NoMatchTemplateSet { get; set; }


    /// <summary>
    /// Default constructor
    /// </summary>
    public DataTemplateSetSelector()
    {
        ItemTemplateSets = new Dictionary<Type, IDataTemplateSet>();
        _cachedTemplates = new Dictionary<Type, IDataTemplateSet>();
    }

    /// <summary>
    /// Constructor using external dictionary of type to DataTemplateSets
    /// </summary>
    /// <param name="itemTemplates"></param>
    public DataTemplateSetSelector(Dictionary<Type, IDataTemplateSet> itemTemplates)
    {
        ItemTemplateSets = new Dictionary<Type, IDataTemplateSet>(itemTemplates);
        _cachedTemplates = new Dictionary<Type, IDataTemplateSet>();
    }

    /// <summary>
    /// Constructor using external DataTemplateSetSelector 
    /// </summary>
    /// <param name="selector"></param>
    public DataTemplateSetSelector(DataTemplateSetSelector selector)
    {
        ItemTemplateSets = new Dictionary<Type, IDataTemplateSet>(selector.ItemTemplateSets);
        _cachedTemplates = new Dictionary<Type, IDataTemplateSet>(selector._cachedTemplates);
    }
    
    #endregion

    
    #region Fields

    private readonly Dictionary<Type, IDataTemplateSet> _cachedTemplates;

    protected readonly Dictionary<Type, IDataTemplateSet> ItemTemplateSets;

    #endregion

    
    #region Get View / Get Template
    /// <summary>
    /// Gets a UIElement for given data item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    // ReSharper disable once InconsistentNaming
    public UIElement? GetUIElement(object? item)
    {
        UIElement? element = null;
        try
        {
            var set = SelectDataTemplateSet(item);
            element = set?.Constructor?.Invoke();
            return element;
        }
        catch (Exception e)
        {
            QLog.Error(e);
        }

        return element;
    }

    protected override DataTemplate SelectTemplateCore(object? item, DependencyObject container)
        => SelectDataTemplateSet(item).Template;

    protected override DataTemplate SelectTemplateCore(object? item)
        => SelectDataTemplateSet(item).Template;

    /// <summary>
    /// Find the DataTemplateSet for a given object
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    // ReSharper disable once MemberCanBeProtected.Global
    public virtual IDataTemplateSet SelectDataTemplateSet(object? item)
    {
        if (item is null)
            return NullTemplateSet;

        var type = item.GetType();
        if (_cachedTemplates.TryGetValue(type, out var templateSet))
            return templateSet;

        if (SelectDataTemplateSetCore(type) is not { } templateSetItem)
            return NoMatchTemplateSet;

        _cachedTemplates.Add(type, templateSetItem);
        return templateSetItem;
    }

    protected virtual IDataTemplateSet? SelectDataTemplateSetCore(Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (ItemTemplateSets.TryGetValue(type, out var exactMatch))
            return exactMatch;

        if (type.IsConstructedGenericType)
        {
            var genericSourceType = type.GetGenericTypeDefinition();
            if (ItemTemplateSets.TryGetValue(genericSourceType, out var genericMatch))
                return genericMatch;
        }

        var baseType = type.BaseType;
        return baseType != null 
            ? SelectDataTemplateSetCore(baseType) 
            : null;
    }
    #endregion


    #region IList / IDictionary support

    /// <summary>
    /// Dictionary of DataTemplateSets
    /// </summary>
    /// <param name="key"></param>
    public IDataTemplateSet this[Type key]
    {
        get => ItemTemplateSets.GetValueOrDefault(key, NoMatchTemplateSet);
        set => ItemTemplateSets[key] = value;
    }

    /// <summary>
    /// Index of a particular DataTemplateSet
    /// </summary>
    /// <param name="set"></param>
    /// <returns></returns>
    public int IndexOf(IDataTemplateSet set)
    {
        var values = ItemTemplateSets.Values.ToArray();
        return Array.IndexOf(values, set);
    }


    /// <summary>
    /// DataTypes stored
    /// </summary>
    public ICollection<Type> Keys => ItemTemplateSets.Keys;

    /// <summary>
    /// DataTemplateSets stored
    /// </summary>
    public ICollection<IDataTemplateSet> Values => ItemTemplateSets.Values;

    /// <summary>
    /// Count of DataTemplateSets stored
    /// </summary>
    public int Count => ItemTemplateSets.Count;

    /// <summary>
    /// Is this mutable?
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Add a DataTemplateSet
    /// </summary>
    /// <param name="set"></param>
    public DataTemplateSetSelector Add(IDataTemplateSet set)
    { Add(set.DataType, set); return this; }

    /// <summary>
    /// Add a DataTemplateSet
    /// </summary>
    /// <param name="key"></param>
    /// <param name="set"></param>
    public virtual void Add(Type key, IDataTemplateSet set)
        => ItemTemplateSets[key] = set;

    [Obsolete("Use .Add<TDataType, TTemplateType>(Func<UIElement>? constructor), instead", true)]
    public DataTemplateSetSelector Add(Type dataType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type templateType, Func<UIElement> constructor)
    {
        Add(new DataTemplateSet(dataType, templateType, constructor));
        return this;
    }


    [Obsolete("Use .Add<TDataType, TTemplateType>(Func<UIElement>? constructor), instead", true)]
    // ReSharper disable UnusedParameter.Global
    public DataTemplateSetSelector Add(Type dataType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type templateType)
    {
        var constructor = () => (UIElement)Activator.CreateInstance(templateType);            
        Add(new DataTemplateSet(dataType, templateType, constructor));
        return this;
    }
    // ReSharper restore UnusedParameter.Global

    /// <summary>
    /// Add a DataTemplateSet
    /// </summary>
    /// <param name="item"></param>
    public void Add(KeyValuePair<Type, IDataTemplateSet> item)
        => Add(item.Key, item.Value);

    /// <summary>
    /// Add a DataTemplateSet
    /// </summary>
    /// <param name="constructor"></param>
    /// <typeparam name="TDataType"></typeparam>
    /// <typeparam name="TTemplateType"></typeparam>
    public DataTemplateSetSelector Add<TDataType, TTemplateType>(Func<TTemplateType>? constructor = null) where TTemplateType : FrameworkElement, new()
    {
        Add(new DataTemplateSet<TDataType, TTemplateType>(constructor));
        return this;
    }
    


    /// <summary>
    /// Clear all DataTemplateSets
    /// </summary>
    public void Clear()
        => ItemTemplateSets.Clear();

    /// <summary>
    /// Is there a matching DataTemplateSet
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(KeyValuePair<Type, IDataTemplateSet> item)
        => ItemTemplateSets.Contains(item);

    /// <summary>
    /// Is there a matching DataType?
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(Type key)
        => ItemTemplateSets.ContainsKey(key);

    /// <summary>
    /// Copy DataTemplateSets to an array, at index
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<Type, IDataTemplateSet>[] array, int arrayIndex)
        => ItemTemplateSets.ToArray().CopyTo(array, arrayIndex);

    /// <summary>
    /// Enumerator for DataType, DataTemplateSets KVP
    /// </summary>
    /// <returns></returns>
    public IEnumerator<KeyValuePair<Type, IDataTemplateSet>> GetEnumerator()
        => ItemTemplateSets.GetEnumerator();

    /// <summary>
    /// Remove DataTemplateSet for DataType
    /// </summary>
    /// <param name="key">DataType</param>
    /// <returns></returns>
    public bool Remove(Type key)
        => ItemTemplateSets.Remove(key);
    

    /// <summary>
    /// Remove kvp from DataTemplateSets
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<Type, IDataTemplateSet> item)
    {
        if (!TryGetValue(item.Key, out var value) || item.Value.TemplateType != value.TemplateType)
            return false;

        ItemTemplateSets.Remove(item.Key);
        return true;
    }

    public bool TryGetValue(Type? key, out IDataTemplateSet value)
    {
        if (key is null)
        {
            value = NullTemplateSet;
            return false;
        }
        
        if (ItemTemplateSets.TryGetValue(key, out var set))
        {
            value = set;
            return true;
        }
        
        value = NoMatchTemplateSet;
        return false;
    }

    /// <summary>
    /// Enumerator for DataTemplateSets
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
        => ItemTemplateSets.Select(s => new KeyValuePair<Type, IDataTemplateSet>(s.Key, s.Value)).GetEnumerator();
    
        

    #endregion

}
