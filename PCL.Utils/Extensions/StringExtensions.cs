using System;
using System.Collections.Generic;

namespace PCL.Utils
{
    public static class StringExtensions
    {
        public static bool IsNumeric(this string s)
        {
            foreach (var c in s)
                if (!char.IsDigit(c) && c != '.' && c != '-')
                    return false;
            return true;
        }

        public static string RemoveLast(this string s, int count = 1)
        {
            if (s.Length > 0)
                return s.Remove(s.Length - count);
            return s;
        }

        public static string SubstringLast(this string s, int count = 1)
        {
            if (s.Length <= count)
                return s;
            var result = s.Substring(s.Length - 40);
            System.Diagnostics.Debug.WriteLine("WARNING!!! HAD TO IMPLEMENT SubstringLast on [" + s + "]");
            return result;
        }

        public static Dictionary<char, char> IllegalSafeCharacters = new Dictionary<char, char>
        {
            {'.', '．'},
            {':', '：' },
            {'|', '｜'},
            {' ', '　'},
            {'#', '＃'},
            {'%','％'},
            {'&','＆'},
            {'{', '｛'},
            {'}','｝'},
            {'\\', '＼'},
            {'<', '＜'},
            {'>', '＞'},
            {'*', '＊'},
            {'?', '？'},
            {'/', '／' },
            {'$', '＄'},
            {'!', '！'},
            {'\'', '＇'},
            {'`', '｀'},
            {'"', '＂'},
            {';', '；'},
            {'@', '＠'},
            {'=', '＝'},
            {'~', '～'},
            {'+', '＋'},
            {'[', '［'},
            {']', '］'},
//            {'', ''},
        };



        public static bool HasIllegalCharacter(this string s)
        {
            foreach (var c in s)
                if (IllegalSafeCharacters.ContainsKey(c))
                    return true;
            return false;
        }

        public static string ReplaceIllegalCharacters(this string s)
        {
            if (Equals(s, "$type"))
                return " ＄type";  // used to get $type to be the first key

            var result = s.ToCharArray();

            for (int i = 0; i < result.Length; i++)
            {
                //if (IllegalCharacters.ContainsKey(result[i]))
                //    result[i] = IllegalCharacters[result[i]];
                if (IllegalSafeCharacters.TryGetValue(result[i], out char value))
                    result[i] = value;
            }
            return new string(result);
        }

        public static string ReplaceSafeCharacters(this string s)
        {
            if (Equals(s, " ＄type"))
                return "$type";

            var result = s.ToCharArray();
            for (int i = 0; i < result.Length; i++)
            {
                if (IllegalSafeCharacters.TryGetKey(result[i], out char key))
                    result[i] = key;
            }
            return new string(result);
        }

    }
}
