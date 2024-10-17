using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

public class DataTemplateSet
{

    public Type DataType { get; protected set; }
    public Type TemplateType { get; protected set; }
    
    public readonly HashSet<UIElement> RecycleStore = new ();

    public Func<UIElement>? Constructor { get; protected set; }

    private DataTemplate? _dataTemplate;
    public DataTemplate Template
    {
        get => _dataTemplate ??= TemplateType.AsDataTemplate();
        internal set => _dataTemplate = value;
    }

    public DataTemplateSet(Type dataType, Type templateType, Func<UIElement>? constructor = null)
    {
        DataType = dataType;
        TemplateType = templateType;
        constructor ??= () => (UIElement)Activator.CreateInstance(TemplateType);
        Constructor = constructor;
    }
}

public class NullDataTemplateSet : DataTemplateSet
{
    public NullDataTemplateSet(Type? templateType = null, Func<UIElement> constructor = null) : base (typeof(object), typeof(object), constructor)
    {
        DataType = null;
        TemplateType = templateType;
    }
}
