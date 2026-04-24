
// Type: Umbraco.Forms.Core.Providers.FieldTypes.Textarea
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
    public class Textarea : FieldType
    {
        public const int DefaultNumberOfRows = 2;

        public Textarea()
        {
            this.Id = new Guid("023F09AC-1445-4BCB-B8FA-AB49F33BD046");
            this.Name = "Long answer";
            this.Alias = "longAnswer";
            this.Description = "Renders a textarea, designed for longer answers";
            this.Icon = "icon-autofill";
            this.DataType = FieldDataType.LongString;
            this.Category = "Simple";
            this.SortOrder = 20;
            this.ShowLabel = "True";
            this.FieldTypeViewName = "FieldType.Textarea.cshtml";
            this.EditView = "Umb.PropertyEditorUi.TextArea";
            this.PreviewView = "Forms.FieldPreview.TextArea";
        }

        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        [Setting("Placeholder", Description = "Enter a HTML5 placeholder value.", DisplayOrder = 20, SupportsPlaceholders = true)]
        public virtual string Placeholder { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; } = string.Empty;

        public override bool HideLabel => this.ShowLabel == "False";

        [Setting("Autocomplete attribute", Description = "Optionally enter a value for the autocomplete attribute.", DisplayOrder = 40)]
        public virtual string AutocompleteAttribute { get; set; } = string.Empty;

        [Setting("Number Of Rows", Description = "Enter the number of rows displayed for entry", DisplayOrder = 50, PreValues = "1,50,2", View = "Umb.PropertyEditorUi.Integer")]
        public virtual string NumberOfRows { get; set; } = string.Empty;

        [Setting("Maximum Length", Description = "Enter the maximum number of characters accepted.", DisplayOrder = 60, PreValues = "1", View = "Umb.PropertyEditorUi.Integer")]
        public virtual string MaximumLength { get; set; } = string.Empty;

        public override bool SupportsRegex => true;

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
            if (this.TryGetMaximumLength(field, out maximumLength) && maximumLength > 0)
            {
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
