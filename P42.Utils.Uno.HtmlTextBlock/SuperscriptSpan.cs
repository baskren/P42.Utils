namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Superscript span.
/// </summary>
internal record SuperscriptSpan(int Start, int End, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "Superscript";
}
