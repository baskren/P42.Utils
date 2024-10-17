namespace P42.Utils.Uno;

/// <summary>
/// Font family span.
/// </summary>
internal class FontFamilySpan(int start, int end, Microsoft.UI.Xaml.Media.FontFamily fontFamily =null) : Span(SpanKey, start, end), ICopiable<FontFamilySpan>
{
    internal const string SpanKey = nameof(FontFamilySpan);

    /// <summary>
    /// Gets or sets the name of the font family -OR- resource ID or embedded resource font.
    /// </summary>
    /// <value>The name of the font family.</value>
    public Microsoft.UI.Xaml.Media.FontFamily FontFamily { get; set; } = fontFamily;

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontFamilySpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public FontFamilySpan(FontFamilySpan span) : this (span.Start, span.End, span.FontFamily) {
    }

    public void PropertiesFrom(FontFamilySpan source)
    {
        base.PropertiesFrom(source);
        FontFamily = source.FontFamily;
    }

    public override Span Copy()
        => new FontFamilySpan(Start, End, FontFamily);
    
}
