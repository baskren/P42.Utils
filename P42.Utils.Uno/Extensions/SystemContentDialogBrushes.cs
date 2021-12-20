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
    public static class SystemContentDialogBrushes
    {
        public static Brush Background => ColorExtensions.AppBrush("ContentDialogBackgroundThemeBrush");
        public static Brush Border => ColorExtensions.AppBrush("ContentDialogBorderThemeBrush");
        public static Brush ContentForeg => ColorExtensions.AppBrush("ContentDialogContentForegroundBrush");
        public static Brush Dimming => ColorExtensions.AppBrush("ContentDialogDimmingThemeBrush");
    }
}