// /*******************************************************************
//  *
//  * ActionSpan.cs copyright 2017 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal class HyperlinkSpan : Span, ICopiable<HyperlinkSpan>
{
    internal const string SpanKey = "Hyperlink";

    internal const string NullId = "HyperlinkSpanNullId";

    private string _href = string.Empty;
    /// <summary>
    /// Hyperlink Reference
    /// </summary>
    public string Href
    {
        get => _href; 
        set => SetField(ref _href, value);
    }
    
    /// <summary>
    /// Create a new instance of a HyperlinkSpan.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="href"></param>
    /// <param name="id"></param>
    public HyperlinkSpan(int start, int end, string href="", string id="") : base (start, end, id) {
        Key = SpanKey;
        Href = href;
        Id = id;
    }

    /// <summary>
    /// Create a new instance of a HyperlinkSpan from a source span.
    /// </summary>
    /// <param name="span"></param>
    public HyperlinkSpan(HyperlinkSpan span) : this (span.Start, span.End, span.Href) { }

    /// <summary>
    /// Copy properties from a source span.
    /// </summary>
    /// <param name="source"></param>
    public void PropertiesFrom(HyperlinkSpan source)
    {
        base.PropertiesFrom(source);
        Href = source.Href;
        Id = source.Id;
    }

    /// <summary>
    /// Creates a copy of the span.
    /// </summary>
    /// <returns></returns>
    public override Span Copy()
        => new HyperlinkSpan(Start, End, Href);

    /// <summary>
    /// Get HashCode of span
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
        => HashCode.Combine(base.GetHashCode(), Href);

    [Obsolete("Use HasIdOrLinkReference() instead.")]
    public bool IsEmpty() => HasIdOrLinkReference();

    /// <summary>
    /// Is the HyperlinkSpan dangling (does not reference anything)?
    /// </summary>
    /// <returns></returns>
    public bool HasIdOrLinkReference() => !string.IsNullOrWhiteSpace(Href) && !string.IsNullOrWhiteSpace(Id);
}
