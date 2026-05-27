using FormBuilder.Extension.Enum;
using FormBuilder.Extension.Interfaces;
using FormBuilder.Extension.Workflows;

using Newtonsoft.Json;

using NPoco;

using System.Text.Json.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Extension.Entities
{
    /// <summary>
    /// Represents a workflow that can be associated with a form.
    /// </summary>
    [TableName(TABLE_NAME)]
    [ExplicitColumns]
    [PrimaryKey("Id", AutoIncrement = true)]
    public class Workflow(string name, string description) : IWorkflow
    {
        public const string TABLE_NAME = "FormBuilderWorkflows";

        /// <inheritdoc />
        [Column("id")]
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("guid")]
        [JsonPropertyName("guid")]
        [JsonProperty("guid")]
        public Guid Guid { get; set; }

        /// <inheritdoc />
        [Column("name")]
        [JsonPropertyName("name")]
        [JsonProperty("name")]
        public string Name { get; set; } = name;

        /// <inheritdoc />
        [Column("description")]
        [JsonPropertyName("description")]
        [JsonProperty("description")]
        public string Description { get; set; } = description;

        /// <inheritdoc />
        [Column("formId")]
        [JsonPropertyName("formId")]
        [JsonProperty("formId")]
        public int FormId { get; set; }

        /// <inheritdoc />
        [Column("workflowType")]
        [JsonPropertyName("workflowType")]
        [JsonProperty("workflowType")]
        public int WorkflowType { get; set; }

        /// <inheritdoc />
        [Ignore]
        [JsonPropertyName("type")]
        [JsonProperty("type")]
        public WorkflowType Type { get; set; }

        /// <inheritdoc />
        [Column("settingsJson")]
        [JsonPropertyName("settingsJson")]
        [JsonProperty("settingsJson")]
        public string? SettingsJson { get; set; } // Store workflow settings as JSON if needed

        /// <inheritdoc />
        [Column("isActive")]
        [JsonPropertyName("isActive")]
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }

        /// <inheritdoc />
        public virtual async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
        {
            // Default implementation (can be overridden by specific workflows)
            return await Task.FromResult(WorkflowExecutionStatus.Completed);
        }
    }
}
