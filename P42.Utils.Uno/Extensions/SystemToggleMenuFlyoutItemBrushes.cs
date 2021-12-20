using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{

    public static class SystemToggleMenuFlyoutItemBrushes
    {
        public static Brush Background => ColorExtensions.AppBrush("ToggleMenuFlyoutItemBackground");
        public static Brush BackgroundPointerOver => ColorExtensions.AppBrush("ToggleMenuFlyoutItemBackgroundPointerOver");
        public static Brush BackgroundPressed => ColorExtensions.AppBrush("ToggleMenuFlyoutItemBackgroundPressed");
        public static Brush BackgroundDisabled => ColorExtensions.AppBrush("ToggleMenuFlyoutItemBackgroundDisabled");

        public static Brush Foreground => ColorExtensions.AppBrush("ToggleMenuFlyoutItemForeground");
        public static Brush ForegroundPointerOver => ColorExtensions.AppBrush("ToggleMenuFlyoutItemForegroundPointerOver");
        public static Brush ForegroundPressed => ColorExtensions.AppBrush("ToggleMenuFlyoutItemForegroundPressed");
        public static Brush ForegroundDisabled => ColorExtensions.AppBrush("ToggleMenuFlyoutItemForegroundDisabled");

        public static Brush KeyboardAcceleratorTextForeground => ColorExtensions.AppBrush("ToggleMenuFlyoutItemKeyboardAcceleratorTextForeground");
        public static Brush KeyboardAcceleratorTextForegroundPointerOver => ColorExtensions.AppBrush("ToggleMenuFlyoutItemKeyboardAcceleratorTextForegroundPointerOver");
        public static Brush KeyboardAcceleratorTextForegroundPressed => ColorExtensions.AppBrush("ToggleMenuFlyoutItemKeyboardAcceleratorTextForegroundPressed");
        public static Brush KeyboardAcceleratorTextForegroundDisabled => ColorExtensions.AppBrush("ToggleMenuFlyoutItemKeyboardAcceleratorTextForegroundDisabled");

        public static Brush CheckGlyphForeground => ColorExtensions.AppBrush("ToggleMenuFlyoutItemCheckGlyphForeground");
        public static Brush CheckGlyphForegroundPointerOver => ColorExtensions.AppBrush("ToggleMenuFlyoutItemCheckGlyphForegroundPointerOver");
        public static Brush CheckGlyphForegroundPressed => ColorExtensions.AppBrush("ToggleMenuFlyoutItemCheckGlyphForegroundPressed");
        public static Brush CheckGlyphForegroundDisabled => ColorExtensions.AppBrush("ToggleMenuFlyoutItemCheckGlyphForegroundDisabled");
    }
}