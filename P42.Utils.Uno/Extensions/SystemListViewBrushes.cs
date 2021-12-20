using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{

    public static class SystemListViewBrushes
    {
        public static Brush GroupHeaderForeground => ColorExtensions.AppBrush("ListViewGroupHeaderForegroundThemeBrush");
        public static Brush GroupHeaderPointerOverForeground => ColorExtensions.AppBrush("ListViewGroupHeaderPointerOverForegroundThemeBrush");
        public static Brush GroupHeaderPressedForeground => ColorExtensions.AppBrush("ListViewGroupHeaderPressedForegroundThemeBrush");
        public static Brush ItemCheckHint => ColorExtensions.AppBrush("ListViewItemCheckHintThemeBrush");
        public static Brush ItemCheckSelecting => ColorExtensions.AppBrush("ListViewItemCheckSelectingThemeBrush");
        public static Brush ItemCheck => ColorExtensions.AppBrush("ListViewItemCheckThemeBrush");
        public static Brush ItemDragBackground => ColorExtensions.AppBrush("ListViewItemDragBackgroundThemeBrush");
        public static Brush ItemDragForeground => ColorExtensions.AppBrush("ListViewItemDragForegroundThemeBrush");
        public static Brush ItemFocusBorder => ColorExtensions.AppBrush("ListViewItemFocusBorderThemeBrush");
        public static Brush ItemOverlayBackground => ColorExtensions.AppBrush("ListViewItemOverlayBackgroundThemeBrush");
        public static Brush ItemOverlayForeground => ColorExtensions.AppBrush("ListViewItemOverlayForegroundThemeBrush");
        public static Brush ItemOverlaySecondaryForeground => ColorExtensions.AppBrush("ListViewItemOverlaySecondaryForegroundThemeBrush");
        public static Brush ItemPlaceholderBackground => ColorExtensions.AppBrush("ListViewItemPlaceholderBackgroundThemeBrush");
        public static Brush ItemPointerOverBackground => ColorExtensions.AppBrush("ListViewItemPointerOverBackgroundThemeBrush");
        public static Brush ItemSelectedBackground => ColorExtensions.AppBrush("ListViewItemSelectedBackgroundThemeBrush");
        public static Brush ItemSelectedForeground => ColorExtensions.AppBrush("ListViewItemSelectedForegroundThemeBrush");
        public static Brush ItemSelectedPointerOverBackground => ColorExtensions.AppBrush("ListViewItemSelectedPointerOverBackgroundThemeBrush");
        public static Brush ItemSelectedPointerOverBorder => ColorExtensions.AppBrush("ListViewItemSelectedPointerOverBorderThemeBrush");
    }
}