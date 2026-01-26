using Windows.UI;

namespace P42.Utils.Uno;

/// <summary>
/// P42.Utils.Uno Font color span.
/// </summary>
internal record FontColorSpan(int Start, int End, Color Color, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "FontColor";
}
