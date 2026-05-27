using FormBuilder.Extension.Enum;
using FormBuilder.Extension.Interfaces;

namespace FormBuilder.Extension.Workflows
{
    /// <summary>
    /// Base class for defining custom workflows in the Form Builder plugin.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="WorkflowSchema"/> class.
    /// </remarks>
    /// <param name="name">The name of the workflow.</param>
    /// <param name="category">The category of the workflow.</param>
    /// <param name="description">The description of the workflow.</param>
    public abstract class WorkflowSchema(string name, string category, string description) : IWorkflowSchema
    {
        /// <summary>
        /// The name of the workflow.
        /// </summary>
        public virtual string Name { get; set; } = name;

        /// <summary>
        /// The category of the workflow (e.g., "Custom", "Email").
        /// </summary>
        public virtual string Category { get; set; } = category;

        /// <summary>
        /// A description of what the workflow does.
        /// </summary>
        public virtual string Description { get; set; } = description;

        /// <summary>
        /// A description of what the Guid does.
        /// </summary>
        public virtual Guid Guid { get; }

        /// <summary>
        /// Executes the workflow logic.
        /// This method must be implemented by derived classes to define custom behavior.
        /// </summary>
        /// <param name="context">The execution context containing form submission data.</param>
        /// <returns>A task representing the asynchronous operation, with a status indicating success or failure.</returns>
        public abstract Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context);

    }
}
