using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace P42.Utils.Uno
{
    public class NotifiableProperty
    {
        public string PropertyName { get; private set; }

        public Type PropertyType { get; private set; }

        public Type ObjectType { get; private set; }

        public object DefaultValue { get; private set; }

        private NotifiableProperty() { }

        public static NotifiableProperty Register(string propertyName, Type propertyType, Type objectType, object defaultValue)
        {
            return new NotifiableProperty
            {
                PropertyName = propertyName,
                PropertyType = propertyType,
                ObjectType = objectType,
                DefaultValue = defaultValue
            };
            
        }
    }
}
