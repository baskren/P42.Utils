namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Strikethrough span.
/// </summary>
internal class StrikethroughSpan : Span, ICopiable<StrikethroughSpan>
{
    internal const string SpanKey = "Strikethrough";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.StrikethroughSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public StrikethroughSpan (int start, int end, string id = "") : base (start, end, id) 
        => Key = SpanKey;
    

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.StrikethroughSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public StrikethroughSpan (StrikethroughSpan span) : this (span.Start, span.End) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(StrikethroughSpan source)
        => base.PropertiesFrom(source);

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new StrikethroughSpan(Start, End);
    
}
