
// Type: Umbraco.Forms.Core.Providers.FieldTypes.DatePicker
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System.Data.SqlTypes;
using System.Globalization;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class DatePicker : FieldType
    {
        private readonly IFormThemeResolver _formThemeResolver;
        private readonly DatePickerSettings _datePickerSettings;

        public DatePicker(
          IFormThemeResolver formThemeResolver,
          IOptions<DatePickerSettings> datePickerSettings)
        {
            this._formThemeResolver = formThemeResolver;
            this._datePickerSettings = datePickerSettings.Value;
            this.Id = new Guid("F8B4C3B8-AF28-11DE-9DD8-EF5956D89593");
            this.Name = "Date";
            this.Alias = "date";
            this.Description = "Renders a date picker";
            this.Icon = "icon-calendar";
            this.DataType = FieldDataType.DateTime;
            this.Category = "Simple";
            this.SortOrder = 30;
            this.FieldTypeViewName = "FieldType.DatePicker.cshtml";
            this.RenderView = "date";
            this.EditView = "Umb.PropertyEditorUi.DatePicker";
            this.PreviewView = "Forms.FieldPreview.DatePicker";
            this.ShowLabel = "True";
        }

        public override bool SupportsRegex => false;

        [Setting("Placeholder", Description = "Enter a placeholder value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string Placeholder { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        public override bool HideLabel => this.ShowLabel == "False";

        [Setting("Assistive technology label", Description = "Enter a description of how to select a date, used by assistive technologies such as screen readers.", DisplayOrder = 30, SupportsPlaceholders = true)]
        public virtual string AriaLabel { get; set; } = string.Empty;

        public override IEnumerable<string> RequiredPartialViews(
          Func<string?> themeAccessor,
          Field field)
        {
            return new string[1]
            {
        this._formThemeResolver.GetDatePicker(themeAccessor())
            };
        }

        public virtual IEnumerable<object> ProcessValue(IEnumerable<object> postedValues) => DatePicker.ProcessFieldValues(postedValues);

        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            return DatePicker.ProcessFieldValues(postedValues);
        }

        private static IEnumerable<object> ProcessFieldValues(
          IEnumerable<object> postedValues)
        {
            List<object> objectList = new List<object>();
            postedValues = postedValues.ToArray<object>();
            if (!postedValues.Any<object>())
                return objectList;
            DateTime result;
            if (DateTime.TryParse(postedValues.First<object>().ToString(), out result))
                objectList.Add(result);
            return objectList;
        }

        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            object[] array = postedValues.Take<object>(2).ToArray<object>();
            List<string> errorMessages = new List<string>(base.ValidateField(form, field, array, context, placeholderParsingService, fieldTypeStorage));
            foreach (object obj in array)
            {
                if (obj is string str)
                    this.ValidateDate(str, errorMessages);
            }
            return errorMessages;
        }

        public void ValidateDate(string value, IList<string> errorMessages)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            DateTime result = DateTime.MinValue;
            bool flag = false;
            if (!string.IsNullOrWhiteSpace(this._datePickerSettings.DatePickerFormatForValidation) && DateTime.TryParseExact(value, this._datePickerSettings.DatePickerFormatForValidation, CultureInfo.InvariantCulture, DateTimeStyles.None, out result) || DateTime.TryParse(value, out result))
                flag = true;
            else
                errorMessages.Add("Please enter a valid date");
            if (!flag || !(result < SqlDateTime.MinValue.Value))
                return;
            errorMessages.Add(string.Format("You cannot enter a date earlier than {0}", SqlDateTime.MinValue.Value.ToLongDateString()));
        }
    }
}
