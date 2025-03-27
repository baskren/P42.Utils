// /*******************************************************************
//  *
//  * NumeratorSpan.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal class NumeratorSpan: Span, ICopiable<NumeratorSpan>
{
    internal const string SpanKey = "Numerator";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.NumeratorSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public NumeratorSpan(int start, int end, string id = "") : base(start, end, id) 
        => Key = SpanKey;


    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.NumeratorSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public NumeratorSpan(NumeratorSpan span) : this(span.Start, span.End) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(NumeratorSpan source)
        => base.PropertiesFrom(source);
    

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new NumeratorSpan(Start, End);
    
}
