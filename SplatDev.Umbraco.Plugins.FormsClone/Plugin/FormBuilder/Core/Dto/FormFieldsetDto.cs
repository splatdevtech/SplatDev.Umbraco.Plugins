namespace FormBuilder.Core.Dto
{
    public class FormFieldsetDto
    {
        public Guid Id { get; set; }

        public string? Caption { get; set; }

        public FormConditionDto? Condition { get; set; }

        public IEnumerable<FormFieldsetColumnDto> Columns { get; set; } = [];
    }
}