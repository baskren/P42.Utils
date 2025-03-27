namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Subscript span.
/// </summary>
internal class SubscriptSpan : Span, ICopiable<SubscriptSpan>
{
    internal const string SpanKey = "Subscript";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.SubscriptSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public SubscriptSpan (int start, int end, string id = "") : base (start, end, id) 
        => Key = SpanKey;


    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.SubscriptSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public SubscriptSpan (SubscriptSpan span) : this (span.Start, span.End) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(SubscriptSpan source)
        => base.PropertiesFrom(source);

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new SubscriptSpan(Start, End);
    
}
