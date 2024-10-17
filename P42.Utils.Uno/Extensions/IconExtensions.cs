using Microsoft.UI.Xaml.Controls;

namespace P42.Utils.Uno;

public static class IconExtensions
{
    /// <summary>
    /// Creates IconElement from IconSource
    /// </summary>
    /// <param name="source"></param>
    /// <returns>null on fail</returns>
    public static IconElement? AsIconElement(this IconSource source)
    {
        return source switch
        {
            BitmapIconSource bSource => new BitmapIcon
            {
                UriSource = bSource.UriSource, ShowAsMonochrome = bSource.ShowAsMonochrome
            },
            FontIconSource fSource => new FontIcon
            {
                FontFamily = fSource.FontFamily,
                FontSize = fSource.FontSize,
                FontWeight = fSource.FontWeight,
                Glyph = fSource.Glyph,
                IsTextScaleFactorEnabled = fSource.IsTextScaleFactorEnabled,
                MirroredWhenRightToLeft = fSource.MirroredWhenRightToLeft
            },
            PathIconSource pSource => new PathIcon { Data = pSource.Data },
            SymbolIconSource sSource => new SymbolIcon { Symbol = sSource.Symbol },
            _ => null
        };
    }
}
