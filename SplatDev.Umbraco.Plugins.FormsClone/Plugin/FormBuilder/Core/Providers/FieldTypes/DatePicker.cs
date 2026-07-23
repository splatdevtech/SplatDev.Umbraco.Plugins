using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System.Data.SqlTypes;
using System.Globalization;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a date picker field type for a form.</summary>
    [Serializable]
    public class DatePicker : FieldType
    {
        private readonly IFormThemeResolver _formThemeResolver;
        private readonly DatePickerSettings _datePickerSettings;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public DatePicker(
          IFormThemeResolver formThemeResolver,
          IOptions<DatePickerSettings> datePickerSettings)
        {
            _formThemeResolver = formThemeResolver;
            _datePickerSettings = datePickerSettings.Value;
            Id = new Guid("F8B4C3B8-AF28-11DE-9DD8-EF5956D89593");
            Name = "Date";
            Alias = "date";
            Description = "Renders a date picker";
            Icon = "icon-calendar";
            DataType = FieldDataType.DateTime;
            Category = "Simple";
            SortOrder = 30;
            FieldTypeViewName = "FieldType.cshtml";
            RenderView = "date";
            EditView = "Umb.PropertyEditorUi.DatePicker";
            PreviewView = "Forms.FieldPreview.DatePicker";
            ShowLabel = "True";
        }

        /// <inheritdoc />
        public override bool SupportsRegex => false;

        /// <summary>Gets or sets the form field's placeholder value.</summary>
        [Setting("Placeholder", Description = "Enter a placeholder value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string Placeholder { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <summary>Gets or sets the form field's aria label value.</summary>
        [Setting("Assistive technology label", Description = "Enter a description of how to select a date, used by assistive technologies such as screen readers.", DisplayOrder = 30, SupportsPlaceholders = true)]
        public virtual string AriaLabel { get; set; } = string.Empty;

        /// <inheritdoc />
        public override IEnumerable<string> RequiredPartialViews(
          Func<string?> themeAccessor,
          Field field)
        {
            return
            [
                _formThemeResolver.GetDatePicker(themeAccessor())!
            ];
        }

        /// <summary>Processes the posted values.</summary>
        public virtual IEnumerable<object> ProcessValue(IEnumerable<object> postedValues) => DatePickerHelpers.ProcessFieldValues(postedValues);

        /// <inheritdoc />
        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            return DatePickerHelpers.ProcessFieldValues(postedValues);
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
            object[] array = [.. postedValues.Take(2)];
            List<string> errorMessages = [.. base.ValidateField(form, field, array, context, placeholderParsingService, fieldTypeStorage)];
            foreach (object obj in array)
            {
                if (obj is string str)
                    ValidateDate(str, errorMessages);
            }
            return errorMessages;
        }

        /// <summary>
        /// Validates a date string and, if invalid, adds errors to a collection of error messages.
        /// </summary>
        /// <param name="value">The date value.</param>
        /// <param name="errorMessages">The error messages.</param>
        public void ValidateDate(string value, IList<string> errorMessages)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(_datePickerSettings.DatePickerFormatForValidation) && DateTime.TryParseExact(value, _datePickerSettings.DatePickerFormatForValidation, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result) || DateTime.TryParse(value, out result))
                flag = true;
            else
                errorMessages.Add("Please enter a valid date");
            if (!flag || !(result < SqlDateTime.MinValue.Value))
                return;
            errorMessages.Add(string.Format("You cannot enter a date earlier than {0}", SqlDateTime.MinValue.Value.ToLongDateString()));
        }
    }
}