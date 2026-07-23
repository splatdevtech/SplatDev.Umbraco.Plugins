using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// </summary>
    public class ToLowerCaseString : IParsedPlaceholderFormatter
    {
        /// <inheritdoc />
        public virtual string FunctionName => "lower";

        /// <inheritdoc />
        public virtual string FormatValue(string value, string[] args) => value.ToLower();
    }
}