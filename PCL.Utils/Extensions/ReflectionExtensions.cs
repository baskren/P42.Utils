﻿using System;
using System.Reflection;
using System.Linq;

namespace PCL.Utils
{
	public static class ReflectionExtensions
	{

		public static PropertyInfo GetPropertyInfo(this Type type, string propertyName)
		{
			if (string.IsNullOrEmpty (propertyName))
				return null;
			PropertyInfo propInfo;
			do {
				//propInfo = type.GetRuntimeProperty(propertyName);
				var properties = type.GetRuntimeProperties();
				propInfo = properties.FirstOrDefault(arg => arg.Name == propertyName);
				//var props = type.GetRuntimeProperties();
				//propInfo = type.GetProperty(propertyName,
				//	BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				type = type.GetTypeInfo().BaseType;
				//type = type.BaseType;
			} while (propInfo == null && type != null);
			return propInfo;
		}

		public static FieldInfo GetFieldInfo(this Type type, string fieldName) {
			if (string.IsNullOrEmpty (fieldName))
				return null;
			FieldInfo fieldInfo;
			do {
				var fields = type.GetRuntimeFields();
				fieldInfo = fields.FirstOrDefault (arg => arg.Name == fieldName);
				type = type.GetTypeInfo ().BaseType;
			} while (fieldInfo == null && type != null);
			return fieldInfo;
		}

		public static MethodInfo GetMethodInfo(this Type type, string methodName) {
			if (string.IsNullOrEmpty (methodName))
				return null;
			MethodInfo methodInfo;
			do {
				var methods = type.GetRuntimeMethods ();
				methodInfo = methods.FirstOrDefault (arg => arg.Name == methodName);
				type = type.GetTypeInfo ().BaseType;
			} while (methodInfo == null && type != null);
			return methodInfo;
		}

		public static object GetPropertyValue(this object obj, string propertyName) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			Type objType = obj.GetType();
			PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
			return propInfo == null ? null : propInfo.GetValue (obj, null);
		}

		public static void SetPropertyValue(this object obj, string propertyName, object val) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			Type objType = obj.GetType();
			PropertyInfo propInfo = GetPropertyInfo(objType, propertyName);
			if (propInfo == null)
				throw new ArgumentOutOfRangeException("propertyName", string.Format("Couldn't find property {0} in type {1}", propertyName, objType.FullName));
			propInfo.SetValue(obj, val, null);
		}	

		public static object GetFieldValue(this object obj, string fieldName) {
			if (obj==null)
				throw new ArgumentNullException("obj");
			Type objType = obj.GetType();
			FieldInfo fieldInfo = GetFieldInfo(objType, fieldName);
			return fieldInfo == null ? null : fieldInfo.GetValue (obj);
		}

		public static void SetFieldValue(this object obj, string fieldName, object val) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			Type objType = obj.GetType();
			FieldInfo fieldInfo = GetFieldInfo(objType, fieldName);
			if (fieldInfo == null)
				throw new ArgumentOutOfRangeException("fieldName", string.Format("Couldn't find field {0} in type {1}", fieldName, objType.FullName));
			fieldInfo.SetValue(obj, val);
		}	

		public static object GetFieldValue(this Type type, string fieldName) {
			FieldInfo fieldInfo = GetFieldInfo(type, fieldName);
			return fieldInfo == null ? null : fieldInfo.GetValue (null);
		}

		public static object GetPropertyValue(this Type type, string fieldName) {
			PropertyInfo propertyInfo = GetPropertyInfo(type, fieldName);
			return propertyInfo == null ? null : propertyInfo.GetValue (null);
		}

		public static object CallMethod(this object obj, string methodName, object[] parameters) {
			MethodInfo methodInfo = GetMethodInfo (obj.GetType (), methodName);
			if (methodInfo == null) 
				throw new ArgumentOutOfRangeException("methodName", $"Coundn't find method {methodName} in type {obj.GetType().FullName}");
			return methodInfo.Invoke (obj, parameters);
		}

	}
}

