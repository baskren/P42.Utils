using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{

    public static class SystemToggleButtonBrushes
    {
        public static Brush BackgroundChecked => ColorExtensions.AppBrush("ToggleButtonBackgroundChecked");
        public static Brush BackgroundCheckedPointerOver => ColorExtensions.AppBrush("ToggleButtonBackgroundCheckedPointerOver");
        public static Brush BackgroundCheckedPressed => ColorExtensions.AppBrush("ToggleButtonBackgroundCheckedPressed");
        public static Brush BackgroundCheckedDisabled => ColorExtensions.AppBrush("ToggleButtonBackgroundCheckedDisabled");
        public static Brush ForegroundChecked => ColorExtensions.AppBrush("ToggleButtonForegroundChecked");
        public static Brush ForegroundCheckedPointerOver => ColorExtensions.AppBrush("ToggleButtonForegroundCheckedPointerOver");
        public static Brush ForegroundCheckedPressed => ColorExtensions.AppBrush("ToggleButtonForegroundCheckedPressed");
        public static Brush ForegroundCheckedDisabled => ColorExtensions.AppBrush("ToggleButtonForegroundCheckedDisabled");
        public static Brush Background => ColorExtensions.AppBrush("ToggleButtonBackgroundThemeBrush");


        public static Brush Border => ColorExtensions.AppBrush("ToggleButtonBorderThemeBrush");
        public static Brush CheckedBackground => ColorExtensions.AppBrush("ToggleButtonCheckedBackgroundThemeBrush");
        public static Brush CheckedBorder => ColorExtensions.AppBrush("ToggleButtonCheckedBorderThemeBrush");
        public static Brush CheckedDisabledBackground => ColorExtensions.AppBrush("ToggleButtonCheckedDisabledBackgroundThemeBrush");
        public static Brush CheckedDisabledForeground => ColorExtensions.AppBrush("ToggleButtonCheckedDisabledForegroundThemeBrush");
        public static Brush CheckedForeground => ColorExtensions.AppBrush("ToggleButtonCheckedForegroundThemeBrush");
        public static Brush CheckedPointerOverBackground => ColorExtensions.AppBrush("ToggleButtonCheckedPointerOverBackgroundThemeBrush");
        public static Brush CheckedPointerOverBorder => ColorExtensions.AppBrush("ToggleButtonCheckedPointerOverBorderThemeBrush");
        public static Brush CheckedPressedBackground => ColorExtensions.AppBrush("ToggleButtonCheckedPressedBackgroundThemeBrush");
        public static Brush CheckedPressedBorder => ColorExtensions.AppBrush("ToggleButtonCheckedPressedBorderThemeBrush");
        public static Brush CheckedPressedForeground => ColorExtensions.AppBrush("ToggleButtonCheckedPressedForegroundThemeBrush");
        public static Brush DisabledBorder => ColorExtensions.AppBrush("ToggleButtonDisabledBorderThemeBrush");
        public static Brush DisabledForeground => ColorExtensions.AppBrush("ToggleButtonDisabledForegroundThemeBrush");
        public static Brush Foreground => ColorExtensions.AppBrush("ToggleButtonForegroundThemeBrush");
        public static Brush PointerOverBackground => ColorExtensions.AppBrush("ToggleButtonPointerOverBackgroundThemeBrush");
        public static Brush PressedBackground => ColorExtensions.AppBrush("ToggleButtonPressedBackgroundThemeBrush");
        public static Brush PressedForeground => ColorExtensions.AppBrush("ToggleButtonPressedForegroundThemeBrush");
    }
}