using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace P42.Utils.Uno
{
    public class DataTemplateSetSelector : Windows.UI.Xaml.Controls.DataTemplateSelector, IDictionary<Type, DataTemplateSet>
    {
        
        DataTemplateSet NullTemplateSet { get; set; } = new DataTemplateSet(null, null);

        /*
        public DataTemplateSet NullTemplate
        {
            get => NullTemplateSet.Template;
            set => NullTemplateSet.Template = value;
        }
        */

        DataTemplateSet NoMatchTemplateSet { get; set; } = new DataTemplateSet(typeof(object), null);

        /*
        public DataTemplate NoMatchTemplate
        {
            get => NullTemplateSet.Template;
            set => NullTemplateSet.Template = value;
        }
        */

        //public virtual IEnumerable<DataTemplate> Templates => ItemTemplateSets.Values.Select(s=>s.Template);

        Dictionary<Type, DataTemplateSet> CachedTemplates;

        protected Dictionary<Type, DataTemplateSet> ItemTemplateSets;

        public DataTemplateSetSelector()
        {
            ItemTemplateSets = new Dictionary<Type, DataTemplateSet>();
            CachedTemplates = new Dictionary<Type, DataTemplateSet>();
        }

        public DataTemplateSetSelector(Dictionary<Type, DataTemplateSet> itemTemplates)
        {
            ItemTemplateSets = new Dictionary<Type, DataTemplateSet>(itemTemplates);
        }

        public DataTemplateSetSelector(DataTemplateSetSelector selector)
        {
            ItemTemplateSets = new Dictionary<Type, DataTemplateSet>(selector.ItemTemplateSets);
            CachedTemplates = new Dictionary<Type, DataTemplateSet>(selector.CachedTemplates);
        }

        public UIElement GetUIElement(object item)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var set = SelectDataTemplateSet(item);
            var element = set.Constructor.Invoke();
            System.Diagnostics.Debug.WriteLine($"DataTemplateSet.GetUIElement: [{stopwatch.ElapsedMilliseconds}] [{item}]");
            stopwatch.Stop();
            return element;
        }

        protected override DataTemplate SelectTemplateCore(object item)
            => SelectDataTemplateSet(item)?.Template;

        public virtual DataTemplateSet SelectDataTemplateSet(object item)
        {
            var type = item?.GetType();
            if (type is null)
                return NullTemplateSet;
            if (CachedTemplates.TryGetValue(type, out DataTemplateSet templateSet))
                return templateSet;
            if (SelectDataTemplateSetCore(type) is DataTemplateSet templateSetItem)
            {
                CachedTemplates.Add(type, templateSetItem);
                return templateSetItem;
            }
            return null;
        }

        protected virtual DataTemplateSet SelectDataTemplateSetCore(Type type)
        {
            if (type is null)
                return NullTemplateSet;
            //var typeString = type.ToString();
            if (ItemTemplateSets.TryGetValue(type, out DataTemplateSet exactMatch))
                return exactMatch;
            if (type.IsConstructedGenericType)
            {
                var genericSourceType = type.GetGenericTypeDefinition();
                //var genericTypeString = genericSourceType.ToString();
                if (ItemTemplateSets.TryGetValue(genericSourceType, out DataTemplateSet genericMatch))
                    return genericMatch;
            }
            var baseType = type.BaseType;
            if (baseType != null)
                return SelectDataTemplateSet(baseType);
            else
                return NoMatchTemplateSet;
        }

        public DataTemplateSet this[Type key]
        {
            get
            {
                if (ItemTemplateSets.TryGetValue(key, out DataTemplateSet value))
                    return value;
                return null;
            }
            set
            {
                if (value != null)
                    ItemTemplateSets[key] = value;
                else if (ItemTemplateSets.ContainsKey(key))
                    ItemTemplateSets.Remove(key);
            }
        }


        public ICollection<Type> Keys => ItemTemplateSets.Keys;

        public ICollection<DataTemplateSet> Values => ItemTemplateSets.Values;

        public int Count => ItemTemplateSets.Count;

        public bool IsReadOnly => false;

        public void Add(Type key, DataTemplateSet set)
        {
            if (key is null)
            {
                NullTemplateSet = set;
                return;
            }
            if (set != null)
                ItemTemplateSets[key] = set;
            else if (ItemTemplateSets.ContainsKey(key))
                ItemTemplateSets.Remove(key);
        }

        public void Add(Type key, Type value, Func<UIElement> constructor = null)
            => Add(key, new DataTemplateSet(key, value, constructor));

        public void Add(KeyValuePair<Type, DataTemplateSet> item)
            => Add(item.Key, item.Value);

        public void Clear()
            => ItemTemplateSets.Clear();

        public bool Contains(KeyValuePair<Type, DataTemplateSet> item)
            => ItemTemplateSets.Contains(item);

        public bool ContainsKey(Type key)
            => ItemTemplateSets.ContainsKey(key);

        public void CopyTo(KeyValuePair<Type, DataTemplateSet>[] array, int arrayIndex)
            => ItemTemplateSets.ToArray().CopyTo(array, arrayIndex);

        public IEnumerator<KeyValuePair<Type, DataTemplateSet>> GetEnumerator()
            => ItemTemplateSets.GetEnumerator();

        public bool Remove(Type key)
        {
            if (key is null)
            {
                NullTemplateSet = null;
                return true;
            }
            else if (ItemTemplateSets.ContainsKey(key))
            {
                ItemTemplateSets.Remove(key);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<Type, DataTemplateSet> item)
        {
            if (item.Key is null && NullTemplateSet == item.Value)
            {
                NullTemplateSet = null;
                return true;
            }
            else if (TryGetValue(item.Key, out DataTemplateSet value) && item.Value == value)
            {
                ItemTemplateSets.Remove(item.Key);
                return true;
            }
            return false;
        }

        public bool TryGetValue(Type key, out DataTemplateSet value)
        {
            if (key is null)
            {
                value = NullTemplateSet;
                return true;
            }
            if (ItemTemplateSets.TryGetValue(key, out DataTemplateSet set))
            {
                value = set;
                return true;
            }
            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => ItemTemplateSets.Select(s => new KeyValuePair<Type, DataTemplateSet>(s.Key, s.Value)).GetEnumerator();
    }
}
