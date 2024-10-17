namespace P42.Utils.Uno;

/// <summary>
/// Font size span.
/// </summary>
internal class FontSizeSpan(int start, int end, float size) : Span(SpanKey, start, end), ICopiable<FontSizeSpan>
{
    internal const string SpanKey = nameof(FontSizeSpan);

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public float Size { get; set; } = size;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontSizeSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public FontSizeSpan(FontSizeSpan span) : this(span.Start, span.End, span.Size) { }

    public void PropertiesFrom(FontSizeSpan source)
    {
        base.PropertiesFrom(source);
        Size = source.Size;
    }

    public override Span Copy()
        => new FontSizeSpan(Start, End, Size);
    
}
