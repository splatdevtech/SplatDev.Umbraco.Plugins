using System.ComponentModel.DataAnnotations;

namespace FormBuilder.Core.Models
{
    /// <summary>Model POSTed to rename a folder.</summary>
    public class UpdateFolderModel
    {
        /// <summary>Gets or sets the name for the renamed folder.</summary>
        [Required]
        public string Name { get; set; } = string.Empty;
    }
}