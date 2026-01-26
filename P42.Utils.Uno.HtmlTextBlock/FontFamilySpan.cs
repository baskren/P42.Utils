
namespace P42.Utils.Uno;

/// <summary>
/// Font family span.
/// </summary>
internal record FontFamilySpan(int Start, int End, FontFamily FontFamily, string Id = "") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "FontFamily";
}

