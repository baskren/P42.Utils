using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{

    public static class SystemAccentButtonBrushes
    {
        public static Brush Background => ColorExtensions.AppBrush("AccentButtonBackground");
        public static Brush BackgroundPointerOver => ColorExtensions.AppBrush("AccentButtonBackgroundPointerOver");
        public static Brush BackgroundPressed => ColorExtensions.AppBrush("AccentButtonBackgroundPressed");
        public static Brush BackgroundDisabled => ColorExtensions.AppBrush("AccentButtonBackgroundDisabled");
        public static Brush Foreground => ColorExtensions.AppBrush("AccentButtonForeground");
        public static Brush ForegroundPointerOver => ColorExtensions.AppBrush("AccentButtonForegroundPointerOver");
        public static Brush ForegroundPressed => ColorExtensions.AppBrush("AccentButtonForegroundPressed");
        public static Brush ForegroundDisabled => ColorExtensions.AppBrush("AccentButtonForegroundDisabled");
        public static Brush BorderBrush => ColorExtensions.AppBrush("AccentButtonBorderBrush");
        public static Brush BorderBrushPointerOver => ColorExtensions.AppBrush("AccentButtonBorderBrushPointerOver");
        public static Brush BorderBrushPressed => ColorExtensions.AppBrush("AccentButtonBorderBrushPressed");
        public static Brush BorderBrushDisabled => ColorExtensions.AppBrush("AccentButtonBorderBrushDisabled");
    }
}