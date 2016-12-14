using System.Collections.Generic;
using System.Globalization;
using Core.Exceptions;

namespace Core.Tools
{
    public static class NumberTools
    {
        // todo: this should be done in culture settings instead
        public static string ToDotString(this double number)
        {
            return number.ToString().Replace(',', '.');
        }

        // todo: this should be done in culture settings instead
        public static string ToDotString(this decimal number)
        {
            return number.ToString().Replace(',', '.');
        }

        public static int GetIntOrDefault(object input, int defaultValue = 0)
        {
            if (input == null) return defaultValue;
            if (input is int) return (int)input;
            int x;
            var success = int.TryParse(input.ToString(), out x);
            return success
                ? x
                : defaultValue;
        }

        /// <param name="errorMessageFormat">{0} can be used for the input value</param>
        public static int GetIntOrThrow(object input, string errorMessageFormat = "Value {0} cannot be parsed as int.")
        {
            var value = GetIntOrNull(input);
            if (value == null) throw new ModelValidationException(string.Format(errorMessageFormat, input));
            return value.Value;
        }

        public static int? GetIntOrNull(object input)
        {
            if (input == null) return null;
            if (input is int?) return (int?)input;
            int x;
            var success = int.TryParse(input.ToString(), out x);
            return success
                ? x
                : (int?)null;
        }

        public static double GetDoubleOrDefault(object input, double defaultValue = 0)
        {
            if (input == null) return defaultValue;
            if (input is double) return (double)input;
            double x;
            var success = double.TryParse(input.ToString(), NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture.NumberFormat, out x);
            return success
                ? x
                : defaultValue;
        }

        /// <param name="errorMessageFormat">{0} can be used for the input value</param>
        public static double GetDoubleOrThrow(object input, string errorMessageFormat = "Value {0} cannot be parsed as double.")
        {
            var value = GetDoubleOrNull(input);
            if (value == null) throw new ModelValidationException(string.Format(errorMessageFormat, input));
            return value.Value;
        }

        public static double? GetDoubleOrNull(object input)
        {
            if (input == null) return null;
            if (input is double?) return (double?)input;
            double x;
            var success = double.TryParse(input.ToString().Replace(',', '.'), NumberStyles.Float | NumberStyles.AllowThousands, NumberFormatInfo.InvariantInfo, out x);
            return success
                ? x
                : (double?)null;
        }

        public static decimal GetMedian(List<decimal> values)
        {
            var size = values.Count;
            var mid = size / 2;
            decimal median = (size % 2 != 0) ? (decimal)values[mid] :
            ((decimal)values[mid] + (decimal)values[mid - 1]) / 2;
            return median;
        }

        public static long GetLongOrDefault(object input, long defaultValue = 0)
        {
            if (input == null) return defaultValue;
            if (input is long) return (long)input;
            long x;
            var success = long.TryParse(input.ToString(), out x);
            return success
                ? x
                : defaultValue;
        }

        /// <param name="errorMessageFormat">{0} can be used for the input value</param>
        public static long GetLongOrThrow(object input, string errorMessageFormat = "Value {0} cannot be parsed as long.")
        {
            var value = GetLongOrNull(input);
            if (value == null) throw new ModelValidationException(string.Format(errorMessageFormat, input));
            return value.Value;
        }

        public static long? GetLongOrNull(object input)
        {
            if (input == null) return null;
            if (input is long?) return (long?)input;
            long x;
            var success = long.TryParse(input.ToString(), out x);
            return success
                ? x
                : (long?)null;
        }

        public static decimal GetDecimalOrDefault(object input, decimal defaultValue = 0)
        {
            return GetDecimalOrNull(input) ?? defaultValue;
        }

        /// <param name="errorMessageFormat">{0} can be used for the input value</param>
        public static decimal GetDecimalOrThrow(object input, string errorMessageFormat = "Value {0} cannot be parsed as decimal.")
        {
            var value = GetDecimalOrNull(input);
            if (value == null) throw new ModelValidationException(string.Format(errorMessageFormat, input));
            return value.Value;
        }

        public static decimal? GetDecimalOrNull(object input)
        {
            if (input == null) return null;
            if (input is decimal?) return (decimal?)input;
            decimal x;
            var success = decimal.TryParse(input.ToString().Replace(',', '.'), NumberStyles.Number, NumberFormatInfo.InvariantInfo, out x);
            return success
                ? x
                : (decimal?)null;
        }

        public static string FormatValueEmpty(this int? value) { return FormatValueEmpty(value ?? 0); }
        public static string FormatValueEmpty(this int value)
        {
            return value == 0 ? "" : value.ToString();
        }

        public static string FormatValueEmpty(this decimal? value) { return FormatValueEmpty(value ?? 0); }
        public static string FormatValueEmpty(this decimal value)
        {
            return value == 0 ? "" : value.ToString();
        }

        public static string FormatThousandSpace(long value)
        {
            return $"{value:# ###}";
        }
    }
}
