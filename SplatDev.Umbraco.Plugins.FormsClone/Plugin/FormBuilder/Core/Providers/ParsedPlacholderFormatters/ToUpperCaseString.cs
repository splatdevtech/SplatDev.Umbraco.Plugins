using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// </summary>
    public class ToUpperCaseString : IParsedPlaceholderFormatter
    {
        /// <inheritdoc />
        public virtual string FunctionName => "upper";

        /// <inheritdoc />
        public virtual string FormatValue(string value, string[] args) => value.ToUpper();
    }
}