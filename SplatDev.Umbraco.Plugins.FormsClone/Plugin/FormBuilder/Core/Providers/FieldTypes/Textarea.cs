using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;

using System.Runtime.CompilerServices;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a text area field type for a form.</summary>
    [Serializable]
    public class Textarea : FieldType
    {
        /// <summary>
        /// Defines the default number of rows displayed for the text area form field entry, if not overridden
        /// by the         /// </summary>
        public const int DefaultNumberOfRows = 2;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public Textarea()
        {
            Id = new Guid("023F09AC-1445-4BCB-B8FA-AB49F33BD046");
            Name = "Long answer";
            Alias = "longAnswer";
            Description = "Renders a textarea, designed for longer answers";
            Icon = "icon-autofill";
            DataType = FieldDataType.LongString;
            Category = "Simple";
            SortOrder = 20;
            ShowLabel = "True";
            FieldTypeViewName = "FieldType.Textarea.cshtml";
            EditView = "Umb.PropertyEditorUi.TextArea";
            PreviewView = "Forms.FieldPreview.TextArea";
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
        public virtual string ShowLabel { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <summary>
        /// Gets or sets the form field's autocomplete attribute value.
        /// </summary>
        [Setting("Autocomplete attribute", Description = "Optionally enter a value for the autocomplete attribute.", DisplayOrder = 40)]
        public virtual string AutocompleteAttribute { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of rows available for entry in the text field.
        /// </summary>
        [Setting("Number Of Rows", Description = "Enter the number of rows displayed for entry", DisplayOrder = 50, PreValues = "1,50,2", View = "Umb.PropertyEditorUi.Integer")]
        public virtual string NumberOfRows { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the maximum length of the accepted entry in the field.
        /// </summary>
        [Setting("Maximum Length", Description = "Enter the maximum number of characters accepted.", DisplayOrder = 60, PreValues = "1", View = "Umb.PropertyEditorUi.Integer")]
        public virtual string MaximumLength { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool SupportsRegex => true;

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
            if (TryGetMaximumLength(field, out int maximumLength) && maximumLength > 0)
            {
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