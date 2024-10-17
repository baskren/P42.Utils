// /*******************************************************************
//  *
//  * DenominatorSpan.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal class DenominatorSpan(int start, int end) : Span(SpanKey, start, end), ICopiable<DenominatorSpan>
{
    internal const string SpanKey = nameof(DenominatorSpan);
    
    /// <summary>
    /// Initializes a new instance of the <see cref="P42.Utils.Uno.DenominatorSpan"/> class.
    /// </summary>
    /// <param name="span">Span.</param>
    public DenominatorSpan(DenominatorSpan span) : this (span.Start, span.End) {
    }

    public void PropertiesFrom(DenominatorSpan source)
        => base.PropertiesFrom(source);
    

    public override Span Copy()
        => new DenominatorSpan(Start, End);
    
}
