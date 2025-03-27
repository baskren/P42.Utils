using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class PropertyExtensions
{
    
    /// <summary>
    /// Get PropertyInfo
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="propertyName">Property Name</param>
    /// <returns>null if no match found</returns>
    public static PropertyInfo? GetPropertyInfo(this Type type, string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return null;
            
        PropertyInfo? propInfo = null;
        var t = type;
        do
        {
            if (t.GetRuntimeProperties() is { } properties)
                foreach (var property in properties)
                    if (property.Name == propertyName)
                        propInfo = property;
            t = t.GetTypeInfo().BaseType;
        } while (propInfo == null && t != null);
            
        return propInfo;
    }

    /// <summary>
    /// Get PropertyInfo
    /// </summary>
    /// <param name="obj">Class Instance</param>
    /// <param name="propertyName">Property Name</param>
    /// <returns>null if not found</returns>
    public static PropertyInfo? GetProperty(this object obj, string propertyName)
        => string.IsNullOrWhiteSpace(propertyName) 
            ? null 
            : GetPropertyInfo(obj.GetType(), propertyName);
        
    /// <summary>
    /// Gets all properties for class instance
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <returns>Properties</returns>
    public static IEnumerable<PropertyInfo> GetProperties(this object obj)
        => obj.GetType().GetRuntimeProperties();

    /// <summary>
    /// Gets all property names for a class instance
    /// </summary>
    /// <param name="obj">Names</param>
    /// <returns>Property Names</returns>
    public static IEnumerable<string> GetPropertyNames(this object obj)
        => obj.GetProperties().Select(property => property.Name);
        
    /// <summary>
    /// Tests if property exists in a class instance
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="propertyName">Property Name</param>
    /// <returns>true/false</returns>
    public static bool PropertyExists(this object obj, string propertyName)
        => GetProperty(obj, propertyName) != null;
        
    /// <summary>
    /// Get property value
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    /// <returns>true on success</returns>
    public static bool TryGetPropertyValue(this object obj, string propertyName, out object? value)
    {
        value = null;
        
        if (string.IsNullOrWhiteSpace(propertyName))
            return false;

        if (GetProperty(obj, propertyName) is not { CanRead: true } propInfo)
            return false;

        value = propInfo.GetValue(obj, null);
        return true;
    }

    /// <summary>
    /// Set property value
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    /// <returns>true on success</returns>
    public static bool TrySetPropertyValue(this object obj, string propertyName, object? value)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return false;

        if (GetPropertyInfo(obj.GetType(), propertyName) is not { CanWrite: true } propInfo)
            return false;
        
        propInfo.SetValue(obj, value, null);
        return true;
    }

    /// <summary>
    /// Get static property value
    /// </summary>
    /// <param name="type">class type</param>
    /// <param name="propertyName">property name</param>
    /// <param name="value">value</param>
    /// <returns>true on success</returns>
    public static bool TryGetStaticPropertyValue(this Type type, string propertyName, out object? value)
    {
        value = null;
        if (GetPropertyInfo(type, propertyName) is not { CanRead:true } propInfo)
            return false;

        try
        {
            value = propInfo.GetValue(null);
        }
        catch (Exception)
        {
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Set static property value
    /// </summary>
    /// <param name="type">class type</param>
    /// <param name="propertyName">property name</param>
    /// <param name="value">value</param>
    /// <returns>true on success</returns>
    public static bool TrySetStaticPropertyValue(this Type type, string propertyName, object? value)
    {
        if (GetPropertyInfo(type, propertyName) is not { CanWrite:true } propInfo)
            return false;

        try
        {
            propInfo.SetValue(null, value);
        }
        catch (Exception)
        {
            return false;
        }
        
        return true;
    }
    
}
