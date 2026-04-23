using Microsoft.AspNetCore.Html;

namespace FormBuilder.Core.Models
{
    public class FormsHtmlModel
    {
        private readonly Dictionary<string, FormFieldHtmlModel> _fields;

        public FormsHtmlModel(FormFieldHtmlModel[] fields)
        {
            _fields = fields.ToDictionary(x => x.Alias);
            Fields = fields;
            DynamicFields = FormsHtmlModelHelpers.ToDynamic(_fields);
        }

        public Guid FormId { get; set; }

        public string FormName { get; set; } = string.Empty;

        public int FormPageId { get; set; }

        public string FormPageUrl { get; set; } = string.Empty;

        public Guid EntryUniqueId { get; set; }

        public DateTime FormSubmittedOn { get; set; }

        public IHtmlContent? HeaderHtml { get; set; }

        public IHtmlContent? BodyHtml { get; set; }

        public IHtmlContent? FooterHtml { get; set; }

        public object DynamicFields { get; private set; }

        public IEnumerable<FormFieldHtmlModel> Fields { get; private set; }

        public Dictionary<Guid, Dictionary<string, string?>?>? PrevalueMaps { get; set; } = [];

        public IDictionary<string, string> WorkflowSettings { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public object? GetValue(string alias)
        {
            return _fields.TryGetValue(alias, out FormFieldHtmlModel? formFieldHtmlModel) && formFieldHtmlModel is not null && formFieldHtmlModel.FieldValue.Length != 0 ? formFieldHtmlModel.FieldValue[0] : null;
        }

        public object[]? GetValues(string alias)
        {
            return _fields.TryGetValue(alias, out FormFieldHtmlModel? formFieldHtmlModel) && formFieldHtmlModel is not null && formFieldHtmlModel.FieldValue.Length != 0 ? formFieldHtmlModel.FieldValue : null;
        }
    }
}