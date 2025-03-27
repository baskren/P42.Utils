using System;
using System.Runtime.CompilerServices;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno FormattedString Span
/// </summary>
internal abstract class Span : NotifiableObject.FieldBackedNotifiablePropertyObject, ICopiable<Span> , IEquatable<Span>
{

    #region Properties
    
    private string _key;
    /// <summary>
    /// Span Key
    /// </summary>
    public string Key
    {
        get => _key;
        internal set => SetField(ref _key, value);
    }
    
    
    private int _start;
    /// <summary>
    /// Gets or sets the span's start.
    /// </summary>
    /// <value>The start.</value>
    public int Start
    {
        get => _start;
        set => SetField(ref _start, value);
    }

    // use int.MaxValue to indicate that the span is unterminated (goes to the end of the string)
    private int _end;
    /// <summary>
    /// Gets or sets the span's end.
    /// </summary>
    /// <value>The end.</value>
    public int End
    {
        get => _end;
        set => SetField(ref _end, value);
    }

    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length.  int.MaxValue to indicate that the span is unterminated (goes to the end of the string)</value>
    public int Length
    {
        get
        {
            if (_end == int.MaxValue)
                return int.MaxValue;

            return _end - _start + 1;
        }
        set
        {
            if (_end - _start + 1 == value)
                return;
            if (value == int.MaxValue)
                _end = int.MaxValue;
            else
                _end = _start + value - 1;
            OnPropertyChanged(nameof(End));
        }
    }

    private string _id;
    /// <summary>
    /// Id attribute
    /// </summary>
    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }
    #endregion


    #region Construction / Diposal

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.Span"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    /// <param name="key">optional</param>
    protected Span(int start, int end, string id = "", [CallerMemberName] string key = "")
    {
        // TODO: If this works, can remove Key setting from all derived classes
        if (key.EndsWith("Span"))
            key = key[..^"Span".Length];
        _key = key;
        _id = id;
        _start = start;
        _end = end;
    }
    #endregion


    #region
    /// <summary>
    /// Copies properties from the specified source.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(Span source)
    {
        Key = source.Key;
        Start = source.Start;
        End = source.End;
    }

    /// <summary>
    /// Makes a copy of the span
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual Span Copy()
        => throw new NotImplementedException();
    
    // TODO: Can we make this generic and use PropertiesFrom to complete the copy?
    #endregion

    /// <summary>
    /// Compares the current span with the specified span.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Span? other)
    {
        if (other is null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Key == other.Key && 
               _start == other._start && 
               _end == other._end;
    }

    public override bool Equals(object? obj)
        => obj is Span span && Equals(span);
    

    public override int GetHashCode()
        => HashCode.Combine(Key, Start, End, Id);
    
}
