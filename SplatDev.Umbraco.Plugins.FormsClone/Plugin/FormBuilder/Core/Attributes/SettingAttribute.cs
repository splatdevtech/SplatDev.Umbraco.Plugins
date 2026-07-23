namespace FormBuilder.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingAttribute(string name) : Attribute
    {
        public string Name { get; set; } = name;

        public string Description { get; set; } = string.Empty;

        public string PreValues { get; set; } = string.Empty;

        public string View { get; set; } = "Umb.PropertyEditorUi.TextBox";

        public string Alias { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }

        public bool IsHidden { get; set; }

        public bool IsMandatory { get; set; }

        public bool SupportsPlaceholders { get; set; } = false;

        public bool SupportsHtml { get; set; }

        public bool HtmlEncodeReplacedPlaceholderValues { get; set; } = false;

        public List<string> GetPreValues()
        {
            List<string> preValues = [.. PreValues.Split(',', StringSplitOptions.RemoveEmptyEntries)];
            return preValues;
        }
    }
}