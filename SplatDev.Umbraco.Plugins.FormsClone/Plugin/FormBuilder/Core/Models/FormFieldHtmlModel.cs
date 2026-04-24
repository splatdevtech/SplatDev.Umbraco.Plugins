namespace FormBuilder.Core.Models
{
    public class FormFieldHtmlModel
    {
        public FormFieldHtmlModel(string alias) => Alias = !string.IsNullOrWhiteSpace(alias) ? alias : throw new ArgumentNullException(nameof(alias));

        public string Alias { get; private set; } = string.Empty;

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string FieldType { get; set; } = string.Empty;

        public object[] FieldValue { get; set; } = [];

        public object? GetValue() => FieldValue.Length != 0 ? FieldValue[0] : null;

        public object[] GetValues() => FieldValue.Length != 0 ? FieldValue : [];
    }
}