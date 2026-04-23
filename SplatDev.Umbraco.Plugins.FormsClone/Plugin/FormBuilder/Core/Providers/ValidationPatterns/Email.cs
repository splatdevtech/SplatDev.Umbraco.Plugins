using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.ValidationPatterns
{
    /// <summary>
    /// Implements a     /// </summary>
    public class Email : IValidationPattern
    {
        /// <inheritdoc />
        public string Alias => "email";

        /// <inheritdoc />
        public string Name => string.Empty;

        /// <inheritdoc />
        public string LabelKey => "validation_validateAsEmail";

        /// <inheritdoc />
        public string Pattern => "^[a-zA-Z0-9_\\.\\+-]+@[a-zA-Z0-9-]+\\.[a-zA-Z0-9-\\.]+$";

        /// <inheritdoc />
        public bool ReadOnly => true;
    }
}