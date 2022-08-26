using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    // Full list of brushes found here: https://github.com/unoplatform/uno/blob/master/src/Uno.UI/UI/Xaml/Style/Generic/SystemResources.xaml
    public static class SystemScrollBarBrushes
    {
        public static Brush Background => ColorExtensions.AppBrush("ScrollBarBackground");
        public static Brush BackgroundPointerOver => ColorExtensions.AppBrush("ScrollBarBackgroundPointerOver");
        public static Brush BackgroundDisabled => ColorExtensions.AppBrush("ScrollBarBackgroundDisabled");
        public static Brush Foreground => ColorExtensions.AppBrush("ScrollBarForeground");
        public static Brush BorderBrush => ColorExtensions.AppBrush("ScrollBarBorderBrush");
        public static Brush BorderBrushPointerOver => ColorExtensions.AppBrush("ScrollBarBorderBrushPointerOver");
        public static Brush BorderBrushDisabled => ColorExtensions.AppBrush("ScrollBarBorderBrushDisabled");
        public static Brush ButtonBackground => ColorExtensions.AppBrush("ScrollBarButtonBackground");
        public static Brush ButtonBackgroundPointerOver => ColorExtensions.AppBrush("ScrollBarButtonBackgroundPointerOver");
        public static Brush ButtonBackgroundPressed => ColorExtensions.AppBrush("ScrollBarButtonBackgroundPressed");
        public static Brush ButtonBackgroundDisabled => ColorExtensions.AppBrush("ScrollBarButtonBackgroundDisabled");
        public static Brush ButtonBorderBrush => ColorExtensions.AppBrush("ScrollBarButtonBorderBrush");
        public static Brush ButtonBorderBrushPointerOver => ColorExtensions.AppBrush("ScrollBarButtonBorderBrushPointerOver");
        public static Brush ButtonBorderBrushPressed => ColorExtensions.AppBrush("ScrollBarButtonBorderBrushPressed");
        public static Brush ButtonBorderBrushDisabled => ColorExtensions.AppBrush("ScrollBarButtonBorderBrushDisabled");
        public static Brush ButtonArrowForeground => ColorExtensions.AppBrush("ScrollBarButtonArrowForeground");
        public static Brush ButtonArrowForegroundPointerOver => ColorExtensions.AppBrush("ScrollBarButtonArrowForegroundPointerOver");
        public static Brush ButtonArrowForegroundPressed => ColorExtensions.AppBrush("ScrollBarButtonArrowForegroundPressed");
        public static Brush ButtonArrowForegroundDisabled => ColorExtensions.AppBrush("ScrollBarButtonArrowForegroundDisabled");
        public static Brush ThumbFill => ColorExtensions.AppBrush("ScrollBarThumbFill");
        public static Brush ThumbFillPointerOver => ColorExtensions.AppBrush("ScrollBarThumbFillPointerOver");
        public static Brush ThumbFillPressed => ColorExtensions.AppBrush("ScrollBarThumbFillPressed");
        public static Brush ThumbFillDisabled => ColorExtensions.AppBrush("ScrollBarThumbFillDisabled");
        public static Brush TrackFill => ColorExtensions.AppBrush("ScrollBarTrackFill");
        public static Brush TrackFillPointerOver => ColorExtensions.AppBrush("ScrollBarTrackFillPointerOver");
        public static Brush TrackFillDisabled => ColorExtensions.AppBrush("ScrollBarTrackFillDisabled");
        public static Brush TrackStroke => ColorExtensions.AppBrush("ScrollBarTrackStroke");
        public static Brush TrackStrokePointerOver => ColorExtensions.AppBrush("ScrollBarTrackStrokePointerOver");
        public static Brush TrackStrokeDisabled => ColorExtensions.AppBrush("ScrollBarTrackStrokeDisabled");
        public static Brush PanningThumbBackgroundDisabled => ColorExtensions.AppBrush("ScrollBarPanningThumbBackgroundDisabled");
        public static Brush ThumbBackgroundColor => ColorExtensions.AppBrush("ScrollBarThumbBackgroundColor");
        public static Brush PanningThumbBackgroundColor => ColorExtensions.AppBrush("ScrollBarPanningThumbBackgroundColor");
        public static Brush ThumbBackground => ColorExtensions.AppBrush("ScrollBarThumbBackground");
        public static Brush PanningThumbBackground => ColorExtensions.AppBrush("ScrollBarPanningThumbBackground");
    }
}