using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;

using Microsoft.Extensions.Options;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a password field type for a form.</summary>
    [Serializable]
    public class Password : FieldType
    {
        private readonly SecuritySettings _config;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public Password(IOptions<SecuritySettings> config)
        {
            _config = config.Value;
            Id = new Guid("FB37BC60-D41E-11DE-AEAE-37C155D89593");
            Name = nameof(Password);
            Alias = "password";
            Description = "Renders a password field";
            Icon = "icon-lock";
            DataType = FieldDataType.String;
            Category = "Simple";
            SortOrder = 60;
            FieldTypeViewName = "FieldType.PasswordField.cshtml";
            PreviewView = "Forms.FieldPreview.PasswordField";
            EditView = "password";
            ShowLabel = "True";
        }

        /// <summary>Gets or sets the form field placeholder value.</summary>
        [Setting("Placeholder", Description = "Enter a HTML5 placeholder value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string Placeholder { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <inheritdoc />
        public override bool SupportsRegex => true;

        /// <inheritdoc />
        public override bool StoresData => _config.SavePlainTextPasswords;
    }
}