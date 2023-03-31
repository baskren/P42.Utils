

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace P42.Utils
{
    public class SelfBackedCacheableNotifiablePropertyObject : P42.NotifiableObject.SelfBackedNotifiablePropertyObject
    {
        string _folderName;
        string FolderName => _folderName = _folderName ?? GetType().FullName;

        [JsonIgnore]
        private List<string> InitializedProperties = new List<string>();


        protected T GetCachedValue<T>(T defaultValue = default, [CallerMemberName] string propertyName = null)
        {
            if (InitializedProperties.Contains(propertyName))
                return GetValue<T>(defaultValue, propertyName);

            if (TextCache.Recall(propertyName, FolderName) is string json && !string.IsNullOrEmpty(json))
            {
                try
                {
                    var value = JsonConvert.DeserializeObject<T>(json);
                    InitializedProperties.Add(propertyName);
                    return value;
                }
                catch (Exception)
                {

                }
            }

            InitializedProperties.Add(propertyName);
            return defaultValue;
        }


        protected bool SetCachedValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (SetValue(value, propertyName))
            {
                if (!InitializedProperties.Contains(propertyName))
                    InitializedProperties.Add(@propertyName);

                var json = JsonConvert.SerializeObject(value);
                TextCache.Store(json, propertyName, FolderName);

                return true;
            }
            return false;
        }
    }
}