// Type: Umbraco.Forms.Core.Models.FormFieldHtmlModel
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389


#nullable enable
namespace Umbraco.Forms.Core.Models
{
    public class FormFieldHtmlModel
    {
        public FormFieldHtmlModel(string alias) => this.Alias = !string.IsNullOrWhiteSpace(alias) ? alias : throw new ArgumentNullException(nameof(alias));

        public string Alias { get; private set; } = string.Empty;

        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string FieldType { get; set; } = string.Empty;

        public object[] FieldValue { get; set; } = Array.Empty<object>();

        public object? GetValue() => this.FieldValue.Length != 0 ? this.FieldValue[0] : null;

        public object[] GetValues() => this.FieldValue.Length != 0 ? this.FieldValue : Array.Empty<object>();
    }
}
