namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a prevalue.</summary>
    [Serializable]
    public class PrevalueViewModel
    {
        /// <summary>Gets or sets the prevalue's Id.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue's value.</summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>Gets or sets the prevalue's caption.</summary>
        public string? Caption { get; set; }
    }
}