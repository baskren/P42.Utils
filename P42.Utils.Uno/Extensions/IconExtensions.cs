using Microsoft.UI.Xaml.Controls;

namespace P42.Utils.Uno;

public static class IconExtensions
{
    /// <summary>
    /// Create an IconElement from an IconSource
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IconElement? AsIconElement(this IconSource source)
        => source switch
        {
            BitmapIconSource bitmapIconSource => new BitmapIcon
            {
                UriSource = bitmapIconSource.UriSource, ShowAsMonochrome = bitmapIconSource.ShowAsMonochrome
            },
            FontIconSource fontIconSource => new FontIcon
            {
                FontFamily = fontIconSource.FontFamily,
                FontSize = fontIconSource.FontSize,
                FontWeight = fontIconSource.FontWeight,
                Glyph = fontIconSource.Glyph,
                IsTextScaleFactorEnabled = fontIconSource.IsTextScaleFactorEnabled,
                MirroredWhenRightToLeft = fontIconSource.MirroredWhenRightToLeft
            },
            PathIconSource pathIconSource => new PathIcon { Data = pathIconSource.Data },
            SymbolIconSource symbolIconSource => new SymbolIcon { Symbol = symbolIconSource.Symbol },
            _ => null
        };
    
}
