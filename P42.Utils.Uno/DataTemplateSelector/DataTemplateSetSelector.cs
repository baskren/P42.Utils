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
        NullDataTemplateSet NullTemplateSet { get; set; } = new NullDataTemplateSet();

        DataTemplateSet NoMatchTemplateSet { get; set; } = new NullDataTemplateSet();

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

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            //return base.SelectTemplateCore(item, container);
            //System.Console.WriteLine($"SelectTemplateCore ENTER {item} {container}");
            var result = SelectDataTemplateSet(item)?.Template;
            //System.Console.WriteLine($"SelectTemplateCore EXIT {item} {container}");
            return result;
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            //System.Console.WriteLine($"SelectTemplateCore ENTER {item}");
            var result = SelectDataTemplateSet(item)?.Template;
            //System.Console.WriteLine($"SelectTemplateCore EXIT {item}");
            return result;
        }

        public virtual DataTemplateSet SelectDataTemplateSet(object item)
        {
            //System.Console.WriteLine($"SelectDataTemplateSet ENTER {item}");
            var type = item?.GetType();
            if (type is null)
            {
                //System.Console.WriteLine($"SelectDataTemplateSet EXIT : A");
                return NullTemplateSet;
            }
            if (CachedTemplates.TryGetValue(type, out DataTemplateSet templateSet))
            {
                //System.Console.WriteLine($"SelectDataTemplateSet EXIT : B");
                return templateSet;
            }
            if (SelectDataTemplateSetCore(type) is DataTemplateSet templateSetItem)
            {
                //System.Console.WriteLine($"SelectDataTemplateSet EXIT : C");
                CachedTemplates.Add(type, templateSetItem);
                return templateSetItem;
            }
            //System.Console.WriteLine($"SelectDataTemplateSet EXIT : D");
            return null;
        }

        protected virtual DataTemplateSet SelectDataTemplateSetCore(Type type)
        {
            System.Console.WriteLine($"SelectDataTemplateSetCore ENTER {type.Name}");
            if (type is null)
            {
                System.Console.WriteLine($"SelectDataTemplateSetCore EXIT : A {type.Name}");
                return NullTemplateSet;
            }
            //var typeString = type.ToString();
            if (ItemTemplateSets.TryGetValue(type, out DataTemplateSet exactMatch))
            {
                System.Console.WriteLine($"SelectDataTemplateSetCore EXIT : B {type.Name}");
                return exactMatch;
            }
            if (type.IsConstructedGenericType)
            {
                var genericSourceType = type.GetGenericTypeDefinition();
                //var genericTypeString = genericSourceType.ToString();
                if (ItemTemplateSets.TryGetValue(genericSourceType, out DataTemplateSet genericMatch))
                {
                    System.Console.WriteLine($"SelectDataTemplateSetCore EXIT : C {type.Name}");
                    return genericMatch;
                }
            }
            var baseType = type.BaseType;
            if (baseType != null)
            {
                var result = SelectDataTemplateSetCore(baseType);
                System.Console.WriteLine($"SelectDataTemplateSetCore EXIT : D {type.Name}");
                return result;
            }
            System.Console.WriteLine($"SelectDataTemplateSetCore EXIT : E {type.Name}");
            return NoMatchTemplateSet;
        }

        public DataTemplateSet this[Type key]
        {
            get
            {
                if (key is null)
                    return NullTemplateSet;
                if (ItemTemplateSets.TryGetValue(key, out DataTemplateSet value))
                    return value;
                return NoMatchTemplateSet;
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
                NullTemplateSet = new NullDataTemplateSet(set.TemplateType, set.Constructor);
                if (set.Template != null)
                    NullTemplateSet.Template = set.Template;
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
                NullTemplateSet = new NullDataTemplateSet();
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
            if (item.Key is null && NullTemplateSet.TemplateType == item.Value.TemplateType)
            {
                NullTemplateSet = new NullDataTemplateSet();
                return true;
            }
            else if (TryGetValue(item.Key, out DataTemplateSet value) && item.Value.TemplateType == value.TemplateType)
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
