
// Type: Umbraco.Forms.Core.Providers.FieldTypes.DataConsent
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class DataConsent : FieldType
    {
        private static readonly string s_trueString = true.ToString().ToLowerInvariant();

        public DataConsent()
        {
            this.Id = new Guid("A72C9DF9-3847-47CF-AFB8-B86773FD12CD");
            this.Name = "Data Consent";
            this.Alias = "dataConsent";
            this.Description = "Renders a field to ask for data consent";
            this.Icon = "icon-shield";
            this.DataType = FieldDataType.Bit;
            this.Category = "Data Consent";
            this.SortOrder = 80;
            this.FieldTypeViewName = "FieldType.DataConsent.cshtml";
            this.PreviewView = "Forms.FieldPreview.DataConsent";
        }

        [Setting("Accept Copy", Alias = "acceptCopy", Description = "The text used to confirm consent", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string AcceptCopy { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        public override bool HideLabel => this.ShowLabel == "False";

        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            List<string> stringList = new List<string>();
            if (field.Mandatory && !postedValues.Any<object>(x => string.Equals(x.ToString(), DataConsent.s_trueString, StringComparison.InvariantCultureIgnoreCase)) && FieldType.IsFieldVisible(form, field, fieldTypeStorage, placeholderParsingService))
            {
                string str = !string.IsNullOrWhiteSpace(field.RequiredErrorMessage) ? field.RequiredErrorMessage : "Consent is required to store and process the data in this form.";
                stringList.Add(str);
            }
            return stringList;
        }

        public override IEnumerable<object> ConvertFromRecord(
          Field field,
          IEnumerable<object> storedValues)
        {
            List<object> objectList = new List<object>();
            if (storedValues.ToList<object>().Any<object>())
                objectList.Add(true);
            else
                objectList.Add(false);
            return objectList;
        }

        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record = new List<object>();
            if (postedValues.ToList<object>().Any<object>(x => string.Equals(x.ToString(), DataConsent.s_trueString, StringComparison.InvariantCultureIgnoreCase)))
                record.Add(true);
            else
                record.Add(false);
            return record;
        }
    }
}
