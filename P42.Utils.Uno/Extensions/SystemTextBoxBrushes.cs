using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{

    public static class SystemTextBoxBrushes
    {
        public static Brush ForegroundHeader => ColorExtensions.AppBrush("TextBoxForegroundHeaderThemeBrush");
        public static Brush PlaceholderText => ColorExtensions.AppBrush("TextBoxPlaceholderTextThemeBrush");
        public static Brush Background => ColorExtensions.AppBrush("TextBoxBackgroundThemeBrush");
        public static Brush Border => ColorExtensions.AppBrush("TextBoxBorderThemeBrush");
        public static Brush ButtonBackground => ColorExtensions.AppBrush("TextBoxButtonBackgroundThemeBrush");
        public static Brush ButtonBorder => ColorExtensions.AppBrush("TextBoxButtonBorderThemeBrush");
        public static Brush ButtonForeground => ColorExtensions.AppBrush("TextBoxButtonForegroundThemeBrush");
        public static Brush ectionHighlightColor => ColorExtensions.AppBrush("TextSelectionHighlightColorThemeBrush");
        public static Brush ButtonPointerOverBackground => ColorExtensions.AppBrush("TextBoxButtonPointerOverBackgroundThemeBrush");
        public static Brush ButtonPointerOverBorder => ColorExtensions.AppBrush("TextBoxButtonPointerOverBorderThemeBrush");
        public static Brush ButtonPointerOverForeground => ColorExtensions.AppBrush("TextBoxButtonPointerOverForegroundThemeBrush");
        public static Brush ButtonPressedBackground => ColorExtensions.AppBrush("TextBoxButtonPressedBackgroundThemeBrush");
        public static Brush ButtonPressedBorder => ColorExtensions.AppBrush("TextBoxButtonPressedBorderThemeBrush");
        public static Brush ButtonPressedForeground => ColorExtensions.AppBrush("TextBoxButtonPressedForegroundThemeBrush");
        public static Brush DisabledBackground => ColorExtensions.AppBrush("TextBoxDisabledBackgroundThemeBrush");
        public static Brush DisabledBorder => ColorExtensions.AppBrush("TextBoxDisabledBorderThemeBrush");
        public static Brush DisabledForeground => ColorExtensions.AppBrush("TextBoxDisabledForegroundThemeBrush");
        public static Brush Foreground => ColorExtensions.AppBrush("TextBoxForegroundThemeBrush");
    }
}