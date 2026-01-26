// /*******************************************************************
//  *
//  * NumeratorSpan.cs copyright 2016 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal record NumeratorSpan(int Start, int End, string Id = ""): Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "Numerator";
}
