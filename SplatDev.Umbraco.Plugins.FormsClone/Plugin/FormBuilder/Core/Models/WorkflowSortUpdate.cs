using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a workflow sort update operation.</summary>
    [DataContract(Name = "workflowSortUpdate")]
    [Serializable]
    public class WorkflowSortUpdate
    {
        /// <summary>
        /// Gets or sets the state on which the updated workflow should execute.
        /// </summary>
        [DataMember(Name = "state")]
        public string State { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the workflow Ids (provided in new sort order).
        /// </summary>
        [DataMember(Name = "guids")]
        public string[] Guids { get; set; } = [];
    }
}