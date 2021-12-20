using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    // Full list of system brushes found here: https://github.com/MicrosoftDocs/windows-uwp/issues/2072
    public static class SystemButtonBrushes
    {
        public static Brush Background => ColorExtensions.AppBrush("ButtonBackground");
        public static Brush BackgroundPointerOver => ColorExtensions.AppBrush("ButtonBackgroundPointerOver");
        public static Brush BackgroundPressed => ColorExtensions.AppBrush("ButtonBackgroundPressed");
        public static Brush BackgroundDisabled => ColorExtensions.AppBrush("ButtonBackgroundDisabled");
        public static Brush Foreground => ColorExtensions.AppBrush("ButtonForeground");
        public static Brush ForegroundPointerOver => ColorExtensions.AppBrush("ButtonForegroundPointerOver");
        public static Brush ForegroundPressed => ColorExtensions.AppBrush("ButtonForegroundPressed");
        public static Brush ForegroundDisabled => ColorExtensions.AppBrush("ButtonForegroundDisabled");
        public static Brush BorderBrush => ColorExtensions.AppBrush("ButtonBorderBrush");
        public static Brush BorderBrushPointerOver => ColorExtensions.AppBrush("ButtonBorderBrushPointerOver");
        public static Brush BorderBrushPressed => ColorExtensions.AppBrush("ButtonBorderBrushPressed");
        public static Brush BorderBrushDisabled => ColorExtensions.AppBrush("ButtonBorderBrushDisabled");
    }
}
