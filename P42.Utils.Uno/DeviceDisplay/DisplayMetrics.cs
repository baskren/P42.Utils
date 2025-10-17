using Uno;
using Windows.Graphics.Display;

namespace P42.Utils.Uno;

#if HAS_UNO
[Preserve(AllMembers = true)]
#endif
public readonly struct DisplayMetrics(
    double width,
    double height,
    double density,
    DisplayOrientation orientation,
    DisplayRotation rotation)
    : IEquatable<DisplayMetrics>
{

    
    public double Width { get; } = width;

    public double Height { get; } = height;

    public double Density { get; } = density;

    public DisplayOrientation Orientation { get; } = orientation;

    public DisplayRotation Rotation { get; } = rotation;

    public static bool operator ==(DisplayMetrics left, DisplayMetrics right) =>
        left.Equals(right);

    public static bool operator !=(DisplayMetrics left, DisplayMetrics right) =>
        !left.Equals(right);

    public override bool Equals(object? obj) =>
        obj is DisplayMetrics metrics && Equals(metrics);

    public bool Equals(DisplayMetrics other) =>
        Width.Equals(other.Width) &&
        Height.Equals(other.Height) &&
        Density.Equals(other.Density) &&
        Orientation.Equals(other.Orientation) &&
        Rotation.Equals(other.Rotation);

    public override int GetHashCode() =>
        (Height, Width, Density, Orientation, Rotation).GetHashCode();

    public override string ToString() =>
        $"{nameof(Height)}: {Height}, {nameof(Width)}: {Width}, {nameof(Density)}: {Density}, {nameof(Orientation)}: {Orientation}, {nameof(Rotation)}: {Rotation}";

    public DisplayMetrics Copy() => new(Width, Height, Density, Orientation, Rotation);
    
}

internal static class DisplayInformationExtensions
{
    public static DisplayMetrics ToDisplayMetrics(this DisplayInformation di)
    {
        var rotation = CalculateRotation(di);
        var perpendicular = rotation is DisplayRotation.Rotation90 or DisplayRotation.Rotation270;

        var w = di.ScreenWidthInRawPixels;
        var h = di.ScreenHeightInRawPixels;

        return new DisplayMetrics(
            width: perpendicular ? h : w,
            height: perpendicular ? w : h,
            density: di.LogicalDpi / 96.0,
            orientation: CalculateOrientation(di),
            rotation: rotation);

    }

    private static DisplayOrientation CalculateOrientation(DisplayInformation di)
    => di.CurrentOrientation switch
    {
        DisplayOrientations.Landscape or DisplayOrientations.LandscapeFlipped => DisplayOrientation.Landscape,
        DisplayOrientations.Portrait or DisplayOrientations.PortraitFlipped => DisplayOrientation.Portrait,
        _ => DisplayOrientation.Unknown
    };


    private static DisplayRotation CalculateRotation(DisplayInformation di)
        => di.NativeOrientation switch
        {
            DisplayOrientations.Landscape => di.CurrentOrientation switch
            {
                DisplayOrientations.Landscape => DisplayRotation.Rotation0,
                DisplayOrientations.Portrait => DisplayRotation.Rotation270,
                DisplayOrientations.LandscapeFlipped => DisplayRotation.Rotation180,
                DisplayOrientations.PortraitFlipped => DisplayRotation.Rotation90,
                _ => DisplayRotation.Unknown
            },
            _ => di.CurrentOrientation switch
            {
                DisplayOrientations.Landscape => DisplayRotation.Rotation90,
                DisplayOrientations.Portrait => DisplayRotation.Rotation0,
                DisplayOrientations.LandscapeFlipped => DisplayRotation.Rotation270,
                DisplayOrientations.PortraitFlipped => DisplayRotation.Rotation180,
                _ => DisplayRotation.Unknown
            }
        };

}
