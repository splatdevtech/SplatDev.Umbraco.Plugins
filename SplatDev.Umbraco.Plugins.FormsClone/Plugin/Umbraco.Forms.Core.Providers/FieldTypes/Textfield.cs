
// Type: Umbraco.Forms.Core.Providers.FieldTypes.Textfield
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class Textfield : Umbraco.Forms.Core.FieldType
    {
        public Textfield()
        {
            this.Id = new Guid("3F92E01B-29E2-4A30-BF33-9DF5580ED52C");
            this.Name = "Short answer";
            this.Alias = "shortAnswer";
            this.Description = "Renders an text input field, for short answers";
            this.Icon = "icon-autofill";
            this.DataType = FieldDataType.String;
            this.Category = "Simple";
            this.SortOrder = 10;
            this.ShowLabel = "True";
            this.FieldTypeViewName = "FieldType.Textfield.cshtml";
            this.EditView = "Umb.PropertyEditorUi.TextBox";
            this.PreviewView = "Forms.FieldPreview.TextBox";
        }

        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        [Setting("Placeholder", Description = "Enter a HTML5 placeholder value.", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string Placeholder { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        [Setting("Maximum Length", Description = "Enter the maximum number of characters accepted.", DisplayOrder = 40, PreValues = "1,255", View = "Umb.PropertyEditorUi.Integer")]
        public virtual string MaximumLength { get; set; } = string.Empty;

        [Setting("Field Type", Description = "Select the type of information expected.", DisplayOrder = 50, PreValues = "date,datetime-local,email,tel,text,number,time,url,week", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string FieldType { get; set; } = string.Empty;

        public override bool HideLabel => this.ShowLabel == "False";

        [Setting("Autocomplete attribute", Description = "Optionally enter a value for the autocomplete attribute.", DisplayOrder = 60)]
        public virtual string AutocompleteAttribute { get; set; } = string.Empty;

        public override bool SupportsRegex => true;

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            int result;
            if (int.TryParse(this.MaximumLength, out result) && result > byte.MaxValue)
                exceptionList.Add(new Exception("The maxmimum length setting for the 'short answer' field cannot be more than 255 characters."));
            return exceptionList;
        }

        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingService,
          IFieldTypeStorage fieldTypeStorage)
        {
            List<string> list = base.ValidateField(form, field, postedValues, context, placeholderParsingService, fieldTypeStorage).ToList<string>();
            int maximumLength;
            if (!this.TryGetMaximumLength(field, out maximumLength))
                maximumLength = byte.MaxValue;
            string str = postedValues.FirstOrDefault<object>()?.ToString();
            if (!string.IsNullOrEmpty(str) && str.Length > maximumLength)
            {
                List<string> stringList = list;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
                interpolatedStringHandler.AppendLiteral("The value provided exceeds ");
                interpolatedStringHandler.AppendFormatted<int>(maximumLength);
                interpolatedStringHandler.AppendLiteral(" characters.");
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                stringList.Add(stringAndClear);
            }
            return list;
        }

        private bool TryGetMaximumLength(Field field, out int maximumLength)
        {
            string s;
            if (field.Settings.TryGetValue("MaximumLength", out s))
                return int.TryParse(s, out maximumLength);
            maximumLength = -1;
            return false;
        }
    }
}
