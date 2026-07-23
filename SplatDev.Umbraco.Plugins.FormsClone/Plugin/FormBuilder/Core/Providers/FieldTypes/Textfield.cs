using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;

using System.Runtime.CompilerServices;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a text field type for a form.</summary>
    [Serializable]
    public class Textfield : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public Textfield()
        {
            Id = new Guid("3F92E01B-29E2-4A30-BF33-9DF5580ED52C");
            Name = "Short answer";
            Alias = "shortAnswer";
            Description = "Renders an text input field, for short answers";
            Icon = "icon-autofill";
            DataType = FieldDataType.String;
            Category = "Simple";
            SortOrder = 10;
            ShowLabel = "True";
            FieldTypeViewName = "FieldType.Textfield.cshtml";
            EditView = "Umb.PropertyEditorUi.TextBox";
            PreviewView = "Forms.FieldPreview.TextBox";
        }

        /// <summary>Gets or sets a default value for the form field.</summary>
        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        /// <summary>Gets or sets the form field's placeholder value.</summary>
        [Setting("Placeholder", Description = "Enter a HTML5 placeholder value.", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string Placeholder { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the accepted entry in the field.
        /// </summary>
        [Setting("Maximum Length", Description = "Enter the maximum number of characters accepted.", DisplayOrder = 40, PreValues = "1,255", View = "Umb.PropertyEditorUi.Integer")]
        public virtual string MaximumLength { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum length of the accepted entry in the field.
        /// </summary>
        [Setting("Field Type", Description = "Select the type of information expected.", DisplayOrder = 50, PreValues = "date,datetime-local,email,tel,text,number,time,url,week", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string FieldType { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <summary>
        /// Gets or sets the form field's autocomplete attribute value.
        /// </summary>
        [Setting("Autocomplete attribute", Description = "Optionally enter a value for the autocomplete attribute.", DisplayOrder = 60)]
        public virtual string AutocompleteAttribute { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool SupportsRegex => true;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (int.TryParse(MaximumLength, out int result) && result > byte.MaxValue)
                exceptionList.Add(new Exception("The maxmimum length setting for the 'short answer' field cannot be more than 255 characters."));
            return exceptionList;
        }

        /// <inheritdoc />
        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            List<string> list = [.. base.ValidateField(form, field, postedValues, context, placeholderParsingService, fieldTypeStorage)];
            if (!TryGetMaximumLength(field, out int maximumLength))
                maximumLength = byte.MaxValue;
            string? str = postedValues.FirstOrDefault()?.ToString();
            if (!string.IsNullOrEmpty(str) && str.Length > maximumLength)
            {
                List<string> stringList = list;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(39, 1);
                interpolatedStringHandler.AppendLiteral("The value provided exceeds ");
                interpolatedStringHandler.AppendFormatted(maximumLength);
                interpolatedStringHandler.AppendLiteral(" characters.");
                string? stringAndClear = interpolatedStringHandler.ToStringAndClear();
                stringList.Add(stringAndClear);
            }
            return list;
        }

        private static bool TryGetMaximumLength(Field field, out int maximumLength)
        {
            if (field.Settings.TryGetValue("MaximumLength", out string? s))
                return int.TryParse(s, out maximumLength);
            maximumLength = -1;
            return false;
        }
    }
}