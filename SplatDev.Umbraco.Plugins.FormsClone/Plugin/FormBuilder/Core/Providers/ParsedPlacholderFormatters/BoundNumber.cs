using FormBuilder.Core.Interfaces;

using System.Globalization;

namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// and max value, which is returned if the number is outside the bounds).
    /// </summary>
    /// <remarks>
    /// This has been implemented primarily to have something that uses multiple arguments.
    /// </remarks>
    public class BoundNumber : IParsedPlaceholderFormatter
    {
        /// <inheritdoc />
        public virtual string FunctionName => "bound";

        /// <inheritdoc />
        public virtual string FormatValue(string value, string[] args)
        {
            if (args.Length != 2 || !int.TryParse(args[0], out int result1) || !int.TryParse(args[1], out int result2) || !int.TryParse(value, out int result3) && !int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out result3))
                return value;
            if (result3 < result1)
                return result1.ToString();
            return result3 > result2 ? result2.ToString() : result3.ToString();
        }
    }
}