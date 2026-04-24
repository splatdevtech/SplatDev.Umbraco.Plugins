namespace FormBuilder.Core.Dto
{
    public class DeliveryApiFormDetailsDto
    {
        public Guid FormId { get; set; }

        public string? Theme { get; set; }

        public Guid? RedirectToPageId { get; set; }

        public FormDto? Form { get; set; }
    }
}