using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Http;

using Umbraco.Extensions;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a checkbox field type for a form.</summary>
    [Serializable]
    public class CheckBox : FieldType
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public CheckBox()
        {
            Id = new Guid("D5C0C390-AE9A-11DE-A69E-666455D89593");
            Name = "Checkbox";
            Alias = "checkbox";
            Description = "Renders a checkbox";
            Icon = "icon-checkbox";
            DataType = FieldDataType.Bit;
            Category = "Simple";
            SortOrder = 40;
            FieldTypeViewName = "FieldType.CheckBox.cshtml";
            EditView = "Umb.PropertyEditorUi.Toggle";
            PreviewView = "Forms.FieldPreview.Checkbox";
            ShowLabel = "True";
        }

        /// <summary>Gets or sets the text used to confirm consent.</summary>
        [Setting("Caption", Alias = "caption", Description = "The text associated with the checkbox", DisplayOrder = 10, SupportsPlaceholders = true)]
        public virtual string Caption { get; set; } = string.Empty;

        /// <summary>Gets or sets a default value for the form field.</summary>
        [Setting("Default Value", Description = "Enter a default value.", DisplayOrder = 20)]
        public virtual string DefaultValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 30, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override IEnumerable<string> ValidateField(
          Form form,
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context,
          IPlaceholderParsingService placeholderParsingServicee,
          IFieldTypeStorage fieldTypeStorage)
        {
            List<object> objectList = [.. postedValues];
            if (field.Mandatory && IsUnchecked(objectList))
                objectList = [];
            return base.ValidateField(form, field, objectList, context, placeholderParsingServicee, fieldTypeStorage);
        }

        private static bool IsUnchecked(List<object> submittedValues)
        {
            if (submittedValues == null || submittedValues.Count == 0)
                return true;
            string compare = string.Join(",", submittedValues);
            if (string.IsNullOrEmpty(compare))
                return true;
            return !compare.InvariantContains("on") && !compare.InvariantContains("true");
        }

        /// <inheritdoc />
        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> record = [];
            object[] array = [.. postedValues];
            if (array.Length != 0 && array.First().ToString()?.ToLowerInvariant() == "true")
                record.Add(true);
            else
                record.Add(false);
            return record;
        }
    }
}