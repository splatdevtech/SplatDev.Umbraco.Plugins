using FormBuilder.Core.Interfaces;

using System.Globalization;

namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// </summary>
    public class FormatDate : IParsedPlaceholderFormatter
    {
        /// <inheritdoc />
        public virtual string FunctionName => "date";

        /// <inheritdoc />
        public virtual string FormatValue(string value, string[] args)
        {
            if (args.Length == 0)
                return value;
            string format = args[0];
            return DateTime.TryParse(value, out DateTime result) || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out result) ? result.ToString(format) : value;
        }
    }
}