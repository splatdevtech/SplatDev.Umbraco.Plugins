using FormBuilder.Core.Interfaces;

using System.Net;

namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// </summary>
    public class HtmlEncode : IParsedPlaceholderFormatter
    {
        /// <inheritdoc />
        public virtual string FunctionName => "html";

        /// <inheritdoc />
        public virtual string FormatValue(string value, string[] args) => WebUtility.HtmlEncode(value);
    }
}