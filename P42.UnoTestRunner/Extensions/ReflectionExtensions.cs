using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

internal static class ReflectionExtensions
{
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

    public static PropertyInfo? GetProperty(this object obj, string propertyName)
    => string.IsNullOrWhiteSpace(propertyName)
        ? null
        : GetPropertyInfo(obj.GetType(), propertyName);

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


    public static FieldInfo? GetFieldInfo(this Type type, string fieldName)
    {
        if (string.IsNullOrEmpty(fieldName))
            return null;

        FieldInfo? fieldInfo = null;
        Type? t = type;
        do
        {
            if (t.GetRuntimeFields() is { } fields)
                foreach (var field in fields)
                    if (field.Name == fieldName)
                        fieldInfo = field;
            t = t.GetTypeInfo().BaseType;
        } while (fieldInfo == null && t != null);

        return fieldInfo;
    }

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
        if (GetPropertyInfo(type, propertyName) is not { CanRead: true } propInfo)
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
        if (GetPropertyInfo(type, propertyName) is not { CanWrite: true } propInfo)
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

    /// <summary>
    /// Get value of field
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="fieldName">Name of field</param>
    /// <param name="value">Value</param>
    /// <returns>true on success</returns>
    public static bool TryGetFieldValue(this object obj, string fieldName, out object? value)
    {
        value = null;

        if (string.IsNullOrWhiteSpace(fieldName))
            return false;

        if (GetFieldInfo(obj.GetType(), fieldName) is not { } fieldInfo)
            return false;

        value = fieldInfo.GetValue(obj);
        return true;
    }

    /// <summary>
    /// Set field value
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="fieldName">Field name</param>
    /// <param name="value">value to set</param>
    /// <returns>true on success</returns>
    public static bool TrySetFieldValue(this object obj, string fieldName, object value)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
            return false;

        if (GetFieldInfo(obj.GetType(), fieldName) is not { } fieldInfo)
            return false;

        fieldInfo.SetValue(obj, value);
        return true;
    }

    /// <summary>
    /// Get static field value
    /// </summary>
    /// <param name="type">Class type</param>
    /// <param name="fieldName">Field name</param>
    /// <param name="value">value</param>
    /// <returns>true on success</returns>
    public static bool TryGetStaticFieldValue(this Type type, string fieldName, out object? value)
    {
        value = null;
        if (GetFieldInfo(type, fieldName) is not { IsStatic: true } fieldInfo)
            return false;

        try
        {
            value = fieldInfo.GetValue(null);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

}
