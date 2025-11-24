using System.Runtime.CompilerServices;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno FormattedString Span
/// </summary>
internal abstract class Span : NotifiableObject.FieldBackedNotifiablePropertyObject, ICopiable<Span> , IEquatable<Span>
{

    #region Properties

    /// <summary>
    /// Span Key
    /// </summary>
    public string Key
    {
        get;
        protected set => SetField(ref field, value);
    }


    /// <summary>
    /// Gets or sets the span's start.
    /// </summary>
    /// <value>The start.</value>
    public int Start
    {
        get;
        private set => SetField(ref field, value);
    }

    // use int.MaxValue to indicate that the span is unterminated (goes to the end of the string)
    /// <summary>
    /// Gets or sets the span's end.
    /// </summary>
    /// <value>The end.</value>
    public int End
    {
        get;
        set => SetField(ref field, value);
    }

    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length.  int.MaxValue to indicate that the span is unterminated (goes to the end of the string)</value>
    public int Length
    {
        get
        {
            if (End == int.MaxValue)
                return int.MaxValue;

            return End - Start + 1;
        }
        set
        {
            if (End - Start + 1 == value)
                return;
            if (value == int.MaxValue)
                End = int.MaxValue;
            else
                End = Start + value - 1;
            OnPropertyChanged(nameof(End));
        }
    }

    /// <summary>
    /// Id attribute
    /// </summary>
    public string Id
    {
        get;
        protected set => SetField(ref field, value);
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
        Key = key;
        Id = id;
        Start = start;
        End = end;
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
               Start == other.Start && 
               End == other.End;
    }

    public override bool Equals(object? obj)
        => obj is Span span && Equals(span);
    

    public override int GetHashCode()
        => HashCode.Combine(Key, Start, End, Id);
    
}
