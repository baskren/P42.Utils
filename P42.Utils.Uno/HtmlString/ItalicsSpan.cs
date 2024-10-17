namespace P42.Utils.Uno;

internal class ItalicsSpan(int start, int end) : Span(SpanKey, start, end), ICopiable<ItalicsSpan>
{
    internal const string SpanKey = nameof(ItalicsSpan);

    public ItalicsSpan(ItalicsSpan span) : this (span.Start, span.End) {
    }

    public void PropertiesFrom(ItalicsSpan source)
        => base.PropertiesFrom(source);

    public override Span Copy()
        => new ItalicsSpan(Start, End);
    
}
