using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace P42.Utils.Uno
{
    public static class IconExtensions
    {
        public static IconElement AsIconElement(this IconSource source)
        {
            if (source is BitmapIconSource bsource)
                return new BitmapIcon { UriSource = bsource.UriSource, ShowAsMonochrome = bsource.ShowAsMonochrome };
            if (source is FontIconSource fsource)
                return new FontIcon { FontFamily = fsource.FontFamily, FontSize = fsource.FontSize, FontWeight = fsource.FontWeight, Glyph = fsource.Glyph, IsTextScaleFactorEnabled = fsource.IsTextScaleFactorEnabled, MirroredWhenRightToLeft = fsource.MirroredWhenRightToLeft };
            if (source is PathIconSource psource)
                return new PathIcon { Data = psource.Data };
            if (source is SymbolIconSource ssource)
                return new SymbolIcon { Symbol = ssource.Symbol };
            return null;
        }
    }
}
