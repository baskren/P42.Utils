// /*******************************************************************
//  *
//  * DenominatorSpan.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal class DenominatorSpan : Span, ICopiable<DenominatorSpan>
{
    internal const string SpanKey = "Denominator";

    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.DenominatorSpan"/> class.
    /// </summary>
    /// <param name="start">Start.</param>
    /// <param name="end">End.</param>
    /// <param name="id">optional</param>
    public DenominatorSpan(int start, int end, string id = "") : base (start, end, id)         
        => Key = SpanKey;


    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.DenominatorSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public DenominatorSpan(DenominatorSpan span) : this (span.Start, span.End) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(DenominatorSpan source)
        => base.PropertiesFrom(source);
    

    /// <summary>
    /// Make a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new DenominatorSpan(Start, End);
    
}
