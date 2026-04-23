namespace FormBuilder.Core.Models
{
    /// <summary>Model POSTed to copy a form.</summary>
    public class CopyFormModel
    {
        /// <summary>Gets or sets the new form's name.</summary>
        public string? NewName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether workflows should also be copied along with the form.
        /// </summary>
        public bool CopyWorkflows { get; set; }

        /// <summary>Gets or sets the folder to copy the form to.</summary>
        public string? CopyToFolderId { get; set; }
    }
}