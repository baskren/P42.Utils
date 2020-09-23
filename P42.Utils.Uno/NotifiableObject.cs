using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Dynamic;
using Newtonsoft.Json;
using Windows.UI.Xaml;

namespace P42.Utils.Uno
{
    public class NotifiableObject : INotifyPropertyChanged
    {
        [JsonIgnore]
        public static long Instances { get; private set; }

        [JsonIgnore]
        public long InstanceId { get; private set; }

        [JsonIgnore]
        public virtual bool Logging { get; }

        protected List<string> BatchedPropertyChanges = new List<string>();

        [JsonIgnore]
        private Dictionary<NotifiableProperty, object> ObjectStore = new Dictionary<NotifiableProperty, object>();

        int _batchChanges;
        [JsonIgnore]
        public bool BatchChanges
        {
            get => _batchChanges > 0;
            set
            {
                if (value)
                    _batchChanges++;
                else
                    _batchChanges--;
                _batchChanges = Math.Max(0, _batchChanges);
                if (_batchChanges == 0)
                {
                    var propertyNames = new List<string>(BatchedPropertyChanges);
                    BatchedPropertyChanges.Clear();
                    foreach (var propertyName in propertyNames)
                        OnPropertyChanged(propertyName);
                }
            }
        }

        [JsonIgnore]
        public bool HasChanged { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
            => HasChanged = false;


        public NotifiableObject()
        {
            InstanceId = Instances++;
        }

        #region Property Change Handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //System.Diagnostics.Debug.WriteLineIf(Logging, "\t " + this + " NotifiablePropertyObject.OnPropertyChanged [" + propertyName + "]");
            //System.Diagnostics.Debug.WriteLineIf(Logging, "\t BatchChanges=[" + BatchChanges + "]");

            if (BatchChanges)
                BatchedPropertyChanges.Add(propertyName);
            else
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected object GetValue(NotifiableProperty property)
        {
            if (ObjectStore.TryGetValue(property, out object value))
                return value;
            return property.DefaultValue;
        }

        protected bool SetValue<T>(NotifiableProperty property, T value)
        {
            var current = (T)GetValue(property);
            if (EqualityComparer<T>.Default.Equals(current, value))
                return false;

            HasChanged = true;
            OnPropertyChanged(property.PropertyName);
            return true;
        }

        #endregion


    }
}
