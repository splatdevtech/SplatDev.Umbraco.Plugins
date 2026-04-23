using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Exposes configuration information required for the backoffice front-end rendering.
    /// </summary>
    [DataContract]
    public class BackOfficeConfig
    {
        /// <summary>
        /// Gets or sets the maximum number of columns allowed in fieldsets.
        /// </summary>
        [DataMember(Name = "maxNumberOfColumnsInFormGroup")]
        public int MaxNumberOfColumnsInFormGroup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user security is managed via user groups.
        /// </summary>
        [DataMember(Name = "manageSecurityWithUserGroups")]
        public bool ManageSecurityWithUserGroups { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether scheduled record deletion is enabled.
        /// </summary>
        [DataMember(Name = "scheduledRecordDeletionEnabled")]
        public bool ScheduledRecordDeletionEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fieldset legends are mandatory.
        /// </summary>
        [DataMember(Name = "mandatoryFieldsetLegends")]
        public bool MandatoryFieldsetLegends { get; set; }

        /// <summary>
        /// Gets or sets a value listing the file extensions that are disallowed for upload for all forms.
        /// </summary>
        [DataMember(Name = "disallowedFileUploadExtensions")]
        public string DisallowedFileUploadExtensions { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value listing the only file extensions that are allowed for upload for all forms.
        /// </summary>
        [DataMember(Name = "allowedFileUploadExtensions")]
        public string AllowedFileUploadExtensions { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether multi-page form settings are available for editors.
        /// </summary>
        [DataMember(Name = "enableMultiPageFormSettings")]
        public bool EnableMultiPageFormSettings { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether advanced validation rules are available for editors.
        /// </summary>
        [DataMember(Name = "enableAdvancedValidationRules")]
        public bool EnableAdvancedValidationRules { get; set; }
    }
}