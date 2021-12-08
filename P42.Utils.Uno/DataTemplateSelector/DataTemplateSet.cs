using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace P42.Utils.Uno
{
    public class DataTemplateSet
    {
        public Type DataType { get; private set; }
        public Type TemplateType { get; private set; }
        public readonly HashSet<UIElement> RecycleStore = new HashSet<UIElement>();

        public Func<UIElement> Constructor { get; private set; }

        DataTemplate dataTemplate;
        public DataTemplate Template => dataTemplate = dataTemplate ?? TemplateType.AsDataTemplate();

        public DataTemplateSet(Type dataType, Type templateType, Func<UIElement> constructor = null)
        {
            DataType = dataType;

            TemplateType = templateType;

            Constructor = constructor;

            if (Constructor is null)
                Constructor = () => (UIElement)Activator.CreateInstance(TemplateType);
        }
    }
}