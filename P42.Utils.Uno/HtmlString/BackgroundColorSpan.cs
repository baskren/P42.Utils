using Windows.UI;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Background color span.
/// </summary>
internal class BackgroundColorSpan(int start, int end, Color color)
    : Span(SpanKey, start, end), ICopiable<BackgroundColorSpan>
{
    internal const string SpanKey = nameof(BackgroundColorSpan);

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color { get; set; } = color;

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.BackgroundColorSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public BackgroundColorSpan (BackgroundColorSpan span) : this (span.Start, span.End, span.Color) { }

    public void PropertiesFrom(BackgroundColorSpan source)
    {
        base.PropertiesFrom(source);
        Color = source.Color;
    }

    public override Span Copy()
        => new BackgroundColorSpan(Start, End, Color);

}
