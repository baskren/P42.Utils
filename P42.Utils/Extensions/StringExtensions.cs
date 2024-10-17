using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace P42.Utils;

public static class StringExtensions
{

    public static string UnicodeToHtmlEscapes(this string text)
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


    public static string RemoveWhitespace(this string input)
    {
        return new string(input.ToCharArray()
            .Where(c => !char.IsWhiteSpace(c))
            .ToArray());
    }

    private static System.Security.Cryptography.MD5? _md5;
    private static System.Security.Cryptography.MD5 Md5 => _md5 ??= System.Security.Cryptography.MD5.Create();
    internal static string ToMd5HashString(this string source)
    {
        var hash = Md5.ComputeHash(Encoding.UTF8.GetBytes(source));
        var sBuilder = new StringBuilder();

        foreach (var t in hash)
            sBuilder.Append(t.ToString("x2"));

        return sBuilder.ToString();
    }

    public static bool IsNumeric(this string s)
        => s.All(c => char.IsDigit(c) || c == '.' || c == '-');
        

    public static string RemoveLast(this string s, int count = 1)
    {
        return s.Length > 0 
            ? s.Remove(s.Length - count) 
            : s;
    }

    [Obsolete("Use [^count..] instead.")]
    public static string SubstringLast(this string s, int count = 1)
    {
        if (s.Length <= count || count < 0)
            return s;
            
        return s[^count..];
    }

    // Illegal characters are the keys, the safe characters are the values
    public static readonly Dictionary<char, char> IllegalSafeCharacters = new()
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
        //{'', ''},
        {'[', '［'},
        {']', '］'}
    };



    public static bool HasIllegalCharacter(this string s)
        => s.Any(c => IllegalSafeCharacters.ContainsKey(c));
        

    public static string ReplaceIllegalCharacters(this string s)
    {
        var result = s.ToCharArray();
        for (var i = 0; i < result.Length; i++)
        {
            if (IllegalSafeCharacters.TryGetValue(result[i], out var safeChar))
                result[i] = safeChar;
        }
            
        return new string(result);
    }

    public static string ReplaceSafeCharacters(this string s)
    {
        if (Equals(s, " ＄type"))
            return "$type";

        var result = s.ToCharArray();
        for (var i = 0; i < result.Length; i++)
        {
            if (IllegalSafeCharacters.TryGetKey(result[i], out var illegalChar))
                result[i] = illegalChar;
        }
        return new string(result);
    }

    public static uint ToHex(char c)
    {
        ushort num = c;
        if (num is >= 48 and <= 57)
        {
            return (uint)(num - 48);
        }
        num = (ushort)(num | 0x20);
        if (num is >= 97 and <= 102)
        {
            return (uint)(num - 97 + 10);
        }
        return 0u;
    }

    public static uint ToHex(string str)
        => str.Aggregate<char, uint>(0, (current, c) => (current << 4) + ToHex(c));
    

    private static readonly string[] Suffixes = [" B", " KB", " MB", " GB", " TB", " PB"];

    public static string HumanReadableBytes(double number, int precision = 2, bool si = false)
    {
        // unit's number of bytes
        var unit = si ? 1000 : 1024;

        // suffix counter
        var i = 0;
        // as long as we're bigger than a unit, keep going
        while (number > unit)
        {
            number /= unit;
            i++;
        }
        // apply precision and current suffix
        return Math.Round(number, precision) + Suffixes[i] + (si ? "" : "i");
    }

}
