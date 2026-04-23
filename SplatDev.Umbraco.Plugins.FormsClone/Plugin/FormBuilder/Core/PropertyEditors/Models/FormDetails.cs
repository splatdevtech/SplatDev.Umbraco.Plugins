namespace FormBuilder.Core.PropertyEditors.Models
{
    public class FormDetails
    {
        public Guid? FormId { get; set; }

        public string? Theme { get; set; }

        public Guid? RedirectToPageId { get; set; }
    }
}