using System;

namespace LewisTech.Utils.Formatting
{
    public static class StringExtensions
    {

        public static string Truncate(this string input, int maxLength)
        {
            if (string.IsNullOrEmpty(input)) return input;

            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength);
            }
            return input;

        }

        public static bool Contains(this string str, string value, StringComparison comparison)
        {
            return str.IndexOf(value, comparison) > -1;
        }

    }
}
