
namespace P42.Utils.Uno;

/// <summary>
/// Font size span.
/// </summary>
internal record FontSizeSpan(int Start, int End, float Size, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "FontSize";
}


