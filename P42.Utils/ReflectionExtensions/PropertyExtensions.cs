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
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    [JetBrains.Annotations.PublicAPI]
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
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static PropertyInfo? GetProperty(this object obj, string propertyName)
        => string.IsNullOrWhiteSpace(propertyName) 
            ? null 
            : obj.GetType().GetPropertyInfo(propertyName);
        
    /// <summary>
    /// Gets all properties for class instance
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <returns>Properties</returns>
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    [JetBrains.Annotations.PublicAPI]
    public static IEnumerable<PropertyInfo> GetProperties(this object obj)
        => obj.GetType().GetRuntimeProperties();

    /// <summary>
    /// Gets all property names for a class instance
    /// </summary>
    /// <param name="obj">Names</param>
    /// <returns>Property Names</returns>
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static IEnumerable<string> GetPropertyNames(this object obj)
        => obj.GetProperties().Select(property => property.Name);
        
    /// <summary>
    /// Tests if property exists in a class instance
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="propertyName">Property Name</param>
    /// <returns>true/false</returns>
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static bool PropertyExists(this object obj, string propertyName)
        =>
            obj.GetProperty(propertyName) != null;
        
    /// <summary>
    /// Get property value
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="propertyName">Property name</param>
    /// <param name="value">Value</param>
    /// <returns>true on success</returns>
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static bool TryGetPropertyValue(this object obj, string propertyName, out object? value)
    {
        value = null;
        
        if (string.IsNullOrWhiteSpace(propertyName))
            return false;

        if (obj.GetProperty(propertyName) is not { CanRead: true } propInfo)
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
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static bool TrySetPropertyValue(this object obj, string propertyName, object? value)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
            return false;

        if (obj.GetType().GetPropertyInfo(propertyName) is not { CanWrite: true } propInfo)
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
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static bool TryGetStaticPropertyValue(this Type type, string propertyName, out object? value)
    {
        value = null;
        if (type.GetPropertyInfo(propertyName) is not { CanRead:true } propInfo)
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
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static bool TrySetStaticPropertyValue(this Type type, string propertyName, object? value)
    {
        if (type.GetPropertyInfo(propertyName) is not { CanWrite:true } propInfo)
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
