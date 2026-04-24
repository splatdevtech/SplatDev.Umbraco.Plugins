
// Type: Umbraco.Forms.Core.Providers.FieldTypes.CheckBox
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;

using Umbraco.Extensions;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
    [Serializable]
    public class CheckBox : FieldType
    {
        public CheckBox()
        {
            this.Id = new Guid("D5C0C390-AE9A-11DE-A69E-666455D89593");
            this.Name = "Checkbox";
            this.Alias = "checkbox";
            this.Description = "Renders a checkbox";
            this.Icon = "icon-checkbox";
            this.DataType = FieldDataType.Bit;
            this.Category = "Simple";
            this.SortOrder = 40;
            this.FieldTypeViewName = "FieldType.CheckBox.cshtml";
            this.EditView = "Umb.PropertyEditorUi.Toggle";
            this.PreviewView = "Forms.FieldPreview.Checkbox";
            this.ShowLabel = "True";
        }

        [Setting("Caption", Alias = "caption", Description = "The text associated with the checkbox", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string Caption { get; set; } = string.Empty;

        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 20)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        public override bool HideLabel => this.ShowLabel == "False";
        public override Dictionary<FieldConditionRuleOperator, Func<string, string, bool>> ConditionCheckFunctions
        {
            get
            {
                Dictionary<FieldConditionRuleOperator, Func<string, string, bool>> conditionCheckFunctions = base.ConditionCheckFunctions;

                conditionCheckFunctions[FieldConditionRuleOperator.Is] = CheckBoxIs;
                conditionCheckFunctions[FieldConditionRuleOperator.IsNot] = (cv, fv) => !CheckBoxIs(cv, fv);

                return conditionCheckFunctions;

                static bool CheckBoxIs(string cv, string fv)
                {
                    // Normalize checkbox values
                    cv = cv.ToUpperInvariant() switch
                    {
                        "TRUE" or "ON" => "true",
                        "FALSE" or "OFF" => "false",
                        _ => cv
                    };

                    return string.Equals(cv, fv, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingServicee,
          IFieldTypeStorage fieldTypeStorage)
        {
            List<object> objectList = postedValues.ToList<object>();
            if (field.Mandatory && this.IsUnchecked(objectList))
                objectList = new List<object>();
            return base.ValidateField(form, field, objectList, context, placeholderParsingServicee, fieldTypeStorage);
        }

        private bool IsUnchecked(List<object> submittedValues)
        {
            if (submittedValues == null || submittedValues.Count == 0)
                return true;
            string compare = string.Join<object>(",", submittedValues);
            if (string.IsNullOrEmpty(compare))
                return true;
            return !compare.InvariantContains("on") && !compare.InvariantContains("true");
        }

        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record = new List<object>();
            object[] array = postedValues.ToArray<object>();
            if (array.Any<object>() && array.First<object>().ToString()?.ToLowerInvariant() == "true")
                record.Add(true);
            else
                record.Add(false);
            return record;
        }
    }
}
