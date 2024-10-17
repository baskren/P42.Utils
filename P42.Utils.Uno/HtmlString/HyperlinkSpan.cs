// /*******************************************************************
//  *
//  * ActionSpan.cs copyright 2017 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal class HyperlinkSpan(int start, int end, string? href = null, string? id = null)
    : Span(SpanKey, start, end), ICopiable<HyperlinkSpan>
{
    internal const string SpanKey = nameof(HyperlinkSpan);

    internal const string NullId = "HyperlinkSpanNullId";

    public string? Href { get; set; } = href;

    public string? Id { get; set; } = id;


    public HyperlinkSpan(HyperlinkSpan span) : this (span.Start, span.End, span.Href, span.Id) {
    }

    public void PropertiesFrom(HyperlinkSpan source)
    {
        base.PropertiesFrom(source);
        Href = source.Href;
        Id = source.Id;
    }

    public override Span Copy()
        => new HyperlinkSpan(Start, End, Href, Id);
    

    public bool IsEmpty() => string.IsNullOrWhiteSpace(Href) && string.IsNullOrWhiteSpace(Id);
}
