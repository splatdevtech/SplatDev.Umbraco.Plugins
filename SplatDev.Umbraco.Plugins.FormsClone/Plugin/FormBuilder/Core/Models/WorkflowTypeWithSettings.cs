using FormBuilder.Core.Workflows;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a workflow type representation.</summary>
    [DataContract(Name = "workflowType")]
    [Serializable]
    public class WorkflowTypeWithSettings : IProviderTypeWithSettings
    {
        /// <summary>Gets or sets the workflow type's Id.</summary>
        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets the unique ID for the field type (with the field name required for front-end rendering).
        /// </summary>
        [DataMember(Name = "unique")]
        public Guid Unique => Id;

        /// <summary>
        /// Gets the field type entity type (required for front-end rendering).
        /// </summary>
        [DataMember(Name = "entityType")]
        public static string EntityType => "workflow-type";

        /// <summary>Gets or sets the workflow type's alias.</summary>
        [DataMember(Name = "alias")]
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow type's name.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow type's description.</summary>
        [DataMember(Name = "description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow type's icon.</summary>
        [DataMember(Name = "icon")]
        public string Icon { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow type's group.</summary>
        [DataMember(Name = "group")]
        public string Group { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow type's settings.</summary>
        [DataMember(Name = "settings")]
        public IEnumerable<Setting> Settings { get; set; } = Enumerable.Empty<Setting>();

        public static implicit operator WorkflowTypeWithSettings(WorkflowType v)
        {
            throw new NotImplementedException();
        }
    }
}