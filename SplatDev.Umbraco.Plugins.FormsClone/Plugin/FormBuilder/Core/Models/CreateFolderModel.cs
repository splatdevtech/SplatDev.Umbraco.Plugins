using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.Models
{
    /// <summary>Model POSTed to create a folder.</summary>
    public class CreateFolderModel
    {
        /// <summary>Gets or sets the folder's Id.</summary>
        [Required]
        public Guid Id { get; set; }

        /// <summary>Gets or sets the parent folder Id (or "-1" for root).</summary>
        public Guid? ParentId { get; set; }

        /// <summary>Gets or sets the name for the new folder.</summary>
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}