using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using P42.Serilog.QuickLog;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
/// <summary>
/// Build off-line caching for properties of objects that are descendents
/// </summary>
public class CacheableNotifiablePropertyObject : NotifiableObject.SelfBackedNotifiablePropertyObject
{
    
    private string? _instanceIdentifier;
    /// <summary>
    /// Unique name given to object instance, required recall values in later app session.
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    public string InstanceIdentifier
    {
        get => _instanceIdentifier ?? throw new ArgumentNullException(nameof(InstanceIdentifier));
        protected set
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            if (value == _instanceIdentifier) return;

            if (!string.IsNullOrWhiteSpace(_instanceIdentifier))
                throw new Exception($"Cannot change InstanceIdentifier of {GetType().Name}");

            _instanceIdentifier = value;
        }
    }

    [JsonIgnore]
    private readonly List<string> _initializedProperties = [];
    
    private LocalData.TagItem TagItemKey(string propertyName) => LocalData.TagItem.Get(propertyName, Path.Combine(GetType().Name, InstanceIdentifier), GetType().Assembly);

    /// <summary>
    /// Overrides Serialization Settings for Property
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="settings"></param>
    public void SetJsonSerializatingSettings(string propertyName, JsonSerializerSettings settings)
        => TagItemKey(propertyName).JsonSerializatingSettings = settings;

    /// <summary>
    /// Overrides Deserialization Settings for Proeprty
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="settings"></param>
    public void SetJsonDeserializationSettings(string propertyName, JsonSerializerSettings settings)
        => TagItemKey(propertyName).JsonDeserializingSettings = settings;

    /// <summary>
    /// Recall cached value (or default)
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="defaultValue"></param>
    /// <param name="propertyName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected T? GetCachedValue<T>(string instanceId, T? defaultValue = default, [CallerMemberName] string propertyName = "")
    {
        InstanceIdentifier = instanceId; 
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException(DebugExtensions.NameOfCallingClass());
        
        if (_initializedProperties.Contains(propertyName))
            return GetValue(defaultValue, propertyName);

        if (TagItemKey(propertyName).TryDeserialize<T>(out var value))
            return value;

        _initializedProperties.Add(propertyName);
        return GetValue(defaultValue, propertyName);
    }

    /// <summary>
    ///  Set cached value
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="value"></param>
    /// <param name="propertyName"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    protected bool SetCachedValue<T>(string instanceId, T value, [CallerMemberName] string propertyName = "")
    {
        InstanceIdentifier = instanceId; 
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException(DebugExtensions.NameOfCallingClass());
        
        if (!SetValue(value, propertyName))
            return false;

        if (!_initializedProperties.Contains(propertyName))
            _initializedProperties.Add(propertyName);

        return TagItemKey(propertyName).TrySerialize(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="instanceId"></param>
    /// <param name="propertyName"></param>
    /// <typeparam name="T"></typeparam>
    protected void Cache<T>(T value, string instanceId, string propertyName)
    {
        InstanceIdentifier = instanceId; 
        if (string.IsNullOrEmpty(propertyName))
            throw new ArgumentNullException(DebugExtensions.NameOfCallingClass());
        
        if (!_initializedProperties.Contains(propertyName))
            _initializedProperties.Add(propertyName);

        TagItemKey(propertyName).Serialize(value);

    }
}
