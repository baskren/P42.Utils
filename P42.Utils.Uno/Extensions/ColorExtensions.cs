using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace P42.Utils.Uno
{
    public static class ColorExtensions
    {
        public static Color GetForegroundColor(this Color background)
        {
            var yiq = ((background.R * 299) + (background.G * 587) + (background.B * 114)) / 1000;
            return yiq < 128 ? Colors.White : Colors.Black;
        }


        #region Tranlators
        public static Color AsWinUiColor(this System.Drawing.Color color)
            => new Color
            {
                A = color.A,
                R = color.R,
                G = color.G,
                B = color.B
            };

        public static System.Drawing.Color ToSystemDrawingColor(this Color color)
            => System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);

        public static SolidColorBrush ToBrush(this System.Drawing.Color color)
            => new SolidColorBrush(color.AsWinUiColor());

        public static SolidColorBrush ToBrush(this Color color)
            => new SolidColorBrush(color);
        #endregion


        #region Modifiers
        /// <summary>
        /// Interpolates between two colors - keeping the Alpha of the first (unless it's transparent ... then its white with alpha 0);
        /// </summary>
        /// <returns>The blend.</returns>
        /// <param name="c">C.</param>
        /// <param name="c2">C2.</param>
        /// <param name="percent">Percent.</param>
        public static Color RgbHybridBlend(this Color c, Color c2, double percent)
        {
            var c1 = new Windows.UI.Color { R = c.R, G = c.G, B = c.B, A = c.A };
            if (c1 == Colors.Transparent)
                c1 = new Color { R = 255, G = 255, B = 255, A = 0 };
            var A = c1.A;
            var R = (byte)(c1.R + (c2.R - c1.R) * percent).Clamp(0, 255);
            var G = (byte)(c1.G + (c2.G - c1.G) * percent).Clamp(0, 255);
            var B = (byte)(c1.B + (c2.B - c1.B) * percent).Clamp(0, 255);
            return new Color { R = R, G = G, B = B, A = A };
        }

        /// <summary>
        /// Interpolates between two colors
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Color RgbaBlend(this Color c1, Color c2, double percent)
        {
            var A = (byte)(c1.A + (c2.A - c1.A) * percent).Clamp(0, 255);
            var R = (byte)(c1.R + (c2.R - c1.R) * percent).Clamp(0, 255);
            var G = (byte)(c1.G + (c2.G - c1.G) * percent).Clamp(0, 255);
            var B = (byte)(c1.B + (c2.B - c1.B) * percent).Clamp(0, 255);
            return new Color { R = R, G = G, B = B, A = A };
        }

        /// <summary>
        /// Withs the alpha.
        /// </summary>
        /// <returns>The alpha.</returns>
        /// <param name="c">C.</param>
        /// <param name="alpha">Alpha.</param>
        public static Color WithAlpha(this Color c, double alpha)
            => new Color { R = c.R, G = c.G, B = c.B, A = (byte)(alpha).Clamp(0, 255) };
        #endregion


        #region Factory Methods

        #region RGB
        public static Color ColorFromRgb(byte r, byte g, byte b)
            => ColorFromRgba(r, g, b);

        public static Color ColorFromRgb(int r, int g, int b)
            => ColorFromRgba(r, g, b);

        public static Color ColorFromRgb(double r, double g, double b)
            => ColorFromRgba(r, g, b);
        #endregion

        #region RGBA
        public static Color ColorFromRgba(byte r, byte g, byte b, byte a = 0xFF)
            => new Color { R = r, G = g, B = b, A = a };

        public static Color ColorFromRgba(int r, int g, int b, int a = 255)
            => new Color { R = (byte)r.Clamp(0, 255), G = (byte)g.Clamp(0, 255), B = (byte)b.Clamp(0, 255), A = (byte)a.Clamp(0, 255) };

        public static Color ColorFromRgba(double r, double g, double b, double a = 1.0)
            => ColorFromArgb(a, r, g, b);
        #endregion

        #region ARGB
        public static Color ColorFromArgb(byte a, byte r, byte g, byte b)
            => new Color { R = r, G = g, B = b, A = a };

        public static Color ColorFromArgb(int a, int r, int g, int b)
            => new Color { R = (byte)r.Clamp(0, 255), G = (byte)g.Clamp(0, 255), B = (byte)b.Clamp(0, 255), A = (byte)a.Clamp(0, 255) };

        public static Color ColorFromArgb(double a, double r, double g, double b)
        {
            var A = (byte)(a * 255).Clamp(0, 255);
            var R = (byte)(r * 255).Clamp(0, 255);
            var G = (byte)(g * 255).Clamp(0, 255);
            var B = (byte)(b * 255).Clamp(0, 255);
            return new Color { R = R, G = G, B = B, A = A };
        }
        #endregion

        #region Hue Saturation 
        public static Color ColorFromHsva(double h, double s, double v, double a)
        {
            h = h.Clamp(0, 1);
            s = s.Clamp(0, 1);
            v = v.Clamp(0, 1);
            var range = (int)(Math.Floor(h * 6)) % 6;
            var f = h * 6 - Math.Floor(h * 6);
            var p = v * (1 - s);
            var q = v * (1 - f * s);
            var t = v * (1 - (1 - f) * s);

            switch (range)
            {
                case 0:
                    return ColorFromRgba(v, t, p, a);
                case 1:
                    return ColorFromRgba(q, v, p, a);
                case 2:
                    return ColorFromRgba(p, v, t, a);
                case 3:
                    return ColorFromRgba(p, q, v, a);
                case 4:
                    return ColorFromRgba(t, p, v, a);
            }
            return ColorFromRgba(v, p, q, a);
        }

        public static Color ColorFromHsla(double hue, double saturation, double luminosity, double a = 1.0)
        {

            hue = Math.Min(Math.Max(0, hue), 1.0);
            saturation = Math.Min(Math.Max(0, saturation), 1.0);
            luminosity = Math.Min(Math.Max(0, luminosity), 1.0);

            if (luminosity == 0 || saturation == 0)
                return default;

            var temp2 = luminosity <= 0.5f ? luminosity * (1.0f + saturation) : luminosity + saturation - luminosity * saturation;
            var temp1 = 2.0 * luminosity - temp2;

            var t3 = new[] { hue + 1.0f / 3.0f, hue, hue - 1.0f / 3.0f };
            var clr = new double[] { 0, 0, 0 };
            for (var i = 0; i < 3; i++)
            {
                if (t3[i] < 0)
                    t3[i] += 1.0f;
                if (t3[i] > 1)
                    t3[i] -= 1.0f;
                if (6.0 * t3[i] < 1.0)
                    clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0f;
                else if (2.0 * t3[i] < 1.0)
                    clr[i] = temp2;
                else if (3.0 * t3[i] < 2.0)
                    clr[i] = temp1 + (temp2 - temp1) * (2.0f / 3.0f - t3[i]) * 6.0f;
                else
                    clr[i] = temp1;
            }
            return ColorFromArgb(a, clr[0], clr[1], clr[2]);
        }
        #endregion

        #region UINT
        public static Color ColorFromUint(uint argb)
            => new Color { R = (byte)((argb & 0x00ff0000) >> 0x10), G = (byte)((argb & 0x0000ff00) >> 0x8), B = (byte)(argb & 0x000000ff), A = (byte)((argb & 0xff000000) >> 0x18) };
        #endregion

        #region string
        public static Color ColorFromHex(string hex)
        {
            // Undefined
            if (hex.Length < 3)
                return default(Color);
            int idx = (hex[0] == '#') ? 1 : 0;

            switch (hex.Length - idx)
            {
                case 3: //#rgb => ffrrggbb
                    var t1 = ToHexD(hex[idx++]);
                    var t2 = ToHexD(hex[idx++]);
                    var t3 = ToHexD(hex[idx]);

                    return ColorFromRgb((int)t1, (int)t2, (int)t3);

                case 4: //#argb => aarrggbb
                    var f1 = ToHexD(hex[idx++]);
                    var f2 = ToHexD(hex[idx++]);
                    var f3 = ToHexD(hex[idx++]);
                    var f4 = ToHexD(hex[idx]);
                    return ColorFromRgba((int)f2, (int)f3, (int)f4, (int)f1);

                case 6: //#rrggbb => ffrrggbb
                    return ColorFromRgb((int)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                            (int)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                            (int)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx])));

                case 8: //#aarrggbb
                    var a1 = ToHex(hex[idx++]) << 4 | ToHex(hex[idx++]);
                    return ColorFromRgba((int)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                            (int)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx++])),
                            (int)(ToHex(hex[idx++]) << 4 | ToHex(hex[idx])),
                            (int)a1);

                default: //everything else will result in unexpected results
                    return default(Color);
            }
        }

        /// <summary>
        /// Takes a color string, typical of an HTML tag's color attribute, and coverts it to a Windows.UI color.
        /// </summary>
        /// <returns>The color.</returns>
        /// <param name="s">the color string</param>
        public static Color ColorFromString(this string s)
        {
            if (s.ToLower().StartsWith("rgb(", StringComparison.OrdinalIgnoreCase))
            {
                //var values = s.Substring(4, s.Length - 5).Split(',').Select(int.Parse).ToArray();
                var components = s.Substring(4, s.Length - 5).Split(',');
                if (components.Length != 3)
                    throw new FormatException("Could not parse [" + s + "] into RGB integer components");
                var r = byte.Parse(components[0]);
                var g = byte.Parse(components[1]);
                var b = byte.Parse(components[2]);
                return Color.FromArgb(1, r, g, b);
            }
            if (s.ToLower().StartsWith("rgba(", StringComparison.OrdinalIgnoreCase))
            {
                //var values = s.Substring(5, s.Length - 6).Split(',').Select(int.Parse).ToArray();
                var components = s.Substring(5, s.Length - 6).Split(',');
                if (components.Length != 4)
                    throw new FormatException("Could not parse [" + s + "] into RGBA integer components");
                var r = byte.Parse(components[0]);
                var g = byte.Parse(components[1]);
                var b = byte.Parse(components[2]);
                var a = byte.Parse(components[3]);
                return Color.FromArgb(a, r, g, b);
            }
            if (s.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {

                var color = ColorFromHex(s);
                return color;
            }
            var colorName = s.ToLower();
            switch (colorName)
            {
                case "aliceblue": return ColorFromHex("F0F8FF");
                case "antiquewhite": return ColorFromHex("FAEBD7");
                case "aqua": return ColorFromHex("00FFFF");
                case "aquamarine": return ColorFromHex("7FFFD4");
                case "azure": return ColorFromHex("F0FFFF");
                case "beige": return ColorFromHex("F5F5DC");
                case "bisque": return ColorFromHex("FFE4C4");
                case "black": return ColorFromHex("000000");
                case "blanchedalmond": return ColorFromHex("FFEBCD");
                case "blue": return ColorFromHex("0000FF");
                case "blueviolet": return ColorFromHex("8A2BE2");
                case "brown": return ColorFromHex("A52A2A");
                case "burlywood": return ColorFromHex("DEB887");
                case "cadetblue": return ColorFromHex("5F9EA0");
                case "chartreuse": return ColorFromHex("7FFF00");
                case "chocolate": return ColorFromHex("D2691E");
                case "coral": return ColorFromHex("FF7F50");
                case "cornflowerblue": return ColorFromHex("6495ED");
                case "cornsilk": return ColorFromHex("FFF8DC");
                case "crimson": return ColorFromHex("DC143C");
                case "cyan": return ColorFromHex("00FFFF");
                case "darkblue": return ColorFromHex("00008B");
                case "darkcyan": return ColorFromHex("008B8B");
                case "darkgoldenrod": return ColorFromHex("B8860B");
                case "darkgray": return ColorFromHex("A9A9A9");
                case "darkgrey": return ColorFromHex("A9A9A9");
                case "darkgreen": return ColorFromHex("006400");
                case "darkkhaki": return ColorFromHex("BDB76B");
                case "darkmagenta": return ColorFromHex("8B008B");
                case "darkolivegreen": return ColorFromHex("556B2F");
                case "darkorange": return ColorFromHex("FF8C00");
                case "darkorchid": return ColorFromHex("9932CC");
                case "darkred": return ColorFromHex("8B0000");
                case "darksalmon": return ColorFromHex("E9967A");
                case "darkseagreen": return ColorFromHex("8FBC8F");
                case "darkslateblue": return ColorFromHex("483D8B");
                case "darkslategray": return ColorFromHex("2F4F4F");
                case "darkslategrey": return ColorFromHex("2F4F4F");
                case "darkturquoise": return ColorFromHex("00CED1");
                case "darkviolet": return ColorFromHex("9400D3");
                case "deeppink": return ColorFromHex("FF1493");
                case "deepskyblue": return ColorFromHex("00BFFF");
                case "dimgray": return ColorFromHex("696969");
                case "dimgrey": return ColorFromHex("696969");
                case "dodgerblue": return ColorFromHex("1E90FF");
                case "firebrick": return ColorFromHex("B22222");
                case "floralwhite": return ColorFromHex("FFFAF0");
                case "forestgreen": return ColorFromHex("228B22");
                case "fuchsia": return ColorFromHex("FF00FF");
                case "gainsboro": return ColorFromHex("DCDCDC");
                case "ghostwhite": return ColorFromHex("F8F8FF");
                case "gold": return ColorFromHex("FFD700");
                case "goldenrod": return ColorFromHex("DAA520");
                case "gray": return ColorFromHex("808080");
                case "grey": return ColorFromHex("808080");
                case "green": return ColorFromHex("008000");
                case "greenyellow": return ColorFromHex("ADFF2F");
                case "honeydew": return ColorFromHex("F0FFF0");
                case "hotpink": return ColorFromHex("FF69B4");
                case "indianred ": return ColorFromHex("CD5C5C");
                case "indigo ": return ColorFromHex("4B0082");
                case "ivory": return ColorFromHex("FFFFF0");
                case "khaki": return ColorFromHex("F0E68C");
                case "lavender": return ColorFromHex("E6E6FA");
                case "lavenderblush": return ColorFromHex("FFF0F5");
                case "lawngreen": return ColorFromHex("7CFC00");
                case "lemonchiffon": return ColorFromHex("FFFACD");
                case "lightblue": return ColorFromHex("ADD8E6");
                case "lightcoral": return ColorFromHex("F08080");
                case "lightcyan": return ColorFromHex("E0FFFF");
                case "lightgoldenrodyellow": return ColorFromHex("FAFAD2");
                case "lightgray": return ColorFromHex("D3D3D3");
                case "lightgrey": return ColorFromHex("D3D3D3");
                case "lightgreen": return ColorFromHex("90EE90");
                case "lightpink": return ColorFromHex("FFB6C1");
                case "lightsalmon": return ColorFromHex("FFA07A");
                case "lightseagreen": return ColorFromHex("20B2AA");
                case "lightskyblue": return ColorFromHex("87CEFA");
                case "lightslategray": return ColorFromHex("778899");
                case "lightslategrey": return ColorFromHex("778899");
                case "lightsteelblue": return ColorFromHex("B0C4DE");
                case "lightyellow": return ColorFromHex("FFFFE0");
                case "lime": return ColorFromHex("00FF00");
                case "limegreen": return ColorFromHex("32CD32");
                case "linen": return ColorFromHex("FAF0E6");
                case "magenta": return ColorFromHex("FF00FF");
                case "maroon": return ColorFromHex("800000");
                case "mediumaquamarine": return ColorFromHex("66CDAA");
                case "mediumblue": return ColorFromHex("0000CD");
                case "mediumorchid": return ColorFromHex("BA55D3");
                case "mediumpurple": return ColorFromHex("9370DB");
                case "mediumseagreen": return ColorFromHex("3CB371");
                case "mediumslateblue": return ColorFromHex("7B68EE");
                case "mediumspringgreen": return ColorFromHex("00FA9A");
                case "mediumturquoise": return ColorFromHex("48D1CC");
                case "mediumvioletred": return ColorFromHex("C71585");
                case "midnightblue": return ColorFromHex("191970");
                case "mintcream": return ColorFromHex("F5FFFA");
                case "mistyrose": return ColorFromHex("FFE4E1");
                case "moccasin": return ColorFromHex("FFE4B5");
                case "navajowhite": return ColorFromHex("FFDEAD");
                case "navy": return ColorFromHex("000080");
                case "oldlace": return ColorFromHex("FDF5E6");
                case "olive": return ColorFromHex("808000");
                case "olivedrab": return ColorFromHex("6B8E23");
                case "orange": return ColorFromHex("FFA500");
                case "orangered": return ColorFromHex("FF4500");
                case "orchid": return ColorFromHex("DA70D6");
                case "palegoldenrod": return ColorFromHex("EEE8AA");
                case "palegreen": return ColorFromHex("98FB98");
                case "paleturquoise": return ColorFromHex("AFEEEE");
                case "palevioletred": return ColorFromHex("DB7093");
                case "papayawhip": return ColorFromHex("FFEFD5");
                case "peachpuff": return ColorFromHex("FFDAB9");
                case "peru": return ColorFromHex("CD853F");
                case "pink": return ColorFromHex("FFC0CB");
                case "plum": return ColorFromHex("DDA0DD");
                case "powderblue": return ColorFromHex("B0E0E6");
                case "purple": return ColorFromHex("800080");
                case "rebeccapurple": return ColorFromHex("663399");
                case "red": return ColorFromHex("FF0000");
                case "rosybrown": return ColorFromHex("BC8F8F");
                case "royalblue": return ColorFromHex("4169E1");
                case "saddlebrown": return ColorFromHex("8B4513");
                case "salmon": return ColorFromHex("FA8072");
                case "sandybrown": return ColorFromHex("F4A460");
                case "seagreen": return ColorFromHex("2E8B57");
                case "seashell": return ColorFromHex("FFF5EE");
                case "sienna": return ColorFromHex("A0522D");
                case "silver": return ColorFromHex("C0C0C0");
                case "skyblue": return ColorFromHex("87CEEB");
                case "slateblue": return ColorFromHex("6A5ACD");
                case "slategray": return ColorFromHex("708090");
                case "slategrey": return ColorFromHex("708090");
                case "snow": return ColorFromHex("FFFAFA");
                case "springgreen": return ColorFromHex("00FF7F");
                case "steelblue": return ColorFromHex("4682B4");
                case "tan": return ColorFromHex("D2B48C");
                case "teal": return ColorFromHex("008080");
                case "thistle": return ColorFromHex("D8BFD8");
                case "tomato": return ColorFromHex("FF6347");
                case "turquoise": return ColorFromHex("40E0D0");
                case "violet": return ColorFromHex("EE82EE");
                case "wheat": return ColorFromHex("F5DEB3");
                case "white": return ColorFromHex("FFFFFF");
                case "whitesmoke": return ColorFromHex("F5F5F5");
                case "yellow": return ColorFromHex("FFFF00");
                case "yellowgreen": return ColorFromHex("9ACD32");
            }
            return default(Color);
        }
        #endregion

        #endregion


        #region HSL 
        public static (float Hue, float Saturation, float Luminosity) ToHsl(this Color color)
        {
            ToHsl(color, out float h, out float s, out float l);
            return (h, s, l);
        }

        public static void ToHsl(this Color color, out float hue, out float saturation, out float luminosity)
        {
            var r = color.R / 255;
            var g = color.G / 255;
            var b = color.B / 255;

            float v = Math.Max(r, g);
            v = Math.Max(v, b);

            float m = Math.Min(r, g);
            m = Math.Min(m, b);

            luminosity = (m + v) / 2.0f;
            if (luminosity <= 0.0)
            {
                hue = saturation = luminosity = 0;
                return;
            }
            float vm = v - m;
            saturation = vm;

            if (saturation > 0.0)
            {
                saturation /= luminosity <= 0.5f ? v + m : 2.0f - v - m;
            }
            else
            {
                hue = 0;
                saturation = 0;
                return;
            }

            float r2 = (v - r) / vm;
            float g2 = (v - g) / vm;
            float b2 = (v - b) / vm;

            if (r == v)
            {
                hue = g == m ? 5.0f + b2 : 1.0f - g2;
            }
            else if (g == v)
            {
                hue = b == m ? 1.0f + r2 : 3.0f - b2;
            }
            else
            {
                hue = r == m ? 3.0f + g2 : 5.0f - r2;
            }
            hue /= 6.0f;
        }
        #endregion


        #region Tests
        /// <summary>
        /// Tests if the color is one of the default values
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDefault(this Color c)
            => c == default || (c.R == 0 && c.G == 0 && c.B == 0 && c.A == 0);


        /// <summary>
        /// Tests if the color is a default or is transparent
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool IsDefaultOrTransparent(this Color c)
            => IsDefault(c) || c.A == 0;
        #endregion


        #region ToString
        /// <summary>
        /// Returns a string with comma separated, 0-255, integer values for color's RGB
        /// </summary>
        /// <returns>The int rgb color string.</returns>
        /// <param name="color">Color.</param>
        public static string ToIntRgbColorString(this Color color)
            => color.R + "," + color.G + "," + color.B;

        /// <summary>
        /// Returns a sring with comma separated, 0-255, integer values for color's RGBA
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToIntRgbaColorString(this Color color)
            => color.R + "," + color.G + "," + color.B + "," + color.A;

        /// <summary>
        /// Returns a string with comma separated, 0-255, integer values for color's RGBA
        /// </summary>
        /// <returns>The int rgb color string.</returns>
        /// <param name="color">Color.</param>
        public static string ToRgbaColorString(this Color color)
        {
            return color.ToIntRgbColorString() + "," + color.A;
        }

        /// <summary>
        /// Returns a 3 character hexadecimal string of a color's RGB value
        /// </summary>
        /// <returns>The hex rgb color string.</returns>
        /// <param name="color">Color.</param>
        public static string ToHexRgbColorString(this Color color)
        {
            var r = color.R >> 4;
            var g = color.G >> 4;
            var b = color.B >> 4;
            var R = r.ToString("x1");
            var G = g.ToString("x1");
            var B = b.ToString("x1");
            return R + G + B;
        }

        /// <summary>
        /// Returns a 4 character hexadecimal string of a color's ARGB value
        /// </summary>
        /// <returns>The hex rgb color string.</returns>
        /// <param name="color">Color.</param>
        public static string ToHexArgbColorString(this Color color)
        {
            var a = color.A >> 4;
            return a.ToString("x1") + color.ToHexRgbColorString();
        }

        /// <summary>
        /// Returns a 6 character hexadecimal string of a color's RRGGBB value
        /// </summary>
        /// <returns>The hex rgb color string.</returns>
        /// <param name="color">Color.</param>
        public static string ToHexRrggbbColorString(this Color color)
        {
            return color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2");
        }

        /// <summary>
        /// Returns a 8 character hexadecimal string of a color's AARRGGBB value
        /// </summary>
        /// <returns>The hex rgb color string.</returns>
        /// <param name="color">Color.</param>
        public static string ToHextAarrggbbColorString(this Color color)
        {
            return color.A.ToString("x2") + color.ToHexRrggbbColorString();
        }
        #endregion


        #region Internal
        static uint ToHex(char c)
        {
            ushort x = (ushort)c;
            if (x >= '0' && x <= '9')
                return (uint)(x - '0');

            x |= 0x20;
            if (x >= 'a' && x <= 'f')
                return (uint)(x - 'a' + 10);
            return 0;
        }
        static uint ToHexD(char c)
        {
            var j = ToHex(c);
            return (j << 4) | j;
        }
        #endregion


        public static Color AppColor(string key)
        {
            if (Application.Current.Resources[key] is Color color)
                return color;
            throw new Exception("color not found in Application.Current.Resources for key [" + key + "]. ");
        }

    }
}
