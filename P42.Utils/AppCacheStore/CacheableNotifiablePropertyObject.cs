

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace P42.Utils
{
    public class CacheableNotifiablePropertyObject : P42.NotifiableObject.SelfBackedNotifiablePropertyObject
    {
        string _folderName;
        string FolderName => _folderName = _folderName ?? GetType().FullName;

        [JsonIgnore]
        private List<string> InitializedProperties = new List<string>();


        protected T GetCachedValue<T>(T defaultValue = default, [CallerMemberName] string propertyName = null)
        {
            if (InitializedProperties.Contains(propertyName))
                return GetValue(defaultValue, propertyName);

            if (TextCache.Recall(propertyName, FolderName) is string json && !string.IsNullOrEmpty(json))
            {
                try
                {
                    var value = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                    {
                        Formatting = Formatting.Indented,
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                        TypeNameHandling = TypeNameHandling.All
                    });
                    SetValue(value, propertyName);
                    InitializedProperties.Add(propertyName);
                    return value;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"CacheableNotifiablePropertyObject.GetCachedValue<{typeof(T)}> : EXCEPTION : {ex}");
                    Console.WriteLine($"CacheableNotifiablePropertyObject.GetCachedValue<{typeof(T)}> : EXCEPTION : {ex}");
                }
            }

            InitializedProperties.Add(propertyName);
            return GetValue(defaultValue, propertyName);
        }

        protected bool SetCachedValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            if (SetValue(value, propertyName))
            {
                if (!InitializedProperties.Contains(propertyName))
                    InitializedProperties.Add(@propertyName);

                var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    TypeNameHandling = TypeNameHandling.All
                });
                TextCache.Store(json, propertyName, FolderName);

                return true;
            }
            return false;
        }

        protected void Cache<T>(T value, string propertyName)
        {
            if (!InitializedProperties.Contains(propertyName))
                InitializedProperties.Add(@propertyName);

            var json = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.All
            });
            TextCache.Store(json, propertyName, FolderName);

        }
    }
}