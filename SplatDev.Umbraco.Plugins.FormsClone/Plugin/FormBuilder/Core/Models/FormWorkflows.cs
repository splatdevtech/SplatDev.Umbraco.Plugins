using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines the form workflows associated with     /// </summary>
    [DataContract(Name = "formWorkflows")]
    [Serializable]
    public class FormWorkflows
    {
        /// <summary>
        /// Gets or sets a collection of workflows to run on form submission.
        /// </summary>
        [DataMember(Name = "onSubmit")]
        public List<FormWorkflowWithTypeSettings> OnSubmit { get; set; } = [];

        /// <summary>
        /// Gets or sets a collection of workflows to run on form approval.
        /// </summary>
        [DataMember(Name = "onApprove")]
        public List<FormWorkflowWithTypeSettings> OnApprove { get; set; } = [];

        /// <summary>
        /// Gets or sets a collection of workflows to run on form reject.
        /// </summary>
        [DataMember(Name = "onReject")]
        public List<FormWorkflowWithTypeSettings> OnReject { get; set; } = [];
    }
}