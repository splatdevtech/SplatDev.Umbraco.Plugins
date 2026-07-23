using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines a result type for a record workflow audit entry.
    /// </summary>
    [DataContract(Name = "recordWorkflowAuditEntry")]
    [Serializable]
    public class RecordWorkflowAuditEntry
    {
        /// <summary>Gets or sets id of the record audit entry.</summary>
        [DataMember(Name = "id")]
        public int Id { get; set; }

        /// <summary>Gets or sets the key of the workflow.</summary>
        [DataMember(Name = "workflowKey")]
        public Guid WorkflowKey { get; set; }

        /// <summary>Gets or sets the name of the workflow.</summary>
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the type name of the workflow.</summary>
        [DataMember(Name = "typeName")]
        public string TypeName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the datetime when the workflow was executed.
        /// </summary>
        [DataMember(Name = "executedOn")]
        public DateTime ExecutedOn { get; set; }

        /// <summary>Gets or sets the stage when the workflow was executed.</summary>
        [DataMember(Name = "executionStage")]
        public string ExecutionStage { get; set; } = string.Empty;

        /// <summary>Gets or sets the workflow execution result.</summary>
        [DataMember(Name = "result")]
        public string Result { get; set; } = string.Empty;
    }
}