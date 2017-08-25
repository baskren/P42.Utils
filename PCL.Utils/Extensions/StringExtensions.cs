using System;
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

    }
}
