using FormBuilder.Extension.Entities;

using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Extension.Models
{
    /// <summary>
    /// Represents a submission to a form, including dynamic field values.
    /// </summary>
    public class FormSubmissionModel
    {
        /// <summary>
        /// The ID of the form being submitted.
        /// </summary>
        [Required]
        public int FormId { get; set; }

        [Required]
        public Guid FormGuid { get; set; }

        /// <summary>
        /// The name of the submitter (static field, always present).
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The email of the submitter (static field, always present).
        /// </summary>
        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// The dynamic field values, keyed by field alias or ID.
        /// </summary>
        [Required]
        public Dictionary<string, FormField> Fields { get; set; } = [];

        /// <summary>
        /// The submission date/time (set by server).
        /// </summary>
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The IP address of the submitter (optional, set by server).
        /// </summary>
        public string? IpAddress { get; set; }

        /// <summary>
        /// The status of the submission (optional, set by workflow or admin).
        /// </summary>
        public string? Status { get; set; }
    }
}
