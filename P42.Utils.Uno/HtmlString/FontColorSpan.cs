using Windows.UI;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Font color span.
/// </summary>
internal class FontColorSpan(int start, int end, Color color) : Span(SpanKey, start, end), ICopiable<FontColorSpan>
{
    internal const string SpanKey = nameof(FontColorSpan);

    /// <summary>
    /// Gets or sets the font foreground color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color { get; set; } = color;


    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.FontColorSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public FontColorSpan(FontColorSpan span) : this (span.Start, span.End, span.Color) { }

    public void PropertiesFrom(FontColorSpan source)
    {
        base.PropertiesFrom(source);
        Color = source.Color;
    }

    public override Span Copy()
        => new FontColorSpan(Start, End, Color);
    
}
