namespace P42.Utils.Uno;

/// <summary>
/// Italics span.
/// </summary>
internal class ItalicsSpan : Span, ICopiable<ItalicsSpan>
{
    internal const string SpanKey = "Italics";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.ItalicsSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public ItalicsSpan (int start, int end, string id = "") : base (start, end, id) 
        => Key = SpanKey;


    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.ItalicsSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public ItalicsSpan(ItalicsSpan span) : base (span.Start, span.End) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(ItalicsSpan source)
        => base.PropertiesFrom(source);
    

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new ItalicsSpan(Start, End);
    
}
