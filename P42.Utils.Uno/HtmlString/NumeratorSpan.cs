// /*******************************************************************
//  *
//  * NumeratorSpan.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal class NumeratorSpan(int start, int end): Span(SpanKey, start, end), ICopiable<NumeratorSpan>
{
    internal const string SpanKey = nameof(NumeratorSpan);
        
    public NumeratorSpan(NumeratorSpan span) : this(span.Start, span.End) { }

    public void PropertiesFrom(NumeratorSpan source)
        => base.PropertiesFrom(source);
		

    public override Span Copy()
        => new NumeratorSpan(Start, End);
		
}
