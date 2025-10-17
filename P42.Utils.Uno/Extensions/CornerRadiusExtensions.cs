namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public static class CornerRadiusExtensions
{
    /// <summary>
    /// Gets the average of the corner radii.
    /// </summary>
    /// <param name="cornerRadius"></param>
    /// <returns></returns>
    public static double Average(this CornerRadius cornerRadius)
        => (cornerRadius.TopLeft + cornerRadius.TopRight + cornerRadius.BottomLeft + cornerRadius.BottomRight) / 4.0;
}
