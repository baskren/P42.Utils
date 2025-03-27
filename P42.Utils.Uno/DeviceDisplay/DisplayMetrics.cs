using System;
using Uno;

namespace P42.Utils.Uno;

[Preserve(AllMembers = true)]
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
}
