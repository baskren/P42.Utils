using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace P42.Utils.Uno
{
    public class DataTemplateSetSelector : Windows.UI.Xaml.Controls.DataTemplateSelector, IDictionary<Type, DataTemplate>
    {
        
        DataTemplateSet<SelectorItem> NullTemplateSet { get; set; } = new DataTemplateSet<SelectorItem>(null, null);

        public DataTemplate NullTemplate
        {
            get => NullTemplateSet.Template;
            set => NullTemplateSet.Template = value;
        }

        DataTemplateSet<SelectorItem> NoMatchTemplateSet { get; set; } = new DataTemplateSet<SelectorItem>(typeof(object), null);

        public DataTemplate NoMatchTemplate
        {
            get => NullTemplateSet.Template;
            set => NullTemplateSet.Template = value;
        }


        public virtual IEnumerable<DataTemplate> Templates => ItemTemplateSets.Values.Select(s=>s.Template);

        Dictionary<Type, DataTemplate> CachedTemplates = new Dictionary<Type, DataTemplate>();

        protected Dictionary<Type, DataTemplateSet<SelectorItem>> ItemTemplateSets;

        public DataTemplateSetSelector()
        {
            ItemTemplateSets = new Dictionary<Type, DataTemplateSet<SelectorItem>>();
        }

        public DataTemplateSetSelector(Dictionary<Type, DataTemplateSet<SelectorItem>> itemTemplates)
        {
            ItemTemplateSets = new Dictionary<Type, DataTemplateSet<SelectorItem>>(itemTemplates);
        }

        public DataTemplateSetSelector(DataTemplateSetSelector selector)
        {
            ItemTemplateSets = new Dictionary<Type, DataTemplateSet<SelectorItem>>(selector.ItemTemplateSets);
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            var type = item.GetType();
            if (CachedTemplates.TryGetValue(type, out DataTemplate template))
                return template;
            if (SelectDataTemplateSet(type) is DataTemplateSet<SelectorItem> templateItem)
            {
                CachedTemplates.Add(type, templateItem.Template);
                return templateItem.Template;
            }
            return null;
        }

        protected virtual DataTemplateSet<SelectorItem> SelectDataTemplateSet(Type type)
        {
            if (type is null)
                return NullTemplateSet;
            //var typeString = type.ToString();
            if (ItemTemplateSets.TryGetValue(type, out DataTemplateSet<SelectorItem> exactMatch))
                return exactMatch;
            if (type.IsConstructedGenericType)
            {
                var genericSourceType = type.GetGenericTypeDefinition();
                //var genericTypeString = genericSourceType.ToString();
                if (ItemTemplateSets.TryGetValue(genericSourceType, out DataTemplateSet<SelectorItem> genericMatch))
                    return genericMatch;
            }
            var baseType = type.BaseType;
            if (baseType != null)
                return SelectDataTemplateSet(baseType);
            else
                return NoMatchTemplateSet;
        }

        public DataTemplate this[Type key]
        {
            get
            {
                if (ItemTemplateSets.TryGetValue(key, out DataTemplateSet<SelectorItem> value))
                    return value.Template;
                return null;
            }
            set
            {
                if (value != null)
                    ItemTemplateSets[key] = new DataTemplateSet<SelectorItem>(key, value);
                else if (ItemTemplateSets.ContainsKey(key))
                    ItemTemplateSets.Remove(key);
            }
        }


        public ICollection<Type> Keys => ItemTemplateSets.Keys;

        public ICollection<DataTemplate> Values => ItemTemplateSets.Values.Select(s=>s.Template).ToList();

        public int Count => ItemTemplateSets.Count;

        public bool IsReadOnly => false;

        public void AddGeneric(Type key, DataTemplate value)
        {
            if (key.IsGenericType)
                key = key.GetGenericTypeDefinition();
            Add(key, value);
        }

        public void Add(Type key, DataTemplate value)
        {
            if (key is null)
            {
                NullTemplate = value;
                return;
            }
            if (value != null)
                ItemTemplateSets[key] = new DataTemplateSet<SelectorItem>(key, value);
            else if (ItemTemplateSets.ContainsKey(key))
                ItemTemplateSets.Remove(key);
        }

        public void Add(Type key, Type value)
            => Add(key, (DataTemplate)value?.AsDataTemplate());

        public void Add(KeyValuePair<Type, DataTemplate> item)
            => Add(item.Key, item.Value);

        public void Add(KeyValuePair<Type, Type> item)
            => Add(item.Key, item.Value);

        public void Clear()
            => ItemTemplateSets.Clear();

        public bool Contains(KeyValuePair<Type, DataTemplate> item)
            => ItemTemplateSets.Contains(item);

        public bool ContainsKey(Type key)
            => ItemTemplateSets.ContainsKey(key);

        public void CopyTo(KeyValuePair<Type, DataTemplate>[] array, int arrayIndex)
        => ItemTemplateSets.Select(s => new KeyValuePair<Type, DataTemplate>(s.Key, s.Value.Template)).ToArray().CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<Type, DataTemplate>> GetEnumerator()
            => ItemTemplateSets.Select(s => new KeyValuePair<Type, DataTemplate>(s.Key, s.Value.Template)).GetEnumerator();

        public bool Remove(Type key)
        {
            if (key is null && NullTemplate != null)
            {
                NullTemplate = null;
                return true;
            }
            else if (ItemTemplateSets.ContainsKey(key))
            {
                ItemTemplateSets.Remove(key);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<Type, DataTemplate> item)
        {
            if (item.Key is null && NullTemplate == item.Value)
            {
                NullTemplate = null;
                return true;
            }
            else if (TryGetValue(item.Key, out DataTemplate value) && item.Value == value)
            {
                ItemTemplateSets.Remove(item.Key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(Type key, out DataTemplate value)
        {
            if (key is null)
            {
                value = NullTemplate;
                return true;
            }
            if (ItemTemplateSets.TryGetValue(key, out DataTemplateSet<SelectorItem> set))
            {
                value = set.Template;
                return true;
            }
            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ItemTemplateSets.Select(s => new KeyValuePair<Type, DataTemplate>(s.Key, s.Value.Template)).GetEnumerator();
    }
}
