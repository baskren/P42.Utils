using System.Reflection;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class FieldExtensions
{
    /// <summary>
    /// Get Field Ino
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="fieldName">Field Name</param>
    /// <returns>null of no match found</returns>
    [Obsolete("NOT FOR RELEASE BUILDS")]
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


    /// <summary>
    /// Get value of field
    /// </summary>
    /// <param name="obj">Class instance</param>
    /// <param name="fieldName">Name of field</param>
    /// <param name="value">Value</param>
    /// <returns>true on success</returns>
    [Obsolete("NOT FOR RELEASE BUILDS")]
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
    [Obsolete("NOT FOR RELEASE BUILDS")]
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
    [Obsolete("NOT FOR RELEASE BUILDS")]
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
