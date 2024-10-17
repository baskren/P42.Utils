namespace P42.Utils.Uno;

internal class UnderlineSpan(int start, int end) : Span(SpanKey, start, end), ICopiable<UnderlineSpan>
{
    internal const string SpanKey = nameof(UnderlineSpan);

    //Color = color;
    //Style = style;

    public UnderlineSpan (UnderlineSpan span) : this (span.Start, span.End) { }

    public void PropertiesFrom(UnderlineSpan source)
        => base.PropertiesFrom(source);
    

    public override Span Copy()
        => new UnderlineSpan(Start, End);
    
}
