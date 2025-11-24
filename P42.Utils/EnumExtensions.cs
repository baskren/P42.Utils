using System.Globalization;

namespace P42.Utils;

// ReSharper disable once UnusedType.Global
public static class EnumExtensions
{
    /// <summary>
    /// Get int value for an Enum
    /// </summary>
    /// <param name="enumValue"></param>
    /// <returns></returns>
    public static int ToInt(this Enum enumValue) => Convert.ToInt32(enumValue, CultureInfo.InvariantCulture);
}
