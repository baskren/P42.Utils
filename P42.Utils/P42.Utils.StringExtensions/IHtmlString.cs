

using JetBrains.Annotations;

namespace P42.Utils;

/// <summary>
/// Interface for HtmlString
/// </summary>
[UsedImplicitly]
public interface IHtmlString
{
    /// <summary>
    /// Convert object to HTML string
    /// </summary>
    /// <returns></returns>
    string ToHtml();
}
