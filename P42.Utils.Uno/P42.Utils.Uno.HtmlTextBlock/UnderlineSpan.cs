namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Underline span.
/// </summary>
internal class UnderlineSpan : Span, ICopiable<UnderlineSpan>
{
    internal const string SpanKey = "Underline";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.UnderlineSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public UnderlineSpan (int start, int end, string id = "") : base (start, end, id) 
    {
        //Color = color;
        //Style = style;
        Key = SpanKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.UnderlineSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public UnderlineSpan (UnderlineSpan span) : this (span.Start, span.End) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(UnderlineSpan source)
        => base.PropertiesFrom(source);
    

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new UnderlineSpan(Start, End);
    
}
