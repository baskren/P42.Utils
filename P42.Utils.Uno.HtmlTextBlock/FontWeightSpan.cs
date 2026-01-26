namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Bold span.
/// </summary>
internal record FontWeightSpan(int Start, int End, short Weight, bool IsRelativeToParent = false, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "Weight";
}
