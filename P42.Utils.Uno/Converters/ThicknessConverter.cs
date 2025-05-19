using Microsoft.UI.Xaml.Data;

namespace P42.Utils.Uno;

public class ThicknessConverter : IValueConverter
{
    public static readonly ThicknessConverter Instance = new ThicknessConverter();

    private static readonly Thickness Zero = new(0);
    internal ThicknessConverter() { }

    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        var fallback = Zero;
        if (parameter is Thickness param)
            fallback = param;

        switch (value)
        {
            case bool tf:
                return tf ? fallback : Zero;
            case int intValue:
                return new Thickness(intValue);
            case double doubleValue:
                return new Thickness(doubleValue);
            case System.Drawing.Size sdSize:
                return new Thickness(sdSize.Width, sdSize.Height, sdSize.Width, sdSize.Height);
            case Windows.Foundation.Size wfSize:
                return new Thickness(wfSize.Width, wfSize.Height, wfSize.Width, wfSize.Height);
            case Thickness thickness:
                return thickness;
            case string s:
            {
                var parts = s.Split(',');
                List<double> values = [];
                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];
                    if (string.IsNullOrWhiteSpace(part))
                        continue;
                    if (part.Contains('"'))
                        part = part.Replace("\"", "");
                    part = part.Trim();

                    if (double.TryParse(part, out var val))
                        values.Add(val);
                    else
                        values.Add(0);
                }

                return values.Count switch
                {
                    0 => fallback,
                    1 => new Thickness(values[0]),
                    2 => new Thickness(values[0], values[1], values[0], values[1]),
                    3 => new Thickness(values[0], values[1], values[2], 0),
                    _ => new Thickness(values[0], values[1], values[2], values[3])
                };
            }
            default:
                return fallback;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is not Thickness thickness)
            return new ArgumentException("ThicknessConverter.ConvertBack must have Thickness for the value argument");
        
        var fallback = Zero;
        if (parameter is Thickness param)
            fallback = param;

        if (targetType == typeof(Thickness))
            return thickness;
        if (targetType == typeof(string))
            return thickness.ToString();
        if (targetType == typeof(double))
            return thickness.Average();
        if (targetType == typeof(int))
            return (int)thickness.Average();
        if (targetType == typeof(System.Drawing.Size))
            return new System.Drawing.Size((int)((thickness.Left+thickness.Right)/2), (int)((thickness.Top+thickness.Bottom)/2));
        if (targetType == typeof(Windows.Foundation.Size))
            return new Windows.Foundation.Size((thickness.Left+thickness.Right)/2, (thickness.Top+thickness.Bottom)/2);
        if (targetType == typeof(bool))
            return !thickness.Equals(fallback);
        
        throw new ArgumentException($"Cannot convert {value.GetType()}:[{value}] to {targetType}");
    }
}
