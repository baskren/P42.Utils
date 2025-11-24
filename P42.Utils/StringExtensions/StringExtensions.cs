using System.Text;

namespace P42.Utils;

public static class StringExtensions
{
    /// <summary>
    /// Replacement characters to for smooth serialization
    /// </summary>
    public static readonly Dictionary<char, char> IllegalToSafeCharacters = new()
    {
        //{'.', 'ᆞ'},
        //{':', '：' },
        //{'|', '｜'},
        //{' ', '　'},
        //{'#', '＃'},
        //{'%','％'},
        //{'&','＆'},
        {'{', '｛'},
        {'}','｝'},
        //{'\\', '＼'},
        //{'<', '＜'},
        //{'>', '＞'},
        //{'*', '＊'},
        //{'?', '？'},
        //{'/', '／' },
        //{'$', '＄'},
        //{'!', '！'},
        //{'\'', '＇'},
        //{'`', '｀'},
        //{'"', '＂'},
        //{';', '；'},
        //{'@', '＠'},
        //{'=', '＝'},
        //{'~', '～'},
        //{'+', '＋'},
//            {'', ''},
        {'[', '［'},
        {']', '］'}
    };
    
    public static readonly Dictionary<char, char> SafeToIllegalCharacters = IllegalToSafeCharacters.ToDictionary(c => c.Value, c => c.Key);

    private static System.Security.Cryptography.MD5? _md5;
    private static System.Security.Cryptography.MD5 Md5 => _md5 ??= System.Security.Cryptography.MD5.Create();


