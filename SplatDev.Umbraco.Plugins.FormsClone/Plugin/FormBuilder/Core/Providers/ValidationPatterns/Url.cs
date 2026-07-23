using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.ValidationPatterns
{
    /// <summary>
    /// Implements a     /// </summary>
    public class Url : IValidationPattern
    {
        /// <inheritdoc />
        public string Alias => "url";

        /// <inheritdoc />
        public string Name => string.Empty;

        /// <inheritdoc />
        public string LabelKey => "validation_validateAsUrl";

        /// <inheritdoc />
        public string Pattern => "https?\\:\\/\\/[a-zA-Z0-9\\-\\.]+\\.[a-zA-Z]{2,}";

        /// <inheritdoc />
        public bool ReadOnly => true;
    }
}