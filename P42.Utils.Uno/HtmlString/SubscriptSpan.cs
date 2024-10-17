namespace P42.Utils.Uno;

internal class SubscriptSpan(int start, int end) : Span(SpanKey, start, end), ICopiable<SubscriptSpan>
{
    internal const string SpanKey = nameof(SubscriptSpan);

    public SubscriptSpan (SubscriptSpan span) : this (span.Start, span.End) { }

    public void PropertiesFrom(SubscriptSpan source)
        => base.PropertiesFrom(source);

    public override Span Copy()
        => new SubscriptSpan(Start, End);
    
}
