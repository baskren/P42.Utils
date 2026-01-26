// /*******************************************************************
//  *
//  * ActionSpan.cs copyright 2017 ben, 42nd Parallel - ALL RIGHTS RESERVED.
//  *
//  *******************************************************************/

namespace P42.Utils.Uno;

internal record HyperlinkSpan(int Start, int End, string Href="", string Id="") : Span(SpanKey, Start, End, Id)
{
    public const string SpanKey = "HyperLink";
    
    [Obsolete("Use HasIdOrLinkReference() instead.")]
    public bool IsEmpty() => HasIdOrLinkReference();

    /// <summary>
    /// Is the HyperlinkSpan dangling (does not reference anything)?
    /// </summary>
    /// <returns></returns>
    public bool HasIdOrLinkReference() => !string.IsNullOrWhiteSpace(Href) && !string.IsNullOrWhiteSpace(Id);
}
