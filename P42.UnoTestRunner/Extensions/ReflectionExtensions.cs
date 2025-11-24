using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace P42.UnoTestRunner;

internal static class ReflectionExtensions
{
    extension(object obj)
    {
        public bool TryGetPropertyValue(string propertyName, out object? value)
        {
            value = null;

            if (string.IsNullOrWhiteSpace(propertyName))
                return false;

            if (GetProperty(obj, propertyName) is not { CanRead: true } propInfo)
                return false;

            value = propInfo.GetValue(obj, null);
            return true;
        }

        public PropertyInfo? GetProperty(string propertyName)
            => string.IsNullOrWhiteSpace(propertyName)
                ? null
                : GetPropertyInfo(obj.GetType(), propertyName);
    }

    extension(Type type)
    {
        public PropertyInfo? GetPropertyInfo(string propertyName)
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

        public FieldInfo? GetFieldInfo(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName))
                return null;

            FieldInfo? fieldInfo = null;
            var t = type;
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

    /// <param name="type">class type</param>
    extension(Type type)
    {
        /// <summary>
        /// Get static property value
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <param name="value">value</param>
        /// <returns>true on success</returns>
        public bool TryGetStaticPropertyValue(string propertyName, out object? value)
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
        /// <param name="propertyName">property name</param>
        /// <param name="value">value</param>
        /// <returns>true on success</returns>
        public bool TrySetStaticPropertyValue(string propertyName, object? value)
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
    }

    /// <param name="obj">Class instance</param>
    extension(object obj)
    {
        /// <summary>
        /// Get value of field
        /// </summary>
        /// <param name="fieldName">Name of field</param>
        /// <param name="value">Value</param>
        /// <returns>true on success</returns>
        public bool TryGetFieldValue(string fieldName, out object? value)
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
        /// <param name="fieldName">Field name</param>
        /// <param name="value">value to set</param>
        /// <returns>true on success</returns>
        public bool TrySetFieldValue(string fieldName, object value)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return false;

            if (GetFieldInfo(obj.GetType(), fieldName) is not { } fieldInfo)
                return false;

            fieldInfo.SetValue(obj, value);
            return true;
        }
    }

    /// <param name="type">Class type</param>
    extension(Type type)
    {
        /// <summary>
        /// Get static field value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">value</param>
        /// <returns>true on success</returns>
        public bool TryGetStaticFieldValue(string fieldName, out object? value)
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

        public bool HasAttribute(Type attribute)
            => type.CustomAttributes.Any(a => a.AttributeType == attribute);

        public bool HasAttribute<T>()
            => type.HasAttribute(typeof(T));
    }

    extension(MethodInfo method)
    {
        public bool HasAttribute(Type attribute)
            => method.CustomAttributes.Any(a => a.AttributeType == attribute);

        public bool HasAttribute<T>()
            => method.HasAttribute(typeof(T));
    }

    extension(Assembly asm)
    {
        public bool HasAttribute(Type attribute)
            => asm.CustomAttributes.Any(a => a.AttributeType == attribute);

        public bool HasAttribute<T>()
            => asm.HasAttribute(typeof(T));
    }
}
