namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a field setting.</summary>
    [Serializable]
    public class FieldSettingViewModel
    {
        /// <summary>Gets or sets the settings's key.</summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>Gets or sets the settings's value.</summary>
        public string Value { get; set; } = string.Empty;
    }
}