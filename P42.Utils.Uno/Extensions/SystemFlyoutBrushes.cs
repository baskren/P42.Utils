using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Microsoft.UI.Xaml.Media;

namespace P42.Utils.Uno
{

	public static class SystemFlyoutBrushes
	{
		public static Brush Background => ColorExtensions.AppBrush("FlyoutBackgroundThemeBrush");
		public static Brush Border => ColorExtensions.AppBrush("FlyoutBorderThemeBrush");
	}
}