using System.Collections.Generic;

namespace P42.Utils;

/// <summary>
/// Dictionary of stacks, indexed by key
/// </summary>
/// <typeparam name="TK"></typeparam>
/// <typeparam name="TV"></typeparam>
public class KeyedStack<TK, TV> : Dictionary<TK, Stack<TV>>  where TV : class where TK : notnull
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
    public TV? Pop(TK key)
    {
        if (!ContainsKey(key))
            return null;
        var stack = this[key];
        return stack.Count > 0 
            ? stack.Pop() 
            : null;
    }

    /// <summary>
    /// Count of stack tagged with key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public new int Count(TK key)
    {
        return !ContainsKey(key) 
            ? 0 
            : this[key].Count;
    }
}