    /// <param name="text"></param>
    extension(string text)
    {
        /// <summary>
        /// Convert unicode characters to HTML escape codes
        /// </summary>
        /// <returns></returns>
        public string UnicodeToHtmlEscapes()
        {
            var chars = text.ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (var c in chars)
            {
                var value = Convert.ToInt32(c);
                if (value > 127)
                    result.Append($"&#{value};");
                else
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Remove whitespace from string
        /// </summary>
        /// <returns></returns>
        public string RemoveWhitespace()
        {
            return new string(text.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        internal string ToMd5HashString()
        {
            var hash = Md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            var sBuilder = new StringBuilder();

            foreach (var t in hash)
                sBuilder.Append(t.ToString("x2"));

            return sBuilder.ToString();
        }

        /// <summary>
        /// Safe substring of everything but last `count` characters
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string RemoveLast(int count = 1)
            => count <= 0 
                ? text
                : text.Length > count 
                    ? text[..^count] 
                    : string.Empty;

        /// <summary>
        /// Safe substring of last `count` characters
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string SubstringLast(int count = 1)
            => count <= 0
                ? string.Empty
                : text.Length > count 
                    ? text[^count..] 
                    : text;

        public string SubstringRange(int startIndex, int length = 1)
        {
            if (text.Length <= startIndex)
                return string.Empty;

            return text.Length > startIndex + length 
                ? text.Substring(startIndex, length) 
                : text[startIndex..];
        }

        /// <summary>
        /// Does the string contain any illegal characters?
        /// </summary>
        /// <returns></returns>
        public bool HasIllegalCharacter()
            => text.Any(c => IllegalToSafeCharacters.ContainsKey(c));

        /// <summary>
        /// Replace any illegal characters
        /// </summary>
        /// <returns></returns>
        public string ReplaceIllegalCharacters()
        {
            //if (Equals(s, "$type"))
            //    return " ＄type";  // used to get $type to be the first key

            var c = text.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                //if (result[i] == '．')
                //    result[i] = 'ᆞ';

                // Tested the following against TryGetValue(key, out var value)
                // Was faster on every platform but WASM
                if (IllegalToSafeCharacters.ContainsKey(c[i]) )
                    c[i] = IllegalToSafeCharacters[c[i]];
            }
            return new string(c);
        }

        /// <summary>
        /// Reverse of ReplaceIllegalCharacters
        /// </summary>
        /// <returns></returns>
        public string ReplaceSafeCharacters()
        {
            if (Equals(text, " ＄type"))
                return "$type";

            var result = text.ToCharArray();
            for (var i = 0; i < result.Length; i++)
            {
                //if (result[i] == '．')
                //    result[i] = 'ᆞ';
            
                // Tested to be 1/10 the time as IllegalCharacters.TryGetKey
                foreach (var kvp in SafeToIllegalCharacters.Where(kvp => result[i] == kvp.Key))
                    result[i] = kvp.Value;
            }
            return new string(result);
        }
    }


    /// <summary>
    /// Convert char to hexadecimal (uint) value
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static uint ToHex(this char c)
    {
        ushort num = c;
        if (num is >= 48 and <= 57)

            return (uint)(num - 48);
        
        num = (ushort)(num | 0x20);
        if (num is >= 97 and <= 102)
            return (uint)(num - 97 + 10);

        throw new ArgumentOutOfRangeException($"Char [{c}] cannot be converted to hex");
    }

    [Obsolete("OBSOLETE: Use byte.Parse(), instead.", true)]
    public static uint ToHex(string str)
        => str.Aggregate<char, uint>(0, (current, c) => (current << 4) + ToHex(c));
    

    private static readonly string[] Suffixes = [" B", " KB", " MB", " GB", " TB", " PB"];

    /// <summary>
    /// Convert number of bytes to human-readable value
    /// </summary>
    /// <param name="number"></param>
    /// <param name="precision"></param>
    /// <param name="si"></param>
    /// <param name="thouSeparators"></param>
    /// <returns></returns>
    // ReSharper disable once UnusedParameter.Global
    public static string HumanReadableBytes(this double number, int precision = 2, bool si = false, bool thouSeparators = true)
    {
        // unit's number of bytes
        var unit = si ? 1000 : 1024;

        // suffix counter
        var i = 0;
        // as long as we're bigger than a unit, keep going
        while (number >= unit)
        {
            number /= unit;
            i++;
        }

        var format = "#." + new string('#', precision);
        // apply precision and current suffix
        return  Math.Round(number, precision).ToString(format) + Suffixes[i] + (si ? "" : "i");
    }

    /// <summary>
    /// Convert number of bytes to human-readable value
    /// </summary>
    /// <param name="num"></param>
    /// <param name="precision"></param>
    /// <param name="si"></param>
    /// <param name="thouSeparators"></param>
    /// <returns></returns>
    public static string HumanReadableBytes(this ulong num, int precision = 2, bool si = false, bool thouSeparators = true)
        => HumanReadableBytes((double)num, precision, si, thouSeparators);

    /// <summary>
    /// Convert number of bytes to human-readable value
    /// </summary>
    /// <param name="num"></param>
    /// <param name="precision"></param>
    /// <param name="si"></param>
    /// <param name="thouSeparators"></param>
    /// <returns></returns>
    public static string HumanReadableBytes(this long num, int precision = 2, bool si = false, bool thouSeparators = true)
        => HumanReadableBytes((double)num, precision, si, thouSeparators);

    /// <summary>
    /// Gets the first non-null or whitespace entry from an IEnumerable
    /// </summary>
    /// <param name="values"></param>
    /// <param name="onFail">value returned if none found</param>
    /// <returns>string.Empty if none found</returns>
    public static string FirstNotNullOrWhiteSpace(this IEnumerable<string?> values, string onFail = "")
        => values.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)) ?? onFail;
    
    /// <summary>
    /// Removes all white redundant white space, leaving only one space between words
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string RemoveExtraWhitespace(this string str)
    {
        var sb = new StringBuilder();
        var lastCharIsWhitespace = false;
        foreach (var c in str)
        {
            if (char.IsWhiteSpace(c))
            {
                lastCharIsWhitespace = true;
                continue;
            }

            if (lastCharIsWhitespace)
                sb.Append(' ');
            sb.Append(c);
            lastCharIsWhitespace = false;
        }
    
        return sb.ToString();
    }
}
