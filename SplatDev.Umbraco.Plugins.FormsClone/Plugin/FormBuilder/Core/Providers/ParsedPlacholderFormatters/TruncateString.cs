using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// </summary>
    public class TruncateString : IParsedPlaceholderFormatter
    {
        /// <inheritdoc />
        public virtual string FunctionName => "truncate";

        /// <inheritdoc />
        public virtual string FormatValue(string value, string[] args)
        {
            int result;
            return args.Length == 0 || !int.TryParse(args[0], out result) || value.Length <= result ? value : value[..result] + "...";
        }
    }
}