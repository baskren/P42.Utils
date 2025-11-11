
namespace P42.Utils.Uno;

/// <summary>
/// Font family span.
/// </summary>
internal class FontFamilySpan : Span, ICopiable<FontFamilySpan>
{
    internal const string SpanKey = "FontFamily";

    private FontFamily? _fontFamily;

    /// <summary>
    /// Gets or sets the name of the font family -OR- resource ID or embedded resource font.
    /// </summary>
    /// <value>The name of the font family.</value>
    public FontFamily? FontFamily
    {
        get => _fontFamily; 
        set => SetField(ref _fontFamily, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontFamilySpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="fontFamily">Font name.</param>
    /// <param name="id">optional</param>
    public FontFamilySpan(int start, int end, FontFamily? fontFamily = null, string id = "") : base (start, end, id) 
    {
        Key = SpanKey;
        FontFamily = fontFamily;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontFamilySpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public FontFamilySpan(FontFamilySpan span) : this (span.Start, span.End, span.FontFamily) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(FontFamilySpan source)
    {
        base.PropertiesFrom(source);
        FontFamily = source.FontFamily;
    }

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new FontFamilySpan(Start, End, FontFamily);
    
    /// <summary>
    /// Get HashCode of span
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), FontFamily);

}
