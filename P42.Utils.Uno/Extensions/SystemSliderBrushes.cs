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
    public static class SystemSliderBrushes
    {
        public static Brush ContainerBackground => ColorExtensions.AppBrush("SliderContainerBackground");
        public static Brush ContainerBackgroundPointerOver => ColorExtensions.AppBrush("SliderContainerBackgroundPointerOver");
        public static Brush ContainerBackgroundPressed => ColorExtensions.AppBrush("SliderContainerBackgroundPressed");
        public static Brush ContainerBackgroundDisabled => ColorExtensions.AppBrush("SliderContainerBackgroundDisabled");
        //public static Brush ThumbBackground => ColorExtensions.AppBrush("SliderThumbBackground");
        public static Brush ThumbBackgroundPointerOver => ColorExtensions.AppBrush("SliderThumbBackgroundPointerOver");
        public static Brush ThumbBackgroundPressed => ColorExtensions.AppBrush("SliderThumbBackgroundPressed");
        public static Brush ThumbBackgroundDisabled => ColorExtensions.AppBrush("SliderThumbBackgroundDisabled");
        public static Brush TrackFill => ColorExtensions.AppBrush("SliderTrackFill");
        public static Brush TrackFillPointerOver => ColorExtensions.AppBrush("SliderTrackFillPointerOver");
        public static Brush TrackFillPressed => ColorExtensions.AppBrush("SliderTrackFillPressed");
        public static Brush TrackFillDisabled => ColorExtensions.AppBrush("SliderTrackFillDisabled");
        public static Brush TrackValueFill => ColorExtensions.AppBrush("SliderTrackValueFill");
        public static Brush TrackValueFillPointerOver => ColorExtensions.AppBrush("SliderTrackValueFillPointerOver");
        public static Brush TrackValueFillPressed => ColorExtensions.AppBrush("SliderTrackValueFillPressed");
        public static Brush TrackValueFillDisabled => ColorExtensions.AppBrush("SliderTrackValueFillDisabled");
        //public static Brush HeaderForeground => ColorExtensions.AppBrush("SliderHeaderForeground");
        public static Brush HeaderForegroundDisabled => ColorExtensions.AppBrush("SliderHeaderForegroundDisabled");
        public static Brush TickBarFill => ColorExtensions.AppBrush("SliderTickBarFill");
        public static Brush TickBarFillDisabled => ColorExtensions.AppBrush("SliderTickBarFillDisabled");
        public static Brush InlineTickBarFill => ColorExtensions.AppBrush("SliderInlineTickBarFill");

        public static Brush Border => ColorExtensions.AppBrush("SliderBorderThemeBrush");
        public static Brush DisabledBorder => ColorExtensions.AppBrush("SliderDisabledBorderThemeBrush");
        public static Brush ThumbBackground => ColorExtensions.AppBrush("SliderThumbBackgroundThemeBrush");
        public static Brush ThumbBorder => ColorExtensions.AppBrush("SliderThumbBorderThemeBrush");
        public static Brush ThumbDisabledBackground => ColorExtensions.AppBrush("SliderThumbDisabledBackgroundThemeBrush");
        public static Brush ThumbPointerOverBackground => ColorExtensions.AppBrush("SliderThumbPointerOverBackgroundThemeBrush");
        public static Brush ThumbPointerOverBorder => ColorExtensions.AppBrush("SliderThumbPointerOverBorderThemeBrush");
        public static Brush ThumbPressedBackground => ColorExtensions.AppBrush("SliderThumbPressedBackgroundThemeBrush");
        public static Brush ThumbPressedBorder => ColorExtensions.AppBrush("SliderThumbPressedBorderThemeBrush");
        public static Brush TickMarkInlineBackground => ColorExtensions.AppBrush("SliderTickMarkInlineBackgroundThemeBrush");
        public static Brush TickMarkInlineDisabledForeground => ColorExtensions.AppBrush("SliderTickMarkInlineDisabledForegroundThemeBrush");
        public static Brush TickmarkOutsideBackground => ColorExtensions.AppBrush("SliderTickmarkOutsideBackgroundThemeBrush");
        public static Brush TickMarkOutsideDisabledForeground => ColorExtensions.AppBrush("SliderTickMarkOutsideDisabledForegroundThemeBrush");
        public static Brush TrackBackground => ColorExtensions.AppBrush("SliderTrackBackgroundThemeBrush");
        public static Brush TrackDecreaseBackground => ColorExtensions.AppBrush("SliderTrackDecreaseBackgroundThemeBrush");
        public static Brush TrackDecreaseDisabledBackground => ColorExtensions.AppBrush("SliderTrackDecreaseDisabledBackgroundThemeBrush");
        public static Brush TrackDecreasePointerOverBackground => ColorExtensions.AppBrush("SliderTrackDecreasePointerOverBackgroundThemeBrush");
        public static Brush TrackDecreasePressedBackground => ColorExtensions.AppBrush("SliderTrackDecreasePressedBackgroundThemeBrush");
        public static Brush TrackDisabledBackground => ColorExtensions.AppBrush("SliderTrackDisabledBackgroundThemeBrush");
        public static Brush TrackPointerOverBackground => ColorExtensions.AppBrush("SliderTrackPointerOverBackgroundThemeBrush");
        public static Brush TrackPressedBackground => ColorExtensions.AppBrush("SliderTrackPressedBackgroundThemeBrush");
        public static Brush HeaderForeground => ColorExtensions.AppBrush("SliderHeaderForegroundThemeBrush");
    }
}