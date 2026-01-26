using Windows.UI;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Background color span.
/// </summary>
internal record BackgroundColorSpan(int Start, int End, Color Color, string Id = "")
    : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "BackgroundColor";
}
