
// Type: Umbraco.Forms.Web.Models.FieldViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class FieldViewModel : ISupportFileUploads
  {
    public string Id { get; set; } = string.Empty;

    public Guid FieldsetId { get; set; }

    public Guid PageId { get; set; }

    public Guid FormId { get; set; }

    public string Alias { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Caption { get; set; } = string.Empty;

    public bool Mandatory { get; set; }

    public string RequiredErrorMessage { get; set; } = string.Empty;

    public bool Validate { get; set; }

    public string Regex { get; set; } = string.Empty;

    public string InvalidErrorMessage { get; set; } = string.Empty;

    public string FieldTypeName { get; set; } = string.Empty;

    public FieldType? FieldType { get; set; }

    public bool HideLabel { get; set; }

    public bool ShowIndicator { get; set; }

    public string ToolTip { get; set; } = string.Empty;

    public string CssClass { get; set; } = string.Empty;

    public IEnumerable<PrevalueViewModel> PreValues { get; set; } = Enumerable.Empty<PrevalueViewModel>();

    public bool AllowAllUploadExtensions { get; set; }

    public IEnumerable<string> AllowedUploadExtensions { get; set; } = Enumerable.Empty<string>();

    public bool AllowMultipleFileUploads { get; set; }

    public IDictionary<string, string> AdditionalSettings { get; set; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public bool HasCondition { get; set; }

    public string PlaceholderText { get; set; } = string.Empty;

    public FieldConditionActionType ConditionActionType { get; set; }

    public FieldConditionLogicType ConditionLogicType { get; set; }

    public IEnumerable<FieldConditionRule> ConditionRules { get; set; } = Enumerable.Empty<FieldConditionRule>();

    public IEnumerable<FieldCondition> ParentConditions { get; set; } = Enumerable.Empty<FieldCondition>();

    public FieldCondition? Condition { get; set; }

    public IEnumerable<object> Values { get; set; } = Enumerable.Empty<object>();

    public object? ValueAsObject
    {
      get
      {
        object valueAsObject = (object) null;
        if (this.Values != null && this.Values.Any<object>())
          valueAsObject = this.Values.First<object>();
        return valueAsObject;
      }
    }

    public HtmlString ValueAsHtmlString
    {
      get
      {
        string str = string.Empty;
        object obj = this.Values.FirstOrDefault<object>();
        if (obj != null)
          str = HttpUtility.HtmlAttributeEncode(obj.ToString()) ?? string.Empty;
        return new HtmlString(str);
      }
    }

    public bool ContainsValue(object value) => this.Values != null && this.Values.Any<object>() && ((IEnumerable<object>) this.Values.Select<object, string>((Func<object, string>) (x => x.ToString()))).Contains<object>(value);
  }
}
