using Core.Exceptions;

namespace Core.Tools
{
    public static class BoolTools
    {
        public static bool GetBoolOrDefault(object input, bool defaultValue = false)
        {
            if (input == null) return defaultValue;
            var value = GetBoolOrNull(input);
            return value ?? defaultValue;
        }

        public static bool? GetBoolOrNull(object input)
        {
            if (input == null) return null;
            if (input is string)
            {
                var inputString = input.ToString().ToLower();
                if (inputString == "j" || inputString == "ja" || inputString == "y" || inputString == "yes" || inputString == "1")
                    input = "true";
                else if (inputString == "n" || inputString == "nej" || inputString == "no" || inputString == "0")
                    input = "false";
            }
            else if (input is int)
            {
                var inputInt = (int)input;
                if (inputInt == 1)
                    input = "true";
                else if (inputInt == 0)
                    input = "false";
            }
            bool x;
            var success = bool.TryParse(input.ToString(), out x);
            return success
                ? x
                : (bool?)null;
        }

        public static bool GetBoolOrThrow(object input, string errorMessageFormat = "Value {0} cannot be parsed as bool.")
        {
            var value = GetBoolOrNull(input);
            if (value == null) throw new ModelValidationException(string.Format(errorMessageFormat, input));
            return value.Value;
        }

        public static string FormatJN(this bool value)
        {
            return value ? "J" : "N";
        }
        public static string FormatJaNej(this bool value)
        {
            return value ? "Ja" : "Nej";
        }
        public static string FormatJaNejEmpty(this bool? value)
        {
            return value.HasValue ? (value.Value ? "Ja" : "Nej") : "";
        }
        public static string FormatJaEmpty(this bool value)
        {
            return value ? "Ja" : "";
        }

        public static string FormatJNEmpty(this bool? value)
        {
            return value.HasValue ? (value.Value ? "J" : "N") : "";
        }

        public static string FormatYesEmpty(this bool? value) { return FormatYesEmpty(value ?? false); }
        public static string FormatYesEmpty(this bool value)
        {
            return value ? "J" : "";
        }
    }
}
