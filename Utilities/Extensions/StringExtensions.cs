using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveWhitespaces(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return Regex.Replace(value, @"\s+", "");
        }

        public static string RemoveLetters(this string value)
        {
            return Regex.Replace(value, @"\w+", "");
        }

        public static string RemoveNonDigit(this string value)
        {
            return Regex.Replace(value, @"[^\d]", "");
        }

        public static decimal? FindDecimalOrNull(this string value, string separator = ",")
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var decimalRegex = new Regex(@$"[0-9{separator}]+");

            if (decimalRegex.IsMatch(value))
                return decimal.Parse(decimalRegex.Match(value).Value,
                    new NumberFormatInfo { NumberDecimalSeparator = separator });

            return null;
        }


        public static decimal FindDecimal(this string value, string separator = ",",
            decimal defaultValue = Decimal.Zero)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            var decimalRegex = new Regex(@$"[0-9{separator}]+");

            if (decimalRegex.IsMatch(value))
                return decimal.Parse(decimalRegex.Match(value).Value,
                    new NumberFormatInfo {NumberDecimalSeparator = separator});

            return defaultValue;
        }

        public static int? FindIntegerNullable(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var decimalRegex = new Regex(@"[0-9]+");

            if (decimalRegex.IsMatch(value)) return int.Parse(decimalRegex.Match(value).Value);

            return null;
        }

        public static int FindInteger(this string value, int defaultValue = 0)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            var decimalRegex = new Regex(@"[0-9]+");

            if (decimalRegex.IsMatch(value)) return int.Parse(decimalRegex.Match(value).Value);

            return defaultValue;
        }

        public static DateTime FindDate(this string value, string separator = "-", DateTime defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            var dateRegex = new Regex($@"((0[1-9]|[12]\d|3[01]){separator}(0[1-9]|1[0-2]){separator}([12]\d{{3}}))");
            string[] format = {$"dd{separator}MM{separator}yyyy"};

            if (dateRegex.IsMatch(value)) return DateTime.ParseExact(dateRegex.Match(value).Value, format, new CultureInfo("en-US"), DateTimeStyles.None);

            return defaultValue;
        }

        public static bool StringToBool(this string value,
            string trueValue,
            string falseValue,
            bool defaultValue = false,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;

            if (value.Contains(trueValue, stringComparison)) return true;

            if (value.Contains(falseValue, stringComparison)) return false;

            return defaultValue;
        }

        public static bool? StringToNullableBool(this string value,
            string trueValue,
            string falseValue,
            StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (value.Contains(trueValue, stringComparison)) return true;

            if (value.Contains(falseValue, stringComparison)) return false;

            return null;
        }
    }
}