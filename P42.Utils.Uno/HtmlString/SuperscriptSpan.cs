namespace P42.Utils.Uno;

internal class SuperscriptSpan(int start, int end) : Span(SpanKey, start, end), ICopiable<SuperscriptSpan>
{
    internal const string SpanKey = nameof(SuperscriptSpan);

    public SuperscriptSpan (SuperscriptSpan span) : this(span.Start, span.End) { }

    public void PropertiesFrom(SuperscriptSpan source)
        => base.PropertiesFrom(source);

    public override Span Copy()
        => new SuperscriptSpan(Start, End);
    
}
