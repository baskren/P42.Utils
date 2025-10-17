using Windows.UI;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Background color span.
/// </summary>
internal class BackgroundColorSpan : Span, ICopiable<BackgroundColorSpan>
{
    internal const string SpanKey = "BackgroundColor";

    private Color _color;
    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
        get => _color; 
        set => SetField(ref _color, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.BackgroundColorSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="color">Color.</param>
    /// <param name="id">optional</param>
    public BackgroundColorSpan (int start, int end, Color color, string id = "") : base (start, end, id)
    {
        Key = SpanKey;
        Color = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.BackgroundColorSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public BackgroundColorSpan (BackgroundColorSpan span) : this (span.Start, span.End, span.Color) { }

    /// <summary>
    /// Properties from the specified source.
    /// </summary>
    public void PropertiesFrom(BackgroundColorSpan source)
    {
        base.PropertiesFrom(source);
        Color = source.Color;
    }

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new BackgroundColorSpan(Start, End, Color);
    
    /// <summary>
    /// Get HashCode of span
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Color);
    
}
