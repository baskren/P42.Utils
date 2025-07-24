using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno
{


    public class DataTemplateSet
    {

        public Type DataType { get; protected set; }
        public Type TemplateType { get; protected set; }
        public readonly HashSet<UIElement> RecycleStore = new HashSet<UIElement>();

        public Func<UIElement> Constructor { get; protected set; }

        DataTemplate dataTemplate;
        public DataTemplate Template
        {
            get
            {
                //System.Console.WriteLine($"get_DataTemplate ENTER {DataType} {TemplateType}");
                if (dataTemplate is null)
                {
                    //System.Console.WriteLine($"get_DataTemplate A {DataType} {TemplateType}");
                    dataTemplate = TemplateType?.AsDataTemplate();
                }
                //System.Console.WriteLine($"get_DataTemplate EXIT {DataType} {TemplateType} {dataTemplate}");
                return dataTemplate;
            }
            internal set => dataTemplate = value;
        }

        public DataTemplateSet(Type dataType, Type templateType, Func<UIElement> constructor = null)
        {
            DataType = dataType;
            if (dataType is null)
                throw new ArgumentNullException(nameof(dataType));

            TemplateType = templateType;
            if (templateType is null)
                throw new ArgumentNullException(nameof(templateType));

            Constructor = constructor;

            if (Constructor is null)
                Constructor = () =>
                {
                    if (TemplateType is null)
                        return null;

                    //var constructor = TemplateType.GetPublicConstructorInfo();
                    //return (UIElement)constructor?.Invoke(null);
                    return (UIElement)Activator.CreateInstance(TemplateType);
                };
        }
    }

    public class NullDataTemplateSet : DataTemplateSet
    {
        public NullDataTemplateSet(Type templateType = null, Func<UIElement> constructor = null) : base (typeof(object), typeof(object), constructor)
        {
            DataType = null;
            TemplateType = templateType;
        }
    }
}
