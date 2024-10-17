namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Bold span.
/// </summary>
internal class BoldSpan(int start, int end) : Span(SpanKey, start, end), ICopiable<BoldSpan>
{
    internal const string SpanKey = nameof(BoldSpan);

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.BoldSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public BoldSpan (BoldSpan span) : this (span.Start, span.End) { }

    public void PropertiesFrom(BoldSpan source)
        => base.PropertiesFrom(source);
    

    public override Span Copy()
        => new BoldSpan(Start, End);
    
}
