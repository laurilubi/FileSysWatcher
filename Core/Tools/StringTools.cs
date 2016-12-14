using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Core.Exceptions;

namespace Core.Tools
{
    public static class StringTools
    {
        public static string GetValueOrDefault(object input, string defaultValue = "")
        {
            var value = input == null
                        ? defaultValue
                        : input.ToString();
            return value;
        }

        public static string GetValueOrThrow(object input, string errorMessageFormat = "Value cannot be null.")
        {
            if (input == null) throw new ModelValidationException(errorMessageFormat);
            return input.ToString();
        }

        public static string DefaultIfEmpty(this string input,string defaultValue = "")
        {
            return string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        }

        public static bool IsEmpty(this string input)
        {
            return string.IsNullOrEmpty(input) || input.Trim() == "";
        }

        public static string CommaSeparate<T>(this IEnumerable<T> objects)
        {
            return CommaSeparate(objects, o => o);
        }

        public static string CommaSeparate<T>(this IEnumerable<T> objects, Func<T, object> selector, string separator = ",")
        {
            var s = new StringBuilder();
            foreach (var o in objects.Select(selector))
            {
                s.Append(o);
                s.Append(separator);
            }
            return s.ToString().TrimEnd(separator);
        }

        public static List<int> SplitInts(string values)
        {
            if (string.IsNullOrEmpty(values)) return new List<int>();
            var ss = values.Split(new[] { ',' });
            return (from s in ss where !string.IsNullOrEmpty(s) select Convert.ToInt32(s)).ToList();
        }

        public static List<long> SplitLongs(this string values, params char[] separators)
        {
            return string.IsNullOrWhiteSpace(values) ? new List<long>() : values.Split(separators).Where(_ => !string.IsNullOrWhiteSpace(_)).Select(long.Parse).ToList();
        }

        public static List<int> SplitInts(this string values, params char[] separators)
        {
            return string.IsNullOrWhiteSpace(values) ? new List<int>() : values.Split(separators).Where(_ => !string.IsNullOrWhiteSpace(_)).Select(int.Parse).ToList();
        }

        public static List<string> SplitStrings(string values, bool trim = false, char separator = ',')
        {
            var ss = values.Split(new[] { separator });
            return (from s in ss where !string.IsNullOrEmpty(s) select trim ? s.Trim() : s).ToList();
        }

        public static List<string> SplitStrings(this string values, bool trim = false, params char[] separator)
        {
            return values.Split(separator).Where(_ => !string.IsNullOrWhiteSpace(_)).Select(_ => trim ? _.Trim() : _).ToList();
        }

        private static readonly Regex WhitespaceRgx = new Regex(@"\s", RegexOptions.Compiled);
        public static string RemoveWhitespace(this string str)
        {
            if (str == null) return null;
            var cleaned = WhitespaceRgx.Replace(str, "");
            return cleaned;
        }

        private static readonly Regex OnlyNumbersRgx = new Regex(@"^[0-9]*$", RegexOptions.Compiled);
        public static bool OnlyNumbers(this string str)
        {
            if (str == null) return false;
            return OnlyNumbersRgx.IsMatch(str);
        }

        private static readonly Regex DoubleWhitespaceRgx = new Regex(@"\s\s+", RegexOptions.Compiled);
        public static bool HasDoubleWhitespace(this string str)
        {
            if (str == null) return false;
            return DoubleWhitespaceRgx.IsMatch(str);
        }
        public static string RemoveDoubleWhitespace(this string str)
        {
            if (str == null) return null;
            var cleaned = DoubleWhitespaceRgx.Replace(str, " ");
            return cleaned;
        }

        private static readonly Regex NonNumbersRgx = new Regex(@"[^\d]", RegexOptions.Compiled);
        public static string RemoveNonNumbers(this string str)
        {
            if (str == null) return null;
            var cleaned = NonNumbersRgx.Replace(str, "");
            return cleaned;
        }

        public static string TrimEnd(this string input, string trimString)
        {
            if (input == null) return null;
            while (input.EndsWith(trimString))
                input = input.Substring(0, input.Length - trimString.Length);
            return input;
        }

        public static string Truncate(this string input, int maxLength, string dotdot = "")
        {
            if (input.Length <= maxLength)
                return input;
            return input.Substring(0, maxLength - dotdot.Length) + dotdot;
        }

        /// <summary>
        /// Makes both strings lowercase and removes whitespaces before comparing
        /// </summary>
        public static bool EqualsIgnoreCaseWhitespace(this string str1, string str2)
        {
            if (str1 == null && str2 == null) return true;
            if (str1 == null || str2 == null) return false;
            return str1.ToLower().RemoveWhitespace() == str2.ToLower().RemoveWhitespace();
        }

        private static readonly Regex MatchEmailPattern = new Regex(@"^[a-z0-9.+=_&%-/\\]+@[a-z0-9.-]+\.[a-z]{2,4}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static bool IsEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && MatchEmailPattern.IsMatch(email);
        }
        public static string IngenUppgiftIfEmpty(this string str )
        {
            return string.IsNullOrEmpty(str.Trim()) ? "Ingen uppgift" : str;
        }

        public static string GetRandomString(int length, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < length; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return lowerCase
                ? builder.ToString().ToLower()
                : builder.ToString();
        }

        public static int GetRandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }

        public static string InitCap(this string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            if (input.Length == 1)
                return input.ToUpper();
            return string.Format("{0}{1}", input.Substring(0, 1).ToUpper(), input.Substring(1).ToLower());
        }

        // http://stackoverflow.com/questions/1404435/string-split-out-of-memory-exception-when-reading-tab-separated-file/28601805#28601805
        // the string.Split() method from .NET tend to run out of memory on 80 Mb strings.
        // this has been reported several places online.
        // This version is fast and memory efficient and return no empty lines.
        public static List<string> LowMemSplit(this string s, string seperator)
        {
            var result = new List<string>();
            var lastPos = 0;
            var pos = s.IndexOf(seperator, StringComparison.Ordinal);
            while (pos > -1)
            {
                while (pos == lastPos)
                {
                    lastPos += seperator.Length;
                    pos = s.IndexOf(seperator, lastPos, StringComparison.Ordinal);
                    if (pos == -1)
                        return result;
                }

                var tmp = s.Substring(lastPos, pos - lastPos);
                if (tmp.Trim().Length > 0)
                    result.Add(tmp);
                lastPos = pos + seperator.Length;
                pos = s.IndexOf(seperator, lastPos, StringComparison.Ordinal);
            }

            if (lastPos < s.Length)
            {
                var tmp = s.Substring(lastPos, s.Length - lastPos);
                if (tmp.Trim().Length > 0)
                    result.Add(tmp);
            }

            return result;
        }
        public static string Replace(this string originalString, string oldValue, string newValue, StringComparison comparisonType)
        {
            if (originalString==null) return null;
            int startIndex = 0;
            while (true)
            {
                startIndex = originalString.IndexOf(oldValue, startIndex, comparisonType);
                if (startIndex == -1)
                    break;

                originalString = originalString.Substring(0, startIndex) + newValue + originalString.Substring(startIndex + oldValue.Length);

                startIndex += newValue.Length;
            }

            return originalString;
        }
    }
}
