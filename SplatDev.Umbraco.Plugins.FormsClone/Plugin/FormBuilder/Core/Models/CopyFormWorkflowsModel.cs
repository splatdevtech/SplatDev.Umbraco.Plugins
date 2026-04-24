namespace FormBuilder.Core.Models
{
    /// <summary>Model POSTed to copy the workflows of a form.</summary>
    public class CopyFormWorkflowsModel
    {
        /// <summary>Gets or sets the destination form's key.</summary>
        public Guid DestinationId { get; set; }

        /// <summary>Gets or sets the IDs of the workflows to copy.</summary>
        public IEnumerable<Guid> WorkflowIds { get; set; } = [];
    }
}