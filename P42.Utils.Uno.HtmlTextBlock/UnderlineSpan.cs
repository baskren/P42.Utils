namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Underline span.
/// </summary>
internal record UnderlineSpan(int Start, int End, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "Underline";
}
