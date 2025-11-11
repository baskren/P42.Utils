using System.Diagnostics.CodeAnalysis;

namespace P42.Utils;

/// <summary>
/// Dictionary of stacks, indexed by key
/// </summary>
/// <typeparam name="Tk"></typeparam>
/// <typeparam name="Tv"></typeparam>
// ReSharper disable once UnusedType.Global
public class KeyedStack<Tk, Tv> : Dictionary<Tk, Stack<Tv>>
    where Tk : notnull
    where Tv : class 
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
    public void Push(Tk key, Tv value)
    {
        if (!ContainsKey(key))
            Add(key, new Stack<Tv>());
        this[key].Push(value);
    }

    /// <summary>
    /// Pop item off stack tagged with key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Tv Pop(Tk key)
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
    public bool TryPop(Tk key, [MaybeNullWhen(false)] out Tv value)
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
    public new int Count(Tk key)
    {
        return ContainsKey(key)
            ? this[key].Count
            : 0;

    }
}
