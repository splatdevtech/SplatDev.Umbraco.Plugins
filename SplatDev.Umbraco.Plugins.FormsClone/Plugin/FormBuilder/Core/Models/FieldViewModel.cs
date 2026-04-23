using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Interfaces;

using Microsoft.AspNetCore.Html;

using System.Web;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a field.</summary>
    [Serializable]
    public class FieldViewModel : ISupportFileUploads
    {
        /// <summary>Gets or sets the field's Id.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's fieldset's Id.</summary>
        public Guid FieldsetId { get; set; }

        /// <summary>Gets or sets the field's page's Id.</summary>
        public Guid PageId { get; set; }

        /// <summary>Gets or sets the field's form's Id.</summary>
        public Guid FormId { get; set; }

        /// <summary>Gets or sets the field's alias.</summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's caption.</summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field is mandatory for completion.
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the field's validation message to display if it's mandatory and not completed.
        /// </summary>
        public string RequiredErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field should be validated against a regular expression.
        /// </summary>
        public bool Validate { get; set; }

        /// <summary>
        /// Gets or sets a regular expression to validate the field's entry against.
        /// </summary>
        public string Regex { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the field's validation message to display if it doesn't match the provided regular expression.
        /// </summary>
        public string InvalidErrorMessage { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's type's name.</summary>
        public string FieldTypeName { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's type..</summary>
        public FieldType? FieldType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the label for the field should be hidden.
        /// </summary>
        public bool HideLabel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the indicator for the field should be shown.
        /// </summary>
        public bool ShowIndicator { get; set; }

        /// <summary>Gets or sets the field's tooltip.</summary>
        public string ToolTip { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's CSS class.</summary>
        public string CssClass { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's prevalues.</summary>
        public IEnumerable<PrevalueViewModel> PreValues { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether any file extension is allowed for upload (if uploads for the field are supported).
        /// </summary>
        public bool AllowAllUploadExtensions { get; set; }

        /// <summary>
        /// Gets or sets the field's allowed upload file types (if uploads for the field are supported and         /// </summary>
        public IEnumerable<string> AllowedUploadExtensions { get; set; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether multiple file uploads are allowed (if uploads for the field are supported).
        /// </summary>
        public bool AllowMultipleFileUploads { get; set; }

        /// <summary>Gets or sets the field's settings.</summary>
        public IDictionary<string, string> AdditionalSettings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets a value indicating whether the field has a condition.
        /// </summary>
        public bool HasCondition { get; set; }

        /// <summary>Gets or sets the field's placeholder text.</summary>
        public string PlaceholderText { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's action type.</summary>
        public FieldConditionActionType ConditionActionType { get; set; }

        /// <summary>Gets or sets the field's condition logic type.</summary>
        public FieldConditionLogicType ConditionLogicType { get; set; }

        /// <summary>Gets or sets the field's condition rules.</summary>
        public IEnumerable<FieldConditionRule> ConditionRules { get; set; } = [];

        /// <summary>Gets or sets the field's parent conditions.</summary>
        public IEnumerable<FieldCondition> ParentConditions { get; set; } = [];

        /// <summary>Gets or sets the field's condition.</summary>
        public FieldCondition? Condition { get; set; }

        /// <summary>Gets or sets the field's provided values.</summary>
        public IEnumerable<object> Values { get; set; } = [];

        /// <summary>
        /// Gets the field's value as an object (the first value if more than one).
        /// </summary>
        public object? ValueAsObject
        {
            get
            {
                object? valueAsObject = null;
                if (Values is not null && Values.Any())
                    valueAsObject = Values.First();
                return valueAsObject;
            }
        }

        /// <summary>
        /// Gets the field's value as an HTML string (the first value if more than one).
        /// </summary>
        public HtmlString ValueAsHtmlString
        {
            get
            {
                string str = string.Empty;
                object? obj = Values.FirstOrDefault();
                if (obj is not null)
                    str = HttpUtility.HtmlAttributeEncode(obj.ToString()) ?? string.Empty;
                return new HtmlString(str);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the field contains a value.
        /// </summary>
        public bool ContainsValue(object value) => Values is not null && Values.Any() && Values.Select(x => x?.ToString()!).Contains(value);
    }
}