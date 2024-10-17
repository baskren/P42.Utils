using Microsoft.UI.Xaml;

namespace P42.Utils.Uno;

public static class CornerRadiusExtensions
{
    /// <summary>
    /// Get average corner radius
    /// </summary>
    /// <param name="cornerRadius"></param>
    /// <returns></returns>
    public static double Average(this CornerRadius cornerRadius)
        => (cornerRadius.TopLeft + cornerRadius.TopRight + cornerRadius.BottomLeft + cornerRadius.BottomRight) / 4.0;
}
