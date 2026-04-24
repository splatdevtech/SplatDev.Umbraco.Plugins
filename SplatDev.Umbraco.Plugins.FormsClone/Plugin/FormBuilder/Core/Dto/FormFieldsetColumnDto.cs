namespace FormBuilder.Core.Dto
{
    public class FormFieldsetColumnDto
    {
        public string? Caption { get; set; }

        public int Width { get; set; }

        public IEnumerable<FormFieldDto> Fields { get; set; } = [];
    }
}