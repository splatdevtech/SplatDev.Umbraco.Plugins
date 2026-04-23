
// Type: Umbraco.Forms.Core.Models.FormsHtmlModel
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  public class FormsHtmlModel
  {
    private readonly IDictionary<string, FormFieldHtmlModel> _fields;

    public FormsHtmlModel(FormFieldHtmlModel[] fields)
    {
      ArgumentNullException.ThrowIfNull((object) nameof (fields), "nameof(fields)");
      this._fields = (IDictionary<string, FormFieldHtmlModel>) ((IEnumerable<FormFieldHtmlModel>) fields).ToDictionary<FormFieldHtmlModel, string>((Func<FormFieldHtmlModel, string>) (x => x.Alias));
      this.Fields = (IEnumerable<FormFieldHtmlModel>) fields;
      this.DynamicFields = this.ToDynamic(this._fields);
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

    public Dictionary<Guid, Dictionary<string, string?>> PrevalueMaps { get; set; } = new Dictionary<Guid, Dictionary<string, string>>();

    public IDictionary<string, string> WorkflowSettings { get; set; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public object? GetValue(string alias)
    {
      FormFieldHtmlModel formFieldHtmlModel;
      return this._fields.TryGetValue(alias, out formFieldHtmlModel) && formFieldHtmlModel != null && formFieldHtmlModel.FieldValue.Length != 0 ? formFieldHtmlModel.FieldValue[0] : (object) null;
    }

    public object[]? GetValues(string alias)
    {
      FormFieldHtmlModel formFieldHtmlModel;
      return this._fields.TryGetValue(alias, out formFieldHtmlModel) && formFieldHtmlModel != null && formFieldHtmlModel.FieldValue.Length != 0 ? formFieldHtmlModel.FieldValue : (object[]) null;
    }

    private object ToDynamic(IDictionary<string, FormFieldHtmlModel> fields)
    {
      ExpandoObject dynamic = new ExpandoObject();
      ICollection<KeyValuePair<string, object>> keyValuePairs = (ICollection<KeyValuePair<string, object>>) dynamic;
      foreach (KeyValuePair<string, FormFieldHtmlModel> field in (IEnumerable<KeyValuePair<string, FormFieldHtmlModel>>) fields)
      {
        object obj = (object) null;
        if (field.Value != null && field.Value.FieldValue != null)
          obj = field.Value.FieldValue.Length != 1 ? (object) field.Value.FieldValue : field.Value.FieldValue[0];
        keyValuePairs.Add(new KeyValuePair<string, object>(field.Key, obj));
      }
      return (object) dynamic;
    }
  }
}
