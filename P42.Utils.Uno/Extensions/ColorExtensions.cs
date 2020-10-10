using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    public static class ColorExtensions
    {
        public static SolidColorBrush ToBrush(this Color color)
            => new SolidColorBrush(color);
    }
}
