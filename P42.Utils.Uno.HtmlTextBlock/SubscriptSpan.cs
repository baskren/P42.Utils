namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Subscript span.
/// </summary>
internal record SubscriptSpan(int Start, int End, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "Subscript";
}
