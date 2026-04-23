namespace FormBuilder.Core.Models
{
    /// <summary>Model POSTed to move a form.</summary>
    public class MoveFormModel
    {
        /// <summary>Gets or sets the folder to move the folder to.</summary>
        public Guid? ParentId { get; set; }
    }
}