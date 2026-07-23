namespace FormBuilder.Core.Providers
{
    public enum ProviderValidationResult
    {
        /// <summary>Validation was successful.</summary>
        Success,

        /// <summary>Validation failed due to type not being found.</summary>
        FailedTypeNotFound,

        /// <summary>Validation failed due to validation error.</summary>
        FailedValidation,
    }
}