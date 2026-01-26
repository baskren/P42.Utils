namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Strikethrough span.
/// </summary>
internal record StrikethroughSpan(int Start, int End, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "Strikethrough";
}
