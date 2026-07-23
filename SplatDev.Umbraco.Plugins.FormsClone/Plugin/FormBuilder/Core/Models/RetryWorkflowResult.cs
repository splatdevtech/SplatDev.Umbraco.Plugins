using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a result type for an retry workflow operation.</summary>
    [DataContract(Name = "retryWorkflowResult")]
    [Serializable]
    public class RetryWorkflowResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the operation succeeded.
        /// </summary>
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets a value indicating a reason why the operation failed.
        /// </summary>
        [DataMember(Name = "message")]
        public string? Message { get; set; }
    }
}