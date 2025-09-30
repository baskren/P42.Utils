using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace P42.Utils;

/// <summary>
/// Dictionary of stacks, indexed by key
/// </summary>
/// <typeparam name="TK"></typeparam>
/// <typeparam name="TV"></typeparam>
// ReSharper disable once UnusedType.Global
public class KeyedStack<TK, TV> : Dictionary<TK, Stack<TV>>
    where TK : notnull
    where TV : class 
{
    /// <summary>
    /// Constructor
    /// </summary>
    public KeyedStack()
    {
    }

    /// <summary>
    /// Push member on stack tagged with key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void Push(TK key, TV value)
    {
        if (!ContainsKey(key))
            Add(key, new Stack<TV>());
        this[key].Push(value);
    }

    /// <summary>
    /// Pop item off stack tagged with key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public TV Pop(TK key)
    {
        if (!ContainsKey(key))
            throw new InvalidOperationException("No such key");
        var stack = this[key];
        return stack.Count > 0 
            ? stack.Pop() 
            : throw new InvalidOperationException("Stack is empty");
    }

    /// <summary>
    /// Try pop item off stack tagged with key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryPop(TK key, [MaybeNullWhen(false)] out TV value)
    {
        if (!TryGetValue(key, out var stack))
        {
            value = null;
            return false;
        }        
        value = stack.Count > 0 
            ? stack.Pop() 
            : null;
        return value != null;
    }

    /// <summary>
    /// Count of stack tagged with key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public new int Count(TK key)
    {
        return ContainsKey(key)
            ? this[key].Count
            : 0;

    }
}
