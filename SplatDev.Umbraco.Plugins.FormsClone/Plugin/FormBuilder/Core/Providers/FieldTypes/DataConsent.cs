using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a data use consent field type for a form.</summary>
    [Serializable]
    public class DataConsent : FieldType
    {
        private static readonly string s_trueString = true.ToString().ToLowerInvariant();

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public DataConsent()
        {
            Id = new Guid("A72C9DF9-3847-47CF-AFB8-B86773FD12CD");
            Name = "Data Consent";
            Alias = "dataConsent";
            Description = "Renders a field to ask for data consent";
            Icon = "icon-shield";
            DataType = FieldDataType.Bit;
            Category = "Data Consent";
            SortOrder = 80;
            FieldTypeViewName = "FieldType.DataConsent.cshtml";
            PreviewView = "Forms.FieldPreview.DataConsent";
        }

        /// <summary>Gets or sets the text used to confirm consent.</summary>
        [Setting("Accept Copy", Alias = "acceptCopy", Description = "The text used to confirm consent", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string AcceptCopy { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <inheritdoc />
        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            List<string> stringList = [];
            if (field.Mandatory && !postedValues.Any(x => string.Equals(x.ToString(), s_trueString, StringComparison.OrdinalIgnoreCase)) && IsFieldVisible(form, field, fieldTypeStorage, placeholderParsingService))
            {
                string str = !string.IsNullOrWhiteSpace(field.RequiredErrorMessage) ? field.RequiredErrorMessage : "Consent is required to store and process the data in this form.";
                stringList.Add(str);
            }
            return stringList;
        }

        /// <summary>
        /// Used when we fetch the record or edit the form submission
        /// Will need to map the true/false bool back to accepted or declined so that the correct radio button is selected
        /// </summary>
        public override IEnumerable<object> ConvertFromRecord(
          Field field,
          IEnumerable<object> storedValues)
        {
            List<object> objectList = [(storedValues.ToList().Count != 0)];
            return objectList;
        }

        /// <summary>
        /// When a form is submitted - we converted the hardcoded radiobutton input value of 'accepted' to a true bool to be stored in the DB
        /// </summary>
        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record =
            [
                (postedValues.ToList().Any(x => string.Equals(x.ToString(), s_trueString, StringComparison.OrdinalIgnoreCase))),
            ];
            return record;
        }
    }
}