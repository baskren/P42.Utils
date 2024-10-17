using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils;

public class CacheableNotifiablePropertyObject : NotifiableObject.SelfBackedNotifiablePropertyObject
{
    private string? _folderName;
    private string FolderName => _folderName ??= GetType().FullName ?? string.Empty;

    [JsonIgnore]
    private readonly List<string> _initializedProperties = [];


    protected T? GetCachedValue<T>(T? defaultValue = default, [CallerMemberName] string? propertyName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        if (_initializedProperties.Contains(propertyName))
            return GetValue(defaultValue, propertyName);

        if (TextCache.Recall(propertyName, FolderName) is { } json && !string.IsNullOrEmpty(json))
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
                _initializedProperties.Add(propertyName);
                return value;
            }
            catch (Exception ex)
            {
                QLog.Error($"CacheableNotifiablePropertyObject.GetCachedValue<{typeof(T)}> : EXCEPTION : {ex}");
            }
        }

        _initializedProperties.Add(propertyName);
        return GetValue(defaultValue, propertyName);
    }

    protected bool SetCachedValue<T>(T value, [CallerMemberName] string? propertyName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        if (!SetValue(value, propertyName))
            return false;
        
        if (!_initializedProperties.Contains(propertyName))
            _initializedProperties.Add(propertyName);

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

    protected void Cache<T>(T value, string propertyName)
        => SetCachedValue(value, propertyName);
}
