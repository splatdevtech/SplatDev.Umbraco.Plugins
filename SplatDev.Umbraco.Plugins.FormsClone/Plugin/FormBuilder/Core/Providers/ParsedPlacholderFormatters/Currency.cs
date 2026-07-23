namespace FormBuilder.Core.Providers.ParsedPlacholderFormatters
{
    /// <summary>
    /// Implements a     /// </summary>
    public class Currency : FormatNumber
    {
        /// <inheritdoc />
        public override string FunctionName => "currency";

        /// <inheritdoc />
        public override string FormatValue(string value, string[] args) => base.FormatValue(value,
        [
            "C"
        ]);
    }
}