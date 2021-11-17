using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace P42.Utils.Uno
{
    public class DataTemplateSet<T>
    {
        public Type DataType { get; set; }
        public DataTemplate Template { get; set; }
        public HashSet<T> RecycleStore = new HashSet<T>();

        public DataTemplateSet(Type dataType, DataTemplate template)
        {
            DataType = dataType;
            Template = template;
        }
    }
}