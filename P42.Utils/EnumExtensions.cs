using System.Globalization;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class EnumExtensions
{
    public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);
}
