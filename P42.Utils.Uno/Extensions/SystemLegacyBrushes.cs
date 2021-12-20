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
    public static class SystemLegacyBrushes
    {
        public static Brush AutoSuggestBackgroundTheme => ColorExtensions.AppBrush("AutoSuggestBackgroundThemeBrush");
        public static Brush ApplicationForegroundTheme => ColorExtensions.AppBrush("ApplicationForegroundThemeBrush");
        public static Brush ApplicationHeaderForegroundTheme => ColorExtensions.AppBrush("ApplicationHeaderForegroundThemeBrush");
        public static Brush ApplicationPageBackgroundTheme => ColorExtensions.AppBrush("ApplicationPageBackgroundThemeBrush");
        public static Brush ApplicationPointerOverForegroundTheme => ColorExtensions.AppBrush("ApplicationPointerOverForegroundThemeBrush");
        public static Brush ApplicationPressedForegroundTheme => ColorExtensions.AppBrush("ApplicationPressedForegroundThemeBrush");
        public static Brush ApplicationSecondaryForegroundTheme => ColorExtensions.AppBrush("ApplicationSecondaryForegroundThemeBrush");
        public static Brush BackButtonBackgroundTheme => ColorExtensions.AppBrush("BackButtonBackgroundThemeBrush");
        public static Brush BackButtonDisabledForegroundTheme => ColorExtensions.AppBrush("BackButtonDisabledForegroundThemeBrush");
        public static Brush BackButtonForegroundTheme => ColorExtensions.AppBrush("BackButtonForegroundThemeBrush");
        public static Brush BackButtonPointerOverBackgroundTheme => ColorExtensions.AppBrush("BackButtonPointerOverBackgroundThemeBrush");
        public static Brush BackButtonPointerOverForegroundTheme => ColorExtensions.AppBrush("BackButtonPointerOverForegroundThemeBrush");
        public static Brush BackButtonPressedForegroundTheme => ColorExtensions.AppBrush("BackButtonPressedForegroundThemeBrush");
        public static Brush ButtonBackgroundTheme => ColorExtensions.AppBrush("ButtonBackgroundThemeBrush");
        public static Brush ButtonBorderTheme => ColorExtensions.AppBrush("ButtonBorderThemeBrush");
        public static Brush ButtonDisabledBackgroundTheme => ColorExtensions.AppBrush("ButtonDisabledBackgroundThemeBrush");
        public static Brush ButtonDisabledBorderTheme => ColorExtensions.AppBrush("ButtonDisabledBorderThemeBrush");
        public static Brush ButtonDisabledForegroundTheme => ColorExtensions.AppBrush("ButtonDisabledForegroundThemeBrush");
        public static Brush ButtonForegroundTheme => ColorExtensions.AppBrush("ButtonForegroundThemeBrush");
        public static Brush ButtonPointerOverBackgroundTheme => ColorExtensions.AppBrush("ButtonPointerOverBackgroundThemeBrush");
        public static Brush ButtonPointerOverForegroundTheme => ColorExtensions.AppBrush("ButtonPointerOverForegroundThemeBrush");
        public static Brush ButtonPressedBackgroundTheme => ColorExtensions.AppBrush("ButtonPressedBackgroundThemeBrush");
        public static Brush ButtonPressedForegroundTheme => ColorExtensions.AppBrush("ButtonPressedForegroundThemeBrush");
    }
}