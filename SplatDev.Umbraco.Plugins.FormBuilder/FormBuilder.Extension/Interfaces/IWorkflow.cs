namespace FormBuilder.Extension.Interfaces
{
    /// <summary>
    /// Interface for defining workflows in the Form Builder plugin.
    /// </summary>
    public interface IWorkflow
    {
        /// <summary>
        /// The identifier of the workflow.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The name of the workflow.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// The description of the workflow.
        /// </summary>
        string Description { get; set; }
    }
}
