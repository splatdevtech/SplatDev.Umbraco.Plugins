using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a hidden field type for a form.</summary>
    [Serializable]
    public class HiddenField : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public HiddenField()
        {
            Id = new Guid("DA206CAE-1C52-434E-B21A-4A7C198AF877");
            Name = "Hidden";
            Alias = "hidden";
            Description = "Renders a HTML hidden field";
            Icon = "icon-checkbox-dotted";
            DataType = FieldDataType.String;
            Category = "Simple";
            SortOrder = 120;
            FieldTypeViewName = "FieldType.HiddenField.cshtml";
            EditView = "textfield";
            PreviewView = "Forms.FieldPreview.HiddenField";
            RenderInputType = RenderInputType.Custom;
        }

        /// <summary>Gets or sets a default value for the form field.</summary>
        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool HideLabel => true;
    }
}