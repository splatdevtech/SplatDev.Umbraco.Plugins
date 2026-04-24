namespace FormBuilder.Core.Dto
{
    public class FormFieldTypeDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool SupportsPreValues { get; set; }

        public bool SupportsUploadTypes { get; set; }

        public string RenderInputType { get; set; } = Enums.RenderInputType.Single.ToString();
    }
}