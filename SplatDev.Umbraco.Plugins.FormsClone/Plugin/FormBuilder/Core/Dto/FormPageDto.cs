namespace FormBuilder.Core.Dto
{
    public class FormPageDto
    {
        public string? Caption { get; set; }

        public FormConditionDto? Condition { get; set; }

        public IEnumerable<FormFieldsetDto> Fieldsets { get; set; } = [];
    }
}