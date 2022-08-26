using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace P42.Utils.Uno
{
    public static class CornerRadiusExtensions
    {
        public static double Average(this CornerRadius cornerRadius)
            => (cornerRadius.TopLeft + cornerRadius.TopRight + cornerRadius.BottomLeft + cornerRadius.BottomRight) / 4.0;
    }
}
