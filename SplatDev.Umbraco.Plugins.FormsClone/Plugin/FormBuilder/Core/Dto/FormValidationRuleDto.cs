namespace FormBuilder.Core.Dto
{
    public class FormValidationRuleDto
    {
        public string Rule { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public Guid FieldId { get; set; }
    }
}