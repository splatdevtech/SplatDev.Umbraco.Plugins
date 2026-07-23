using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.ValidationPatterns
{
    /// <summary>
    /// Implements a     /// </summary>
    public class Number : IValidationPattern
    {
        /// <inheritdoc />
        public string Alias => "number";

        /// <inheritdoc />
        public string Name => string.Empty;

        /// <inheritdoc />
        public string LabelKey => "validation_validateAsNumber";

        /// <inheritdoc />
        public string Pattern => "^[0-9]*$";

        /// <inheritdoc />
        public bool ReadOnly => true;
    }
}