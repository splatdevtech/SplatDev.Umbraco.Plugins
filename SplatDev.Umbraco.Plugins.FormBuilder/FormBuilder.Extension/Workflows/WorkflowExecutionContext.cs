using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Extension.Workflows
{
    /// <summary>
    /// Represents the execution context for a workflow.
    /// Contains information about the form submission and related data.
    /// </summary>
    public class WorkflowExecutionContext(int formId, Guid formGuid)
    {
        public FormSubmission? Record { get; set; } // The submitted form record
        [Required] public int FormId { get; set; } = formId;
        [Required] public Guid FormGuid { get; set; } = formGuid;
    }
}
