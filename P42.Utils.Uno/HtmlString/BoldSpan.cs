namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Bold span.
/// </summary>
internal class BoldSpan : Span, ICopiable<BoldSpan>
{
    internal const string SpanKey = "Bold";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.BoldSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public BoldSpan (int start, int end, string id = "") : base (start, end, id) 
        => Key = SpanKey;


    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.BoldSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public BoldSpan (BoldSpan span) : this (span.Start, span.End) { }

    /// <summary>
    /// Copies the properties from the specified source.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(BoldSpan source) 
        => base.PropertiesFrom(source);
    

    /// <summary>
    /// Make a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new BoldSpan(Start, End);
    
}
