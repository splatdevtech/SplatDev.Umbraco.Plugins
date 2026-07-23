using FormBuilder.Core.Enums;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines a form workflow representation associated with     /// </summary>
    [DataContract(Name = "workflow")]
    [Serializable]
    public class FormWorkflowWithTypeSettings
    {
        /// <summary>Gets or sets the workflow's Id.</summary>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>Gets or sets the workflow's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow's form key.</summary>
        [DataMember(Name = "form")]
        public Guid Form { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow is active.
        /// </summary>
        [DataMember(Name = "active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow contains sensitive data.
        /// </summary>
        [DataMember(Name = "includeSensitiveData")]
        public IncludeSensitiveData IncludeSensitiveData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the workflow is active is deleted.
        /// </summary>
        [DataMember(Name = "isDeleted")]
        public bool IsDeleted { get; set; }

        /// <summary>Gets or sets the workflow's sort order.</summary>
        [DataMember(Name = "sortOrder")]
        public int SortOrder { get; set; }

        /// <summary>Gets or sets the Id of the workflow's type.</summary>
        [DataMember(Name = "workflowTypeId")]
        public Guid WorkflowTypeId { get; set; }

        /// <summary>Gets or sets the name of the workflow's type.</summary>
        [DataMember(Name = "workflowTypeName")]
        public string WorkflowTypeName { get; set; } = string.Empty;

        /// <summary>Gets or sets the description of the workflow's type.</summary>
        [DataMember(Name = "workflowTypeDescription")]
        public string WorkflowTypeDescription { get; set; } = string.Empty;

        /// <summary>Gets or sets the icon of the workflow's type.</summary>
        [DataMember(Name = "workflowTypeIcon")]
        public string WorkflowTypeIcon { get; set; } = string.Empty;

        /// <summary>Gets or sets the group of the workflow's type.</summary>
        [DataMember(Name = "workflowTypeGroup")]
        public string WorkflowTypeGroup { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow's settings.</summary>
        [DataMember(Name = "settings")]
        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets or sets a value indicating whether the workflow is mandatory and cannot be removed via the UI.
        /// </summary>
        [DataMember(Name = "isMandatory")]
        public bool IsMandatory { get; set; }

        /// <summary>Gets or sets the workflow's condition.</summary>
        [DataMember(Name = "condition")]
        public FieldCondition? Condition { get; set; }
    }
}