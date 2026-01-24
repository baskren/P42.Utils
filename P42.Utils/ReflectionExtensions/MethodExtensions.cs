using System.Reflection;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class MethodExtensions
{
    /// <summary>
    /// Get Method Info
    /// </summary>
    /// <param name="type">Type</param>
    /// <param name="methodName">Method Name</param>
    /// <param name="parameterTypes">Parameter Types</param>
    /// <returns></returns>
#if RELEASE
  [Obsolete("NOT FOR RELEASE BUILDS")]
#endif

    [JetBrains.Annotations.PublicAPI]
    public static MethodInfo? GetMethodInfo(this Type type, string methodName, Type[]? parameterTypes = null)
    {
        if (string.IsNullOrEmpty(methodName))
            return null;
            
        parameterTypes ??= [];
        MethodInfo? methodInfo = null;
        var t = type;
        do
        {
            if (t.GetRuntimeMethods() is { } methods)
                foreach (var method in methods)
                {
                    if (
                        method.Name == methodName &&
                        method.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes)
                    )
                        methodInfo = method;
                }

            t = t.GetTypeInfo().BaseType;
        } while (methodInfo == null && t != null);
            
        return methodInfo;
    }

    /// <summary>
    /// Call method
    /// </summary>
    /// <param name="obj">class instance</param>
    /// <param name="methodName">method name</param>
    /// <param name="result">result</param>
    /// <returns>true on success</returns>
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    public static bool TryCallMethod(this object obj, string methodName, out object? result)
        => obj.GetType().TryCallMethod(methodName, null, out result);
    
    /// <summary>
    /// Call method
    /// </summary>
    /// <param name="obj">class instance</param>
    /// <param name="methodName">method name</param>
    /// <param name="parameters">arguments</param>
    /// <param name="result">result</param>
    /// <returns>true on success</returns>
#if RELEASE
    [Obsolete("NOT FOR RELEASE BUILDS")]
#endif
    [JetBrains.Annotations.PublicAPI]
    public static bool TryCallMethod(this object obj, string methodName, object[]? parameters, out object? result)
    {
        result = null;
        MethodInfo? methodInfo;
        if (parameters == null || parameters.Length == 0)
            methodInfo = obj.GetType().GetMethodInfo(methodName);
        else
        {
            var parameterTypes = parameters.Select(p => p.GetType()).ToArray();
            methodInfo = obj.GetType().GetMethodInfo(methodName, parameterTypes);
        }

        if (methodInfo == null)
            return false;
        
        result = methodInfo.Invoke(obj, parameters);
        return true;
    }

    
}
