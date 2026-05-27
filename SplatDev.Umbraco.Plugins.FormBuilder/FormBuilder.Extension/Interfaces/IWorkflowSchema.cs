using FormBuilder.Extension.Enum;
using FormBuilder.Extension.Workflows;

namespace FormBuilder.Extension.Interfaces
{
    public interface IWorkflowSchema
    {
        /// <summary>
        /// The unique identifier of the workflow.
        /// </summary>
        Guid Guid { get; }

        string Category { get; set; }
        string Description { get; set; }

        /// <summary>
        /// Executes the workflow logic.
        /// </summary>
        /// <param name="context">The execution context containing form submission data.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context);
    }
}