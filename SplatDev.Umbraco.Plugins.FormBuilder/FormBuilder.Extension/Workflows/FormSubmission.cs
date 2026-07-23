using FormBuilder.Extension.Entities;

namespace FormBuilder.Extension.Workflows
{
    /// <summary>
    /// Represents a form submission record.
    /// </summary>
    public class FormSubmission
    {
        public int Id { get; set; } // Unique ID of the form submission
        public int FormId { get; set; } // Unique ID of the form submission
        public string FormName { get; set; } = string.Empty;
        public string SubmittedBy { get; set; } = string.Empty; // Email or identifier of the user who submitted
        public Dictionary<string, FormField> Data { get; set; } = [];// data of form fields and values
    }
}
