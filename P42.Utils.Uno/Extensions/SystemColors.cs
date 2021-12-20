using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace P42.Utils.Uno
{
    // XAML theme resources
    // The XAML color ramp and theme-dependent brushes
    // Light and Dark theme colors
    // https://docstaging.z5.web.core.windows.net/aleader/toolkit-7/design/controls-and-patterns/xaml-theme-resources.html#the-xaml-color-ramp-and-theme-dependent-brushes
    public static class SystemColors
    {
        #region Custom
        public static Color ListViewHeaderBackgroundColor
        {
            get
            {
                if (BaseHigh.R == 0)
                    return Color.FromArgb(0xFF, 0x99, 0x99, 0x99);
                return Color.FromArgb(0xFF, 0x66, 0x66, 0x66);
            }
        }

        public static Color ListViewCellBackgroundColor
            => AltHigh;

        public static Color PageBackgroundColor
            => AltHigh;
        #endregion


        #region ALT
        // Light: 0xFFFFFFFF Dark: 0xFF000000
        public static Color AltHigh
            => ColorExtensions.AppColor("SystemAltHighColor");

        // Light: 0x33FFFFFF Dark: 0x33000000
        public static Color AltLow
            => ColorExtensions.AppColor("SystemAltLowColor");

        // Light: 0x99FFFFFF Dark: 0x99000000
        public static Color AltMedium
            => ColorExtensions.AppColor("SystemAltMediumColor");

        // Light: 0xCCFFFFFF Dark: 0xCC000000
        public static Color AltMediumHigh
            => ColorExtensions.AppColor("SystemAltMediumHighColor");

        // Light: 0x66FFFFFF Dark: 0x66000000
        public static Color AltMediumLow 
            => ColorExtensions.AppColor("SystemAltMediumLowColor");
        #endregion


        #region BASE
        // Light: 0xFF000000 Dark 0xFFFFFFFF
        public static Color BaseHigh 
            => ColorExtensions.AppColor("SystemBaseHighColor");

        // Light: 0x33000000 Dark: 0x33FFFFFF
        public static Color BaseLow 
            => ColorExtensions.AppColor("SystemBaseLowColor");

        // Light: 0x99000000 Dark: 0x99FFFFFF
        public static Color BaseMedium 
            => ColorExtensions.AppColor("SystemBaseMediumColor");

        // Light: 0xCC000000 Dark: 0xCCFFFFFF
        public static Color BaseMediumHigh 
            => ColorExtensions.AppColor("SystemBaseMediumHighColor");

        // Light: 0x66000000 Dark: 0x66FFFFFF
        public static Color BaseMediumLow 
            => ColorExtensions.AppColor("SystemBaseMediumLowColor");
        #endregion


        #region Chrome
        // Light: 0xFF171717 Dark: 0xFFF2F2F2
        public static Color ChromeAltLow 
            => ColorExtensions.AppColor("SystemChromeAltLowColor");

        // Light: 0xFF000000 Dark: 0xFF000000
        public static Color ChromeBlackHigh 
            => ColorExtensions.AppColor("SystemChromeBlackHighColor");

        // Light: 33000000 Dark: 33000000
        public static Color ChromeBlackLow 
            => ColorExtensions.AppColor("SystemChromeBlackLowColor");

        // Light: 66000000 Dark: 66000000
        public static Color ChromeBlackMediumLow 
            => ColorExtensions.AppColor("SystemChromeBlackMediumLowColor");

        // Light: CC000000 Dark: CC000000
        public static Color ChromeBlackMedium 
            => ColorExtensions.AppColor("SystemChromeBlackMediumColor");

        // Light: FFCCCCCC Dark: FF333333
        public static Color ChromeDisabledHigh 
            => ColorExtensions.AppColor("SystemChromeDisabledHighColor");

        // Light: FF7A7A7A Dark: FF858585
        public static Color ChromeDisabledLow 
            => ColorExtensions.AppColor("SystemChromeDisabledLowColor");

        // Light: FFCCCCCC Dark: FF767676
        public static Color ChromeHigh 
            => ColorExtensions.AppColor("SystemChromeHighColor");

        public static Color ChromeLow 
            => ColorExtensions.AppColor("SystemChromeLowColor");

        public static Color ChromeMedium 
            => ColorExtensions.AppColor("SystemChromeMediumColor");

        public static Color ChromeMediumLow 
            => ColorExtensions.AppColor("SystemChromeMediumLowColor");

        public static Color ChromeWhite 
            => ColorExtensions.AppColor("SystemChromeWhiteColor");
        #endregion


        #region List
        public static Color ListLow 
            => ColorExtensions.AppColor("SystemListLowColor");

        public static Color ListMedium 
            => ColorExtensions.AppColor("SystemListMediumColor");

        public static Color ListAccentLow
            => ColorExtensions.AppColor("SystemListAccentLowColor");

        public static Color ListAccentMedium
            => ColorExtensions.AppColor("SystemListAccentMediumColor");

        public static Color ListAccentHigh
            => ColorExtensions.AppColor("SystemListAccentHighColor");
        #endregion


        #region Button
        public static Color ColorButtonFace 
            => ColorExtensions.AppColor("SystemColorButtonFaceColor");

        public static Color ColorButtonText 
            => ColorExtensions.AppColor("SystemColorButtonTextColor");
        #endregion

        #region Text
        public static Color ColorGrayText 
            => ColorExtensions.AppColor("SystemColorGrayTextColor");
        #endregion

        #region HighLight
        public static Color ColorHighlight 
            => ColorExtensions.AppColor("SystemColorHighlightColor");

        public static Color ColorHighlightText 
            => ColorExtensions.AppColor("SystemColorHighlightTextColor");
        #endregion


        #region HotLight
        public static Color ColorHotlight 
            => ColorExtensions.AppColor("SystemColorHotlightColor");
        #endregion


        #region Window
        public static Color WindowColor 
            => ColorExtensions.AppColor("SystemColorWindowColor");

        public static Color WindowTextColor 
            => ColorExtensions.AppColor("SystemColorWindowTextColor");
        #endregion


        #region Accent
        public static Color Accent 
            => ColorExtensions.AppColor("SystemAccentColor");

        public static Color AccentLight3
            => ColorExtensions.AppColor("SystemAccentColorLight3");

        public static Color AccentLight2
            => ColorExtensions.AppColor("SystemAccentColorLight2");

        public static Color AccentLight1
            => ColorExtensions.AppColor("SystemAccentColorLight1");

        public static Color AccentDark1
            => ColorExtensions.AppColor("SystemAccentColorDark1");

        public static Color AccentDark2
            => ColorExtensions.AppColor("SystemAccentColorDark2");

        public static Color AccentDark3
            => ColorExtensions.AppColor("SystemAccentColorDark3");
        #endregion

    }
}
