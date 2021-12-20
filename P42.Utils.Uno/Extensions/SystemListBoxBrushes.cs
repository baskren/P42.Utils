using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{

    public static class SystemListBoxBrushes
    {
        public static Brush Background => ColorExtensions.AppBrush("ListBoxBackgroundThemeBrush");
        public static Brush Border => ColorExtensions.AppBrush("ListBoxBorderThemeBrush");
        public static Brush DisabledForeground => ColorExtensions.AppBrush("ListBoxDisabledForegroundThemeBrush");
        public static Brush FocusBackground => ColorExtensions.AppBrush("ListBoxFocusBackgroundThemeBrush");
        public static Brush Foreground => ColorExtensions.AppBrush("ListBoxForegroundThemeBrush");
        public static Brush ItemDisabledForeground => ColorExtensions.AppBrush("ListBoxItemDisabledForegroundThemeBrush");
        public static Brush ItemPointerOverBackground => ColorExtensions.AppBrush("ListBoxItemPointerOverBackgroundThemeBrush");
        public static Brush ItemPointerOverForeground => ColorExtensions.AppBrush("ListBoxItemPointerOverForegroundThemeBrush");
        public static Brush ItemPressedBackground => ColorExtensions.AppBrush("ListBoxItemPressedBackgroundThemeBrush");
        public static Brush ItemPressedForeground => ColorExtensions.AppBrush("ListBoxItemPressedForegroundThemeBrush");
        public static Brush ItemSelectedDisabledBackground => ColorExtensions.AppBrush("ListBoxItemSelectedDisabledBackgroundThemeBrush");
        public static Brush ItemSelectedDisabledForeground => ColorExtensions.AppBrush("ListBoxItemSelectedDisabledForegroundThemeBrush");
        public static Brush ItemSelectedBackground => ColorExtensions.AppBrush("ListBoxItemSelectedBackgroundThemeBrush");
        public static Brush ItemSelectedForeground => ColorExtensions.AppBrush("ListBoxItemSelectedForegroundThemeBrush");
        public static Brush ItemSelectedPointerOverBackground => ColorExtensions.AppBrush("ListBoxItemSelectedPointerOverBackgroundThemeBrush");
    }
}