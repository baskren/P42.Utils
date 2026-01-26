namespace P42.Utils.Uno;

/// <summary>
/// Italics span.
/// </summary>
internal record ItalicsSpan(int Start, int End, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "Italics";
}
