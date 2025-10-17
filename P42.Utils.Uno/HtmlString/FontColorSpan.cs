using Windows.UI;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Font color span.
/// </summary>
internal class FontColorSpan : Span, ICopiable<FontColorSpan>
{
    internal const string SpanKey = "FontColor";

    private Color _color;

    /// <summary>
    /// Gets or sets the font foreground color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
        get => _color; 
        set => SetField(ref _color, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontColorSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="color">Color.</param>
    /// <param name="id">optional</param>
    public FontColorSpan (int start, int end, Color color, string id = "") : base (start, end, id) 
    {
        Key = SpanKey;
        Color = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontColorSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public FontColorSpan(FontColorSpan span) : this (span.Start, span.End, span.Color) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(FontColorSpan source)
    {
        base.PropertiesFrom(source);
        Color = source.Color;
    }

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new FontColorSpan(Start, End, Color);
    
    /// <summary>
    /// Get HashCode of span
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Color);

}
