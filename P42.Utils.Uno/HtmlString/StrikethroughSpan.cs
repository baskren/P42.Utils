namespace P42.Utils.Uno;

internal class StrikethroughSpan(int start, int end) : Span(SpanKey, start, end), ICopiable<StrikethroughSpan>
{
    internal const string SpanKey = nameof(StrikethroughSpan);

    public StrikethroughSpan (StrikethroughSpan span) : this (span.Start, span.End) { }

    public void PropertiesFrom(StrikethroughSpan source)
        => base.PropertiesFrom(source);
    
    public override Span Copy()
        =>  new StrikethroughSpan(Start, End);
    
}
