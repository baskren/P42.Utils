namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Superscript span.
/// </summary>
internal class SuperscriptSpan : Span, ICopiable<SuperscriptSpan>
{
    internal const string SpanKey = "Superscript";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.SuperscriptSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public SuperscriptSpan (int start, int end, string id = "") : base(start, end, id) 
        => Key = SpanKey;
    

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.SuperscriptSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public SuperscriptSpan (SuperscriptSpan span) : this(span.Start, span.End) {
    }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(SuperscriptSpan source)
    {
        base.PropertiesFrom(source);
    }

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new SuperscriptSpan(Start, End);
    
}
