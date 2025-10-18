
namespace P42.Utils.Uno;

/// <summary>
/// Font size span.
/// </summary>
internal class FontSizeSpan : Span, ICopiable<FontSizeSpan>
{
    internal const string SpanKey = "Size";

    private float _size = -1;
    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public float Size
    {
        get => _size; 
        set => SetField(ref _size, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontSizeSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="size">Size.</param>
    /// <param name="id">optional</param>
    public FontSizeSpan(int start, int end, float size, string id = "") : base (start, end, id) 
    {
        Key = SpanKey;
        Size = size;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontSizeSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public FontSizeSpan(FontSizeSpan span) : this(span.Start, span.End, span.Size) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(FontSizeSpan source)
    {
        base.PropertiesFrom(source);
        Size = source.Size;
    }

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new FontSizeSpan(Start, End, Size);
    
    /// <summary>
    /// Get HashCode of span
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Size);

}
