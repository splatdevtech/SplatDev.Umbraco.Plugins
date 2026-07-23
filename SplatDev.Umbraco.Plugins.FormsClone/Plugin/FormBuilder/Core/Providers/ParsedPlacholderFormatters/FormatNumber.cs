using FormBuilder.Core.Interfaces;

using System.Globalization;

namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// </summary>
    public class FormatNumber : IParsedPlaceholderFormatter
    {
        /// <inheritdoc />
        public virtual string FunctionName => "number";

        /// <inheritdoc />
        public virtual string FormatValue(string value, string[] args)
        {
            if (args.Length == 0)
                return value;
            string format = args[0];
            if (int.TryParse(value, out int result1) || int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result1))
                return result1.ToString(format);
            return decimal.TryParse(value, out decimal result2) || decimal.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result2) ? result2.ToString(format) : value;
        }
    }
}