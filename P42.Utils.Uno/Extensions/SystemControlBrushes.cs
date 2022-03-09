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
    public static class SystemControlBrushes
    {
        public static Brush Transparent => ColorExtensions.AppBrush("SystemControlTransparentBrush");
        public static Brush BackgroundAccent => ColorExtensions.AppBrush("SystemControlBackgroundAccentBrush");
        public static Brush BackgroundAltHigh => ColorExtensions.AppBrush("SystemControlBackgroundAltHighBrush");
        public static Brush BackgroundAltMediumHigh => ColorExtensions.AppBrush("SystemControlBackgroundAltMediumHighBrush");
        public static Brush BackgroundAltMedium => ColorExtensions.AppBrush("SystemControlBackgroundAltMediumBrush");
        public static Brush BackgroundAltMediumLow => ColorExtensions.AppBrush("SystemControlBackgroundAltMediumLowBrush");
        public static Brush BackgroundBaseHigh => ColorExtensions.AppBrush("SystemControlBackgroundBaseHighBrush");
        public static Brush BackgroundBaseLow => ColorExtensions.AppBrush("SystemControlBackgroundBaseLowBrush");
        public static Brush BackgroundBaseMedium => ColorExtensions.AppBrush("SystemControlBackgroundBaseMediumBrush");
        public static Brush BackgroundBaseMediumHigh => ColorExtensions.AppBrush("SystemControlBackgroundBaseMediumHighBrush");
        public static Brush BackgroundBaseMediumLow => ColorExtensions.AppBrush("SystemControlBackgroundBaseMediumLowBrush");
        public static Brush BackgroundChromeBlackHigh => ColorExtensions.AppBrush("SystemControlBackgroundChromeBlackHighBrush");
        public static Brush BackgroundChromeBlackMedium => ColorExtensions.AppBrush("SystemControlBackgroundChromeBlackMediumBrush");
        public static Brush BackgroundChromeBlackLow => ColorExtensions.AppBrush("SystemControlBackgroundChromeBlackLowBrush");
        public static Brush BackgroundChromeBlackMediumLow => ColorExtensions.AppBrush("SystemControlBackgroundChromeBlackMediumLowBrush");
        public static Brush BackgroundChromeMedium => ColorExtensions.AppBrush("SystemControlBackgroundChromeMediumBrush");
        public static Brush BackgroundChromeMediumLow => ColorExtensions.AppBrush("SystemControlBackgroundChromeMediumLowBrush");
        public static Brush BackgroundChromeWhite => ColorExtensions.AppBrush("SystemControlBackgroundChromeWhiteBrush");
        public static Brush BackgroundListLow => ColorExtensions.AppBrush("SystemControlBackgroundListLowBrush");
        public static Brush BackgroundListMedium => ColorExtensions.AppBrush("SystemControlBackgroundListMediumBrush");
        public static Brush DisabledAccent => ColorExtensions.AppBrush("SystemControlDisabledAccentBrush");
        public static Brush DisabledBaseHigh => ColorExtensions.AppBrush("SystemControlDisabledBaseHighBrush");
        public static Brush DisabledBaseLow => ColorExtensions.AppBrush("SystemControlDisabledBaseLowBrush");
        public static Brush DisabledBaseMediumLow => ColorExtensions.AppBrush("SystemControlDisabledBaseMediumLowBrush");
        public static Brush DisabledChromeDisabledHigh => ColorExtensions.AppBrush("SystemControlDisabledChromeDisabledHighBrush");
        public static Brush DisabledChromeDisabledLow => ColorExtensions.AppBrush("SystemControlDisabledChromeDisabledLowBrush");
        public static Brush DisabledChromeHigh => ColorExtensions.AppBrush("SystemControlDisabledChromeHighBrush");
        public static Brush DisabledChromeMediumLow => ColorExtensions.AppBrush("SystemControlDisabledChromeMediumLowBrush");
        public static Brush DisabledListMedium => ColorExtensions.AppBrush("SystemControlDisabledListMediumBrush");
        public static Brush DisabledTransparent => ColorExtensions.AppBrush("SystemControlDisabledTransparentBrush");
        public static Brush ForegroundAccent => ColorExtensions.AppBrush("SystemControlForegroundAccentBrush");
        public static Brush ForegroundAltHigh => ColorExtensions.AppBrush("SystemControlForegroundAltHighBrush");
        public static Brush ForegroundAltMediumHigh => ColorExtensions.AppBrush("SystemControlForegroundAltMediumHighBrush");
        public static Brush ForegroundBaseHigh => ColorExtensions.AppBrush("SystemControlForegroundBaseHighBrush");
        public static Brush ForegroundBaseLow => ColorExtensions.AppBrush("SystemControlForegroundBaseLowBrush");
        public static Brush ForegroundBaseMedium => ColorExtensions.AppBrush("SystemControlForegroundBaseMediumBrush");
        public static Brush ForegroundBaseMediumHigh => ColorExtensions.AppBrush("SystemControlForegroundBaseMediumHighBrush");
        public static Brush ForegroundBaseMediumLow => ColorExtensions.AppBrush("SystemControlForegroundBaseMediumLowBrush");
        public static Brush ForegroundChromeBlackHigh => ColorExtensions.AppBrush("SystemControlForegroundChromeBlackHighBrush");
        public static Brush ForegroundChromeHigh => ColorExtensions.AppBrush("SystemControlForegroundChromeHighBrush");
        public static Brush ForegroundChromeMedium => ColorExtensions.AppBrush("SystemControlForegroundChromeMediumBrush");
        public static Brush ForegroundChromeWhite => ColorExtensions.AppBrush("SystemControlForegroundChromeWhiteBrush");
        public static Brush ForegroundChromeDisabledLow => ColorExtensions.AppBrush("SystemControlForegroundChromeDisabledLowBrush");
        public static Brush ForegroundChromeGray => ColorExtensions.AppBrush("SystemControlForegroundChromeGrayBrush");
        public static Brush ForegroundListLow => ColorExtensions.AppBrush("SystemControlForegroundListLowBrush");
        public static Brush ForegroundListMedium => ColorExtensions.AppBrush("SystemControlForegroundListMediumBrush");
        public static Brush ForegroundTransparent => ColorExtensions.AppBrush("SystemControlForegroundTransparentBrush");
        public static Brush ForegroundChromeBlackMedium => ColorExtensions.AppBrush("SystemControlForegroundChromeBlackMediumBrush");
        public static Brush ForegroundChromeBlackMediumLow => ColorExtensions.AppBrush("SystemControlForegroundChromeBlackMediumLowBrush");
        public static Brush HighlightAccent => ColorExtensions.AppBrush("SystemControlHighlightAccentBrush");
        public static Brush HighlightAltAccent => ColorExtensions.AppBrush("SystemControlHighlightAltAccentBrush");
        public static Brush HighlightAltAltHigh => ColorExtensions.AppBrush("SystemControlHighlightAltAltHighBrush");
        public static Brush HighlightAltBaseHigh => ColorExtensions.AppBrush("SystemControlHighlightAltBaseHighBrush");
        public static Brush HighlightAltBaseLow => ColorExtensions.AppBrush("SystemControlHighlightAltBaseLowBrush");
        public static Brush HighlightAltBaseMedium => ColorExtensions.AppBrush("SystemControlHighlightAltBaseMediumBrush");
        public static Brush HighlightAltBaseMediumHigh => ColorExtensions.AppBrush("SystemControlHighlightAltBaseMediumHighBrush");
        public static Brush HighlightAltAltMediumHigh => ColorExtensions.AppBrush("SystemControlHighlightAltAltMediumHighBrush");
        public static Brush HighlightAltBaseMediumLow => ColorExtensions.AppBrush("SystemControlHighlightAltBaseMediumLowBrush");
        public static Brush HighlightAltListAccentHigh => ColorExtensions.AppBrush("SystemControlHighlightAltListAccentHighBrush");
        public static Brush HighlightAltListAccentLow => ColorExtensions.AppBrush("SystemControlHighlightAltListAccentLowBrush");
        public static Brush HighlightAltListAccentMedium => ColorExtensions.AppBrush("SystemControlHighlightAltListAccentMediumBrush");
        public static Brush HighlightAltChromeWhite => ColorExtensions.AppBrush("SystemControlHighlightAltChromeWhiteBrush");
        public static Brush HighlightAltTransparent => ColorExtensions.AppBrush("SystemControlHighlightAltTransparentBrush");
        public static Brush HighlightBaseHigh => ColorExtensions.AppBrush("SystemControlHighlightBaseHighBrush");
        public static Brush HighlightBaseLow => ColorExtensions.AppBrush("SystemControlHighlightBaseLowBrush");
        public static Brush HighlightBaseMedium => ColorExtensions.AppBrush("SystemControlHighlightBaseMediumBrush");
        public static Brush HighlightBaseMediumHigh => ColorExtensions.AppBrush("SystemControlHighlightBaseMediumHighBrush");
        public static Brush HighlightBaseMediumLow => ColorExtensions.AppBrush("SystemControlHighlightBaseMediumLowBrush");
        public static Brush HighlightChromeAltLow => ColorExtensions.AppBrush("SystemControlHighlightChromeAltLowBrush");
        public static Brush HighlightChromeHigh => ColorExtensions.AppBrush("SystemControlHighlightChromeHighBrush");
        public static Brush HighlightListAccentHigh => ColorExtensions.AppBrush("SystemControlHighlightListAccentHighBrush");
        public static Brush HighlightListAccentLow => ColorExtensions.AppBrush("SystemControlHighlightListAccentLowBrush");
        public static Brush HighlightListAccentMedium => ColorExtensions.AppBrush("SystemControlHighlightListAccentMediumBrush");
        public static Brush HighlightListMedium => ColorExtensions.AppBrush("SystemControlHighlightListMediumBrush");
        public static Brush HighlightListLow => ColorExtensions.AppBrush("SystemControlHighlightListLowBrush");
        public static Brush HighlightChromeWhite => ColorExtensions.AppBrush("SystemControlHighlightChromeWhiteBrush");
        public static Brush HighlightTransparent => ColorExtensions.AppBrush("SystemControlHighlightTransparentBrush");
        public static Brush HyperlinkText => ColorExtensions.AppBrush("SystemControlHyperlinkTextBrush");
        public static Brush HyperlinkBaseHigh => ColorExtensions.AppBrush("SystemControlHyperlinkBaseHighBrush");
        public static Brush HyperlinkBaseMedium => ColorExtensions.AppBrush("SystemControlHyperlinkBaseMediumBrush");
        public static Brush HyperlinkBaseMediumHigh => ColorExtensions.AppBrush("SystemControlHyperlinkBaseMediumHighBrush");
        public static Brush PageBackgroundAltMedium => ColorExtensions.AppBrush("SystemControlPageBackgroundAltMediumBrush");
        public static Brush PageBackgroundAltHigh => ColorExtensions.AppBrush("SystemControlPageBackgroundAltHighBrush");
        public static Brush PageBackgroundMediumAltMedium => ColorExtensions.AppBrush("SystemControlPageBackgroundMediumAltMediumBrush");
        public static Brush PageBackgroundBaseLow => ColorExtensions.AppBrush("SystemControlPageBackgroundBaseLowBrush");
        public static Brush PageBackgroundBaseMedium => ColorExtensions.AppBrush("SystemControlPageBackgroundBaseMediumBrush");
        public static Brush PageBackgroundListLow => ColorExtensions.AppBrush("SystemControlPageBackgroundListLowBrush");
        public static Brush PageBackgroundChromeLow => ColorExtensions.AppBrush("SystemControlPageBackgroundChromeLowBrush");
        public static Brush PageBackgroundChromeMediumLow => ColorExtensions.AppBrush("SystemControlPageBackgroundChromeMediumLowBrush");
        public static Brush PageBackgroundTransparent => ColorExtensions.AppBrush("SystemControlPageBackgroundTransparentBrush");
        public static Brush PageTextBaseHigh => ColorExtensions.AppBrush("SystemControlPageTextBaseHighBrush");
        public static Brush PageTextBaseMedium => ColorExtensions.AppBrush("SystemControlPageTextBaseMediumBrush");
        public static Brush PageTextChromeBlackMediumLow => ColorExtensions.AppBrush("SystemControlPageTextChromeBlackMediumLowBrush");
        public static Brush ErrorTextForeground => ColorExtensions.AppBrush("SystemControlErrorTextForegroundBrush");
        public static Brush FocusVisualPrimary => ColorExtensions.AppBrush("SystemControlFocusVisualPrimaryBrush");
        public static Brush FocusVisualSecondary => ColorExtensions.AppBrush("SystemControlFocusVisualSecondaryBrush");
        public static Brush BackgroundBaseLowRevealBackground => ColorExtensions.AppBrush("SystemControlBackgroundBaseLowRevealBackgroundBrush");
        public static Brush BackgroundBaseMediumLowRevealBaseLowBackground => ColorExtensions.AppBrush("SystemControlBackgroundBaseMediumLowRevealBaseLowBackgroundBrush");
        public static Brush BackgroundTransparentRevealBorder => ColorExtensions.AppBrush("SystemControlBackgroundTransparentRevealBorderBrush");
        public static Brush HighlightBaseMediumLowRevealBorder => ColorExtensions.AppBrush("SystemControlHighlightBaseMediumLowRevealBorderBrush");
        public static Brush HighlightTransparentRevealBorder => ColorExtensions.AppBrush("SystemControlHighlightTransparentRevealBorderBrush");
        public static Brush TransparentRevealBorder => ColorExtensions.AppBrush("SystemControlTransparentRevealBorderBrush");
        public static Brush HighlightAltTransparentRevealBorder => ColorExtensions.AppBrush("SystemControlHighlightAltTransparentRevealBorderBrush");
        public static Brush HighlightBaseMediumLowRevealAccentBackground => ColorExtensions.AppBrush("SystemControlHighlightBaseMediumLowRevealAccentBackgroundBrush");
        public static Brush HighlightAccent3RevealBackground => ColorExtensions.AppBrush("SystemControlHighlightAccent3RevealBackgroundBrush");
        public static Brush HighlightAccent2RevealBackground => ColorExtensions.AppBrush("SystemControlHighlightAccent2RevealBackgroundBrush");
        public static Brush HighlightAccentRevealBackground => ColorExtensions.AppBrush("SystemControlHighlightAccentRevealBackgroundBrush");
        public static Brush HighlightAccent3RevealAccent2Background => ColorExtensions.AppBrush("SystemControlHighlightAccent3RevealAccent2BackgroundBrush");
        public static Brush TransientBackground => ColorExtensions.AppBrush("SystemControlTransientBackgroundBrush");
        public static Brush AcrylicElement => ColorExtensions.AppBrush("SystemControlAcrylicElementBrush");
        public static Brush ChromeMediumLowAcrylicElementMedium => ColorExtensions.AppBrush("SystemControlChromeMediumLowAcrylicElementMediumBrush");
        public static Brush ChromeMediumLowAcrylicWindowMedium => ColorExtensions.AppBrush("SystemControlChromeMediumLowAcrylicWindowMediumBrush");
        public static Brush TransparentRevealBackground => ColorExtensions.AppBrush("SystemControlTransparentRevealBackgroundBrush");
        public static Brush HighlightListLowRevealBackground => ColorExtensions.AppBrush("SystemControlHighlightListLowRevealBackgroundBrush");
        public static Brush HighlightListMediumRevealBackground => ColorExtensions.AppBrush("SystemControlHighlightListMediumRevealBackgroundBrush");
    }
}