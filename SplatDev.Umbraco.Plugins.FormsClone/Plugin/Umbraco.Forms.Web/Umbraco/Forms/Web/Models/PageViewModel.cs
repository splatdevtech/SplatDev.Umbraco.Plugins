
// Type: Umbraco.Forms.Web.Models.PageViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class PageViewModel
  {
    public string Caption { get; set; } = string.Empty;

    public IList<FieldsetViewModel> Fieldsets { get; set; } = (IList<FieldsetViewModel>) new List<FieldsetViewModel>();

    public Guid Id { get; set; }

    public bool HasButtonCondition { get; set; }

    public FieldConditionActionType ButtonConditionActionType { get; set; }

    public FieldConditionLogicType ButtonConditionLogicType { get; set; }

    public IEnumerable<FieldConditionRule> ButtonConditionRules { get; set; } = Enumerable.Empty<FieldConditionRule>();

    public FieldCondition? ButtonCondition { get; set; }

    public Dictionary<string, string> JavascriptFiles { get; set; } = new Dictionary<string, string>();

    public Dictionary<string, string> CssFiles { get; set; } = new Dictionary<string, string>();

    public List<string> JavascriptCommands { get; set; } = new List<string>();

    public Dictionary<string, string> PartialViewFiles { get; set; } = new Dictionary<string, string>();

    public void RegisterFieldJavascriptAssets(
      Field field,
      FieldType type,
      IHostingEnvironment hostingEnvironment)
    {
      this.RegisterFieldJavascriptAssets((Func<string>) (() => string.Empty), field, type, hostingEnvironment);
    }

    public void RegisterFieldJavascriptAssets(
      Func<string?> themeAccessor,
      Field field,
      FieldType type,
      IHostingEnvironment hostingEnvironment)
    {
      string[] array1 = type.RequiredJavascriptFiles(field).ToArray<string>();
      string[] array2 = type.RequiredCssFiles(field).ToArray<string>();
      string[] array3 = type.RequiredPartialViews(themeAccessor, field).Union<string>(type.RequiredPartialViews(field)).ToArray<string>();
      if (((IEnumerable<string>) array3).Any<string>())
      {
        foreach (string str in array3)
        {
          string key = str.ToLower().Replace("/", string.Empty).Replace(".", string.Empty);
          if (!this.PartialViewFiles.ContainsKey(key))
            this.PartialViewFiles.Add(key, str);
        }
      }
      if (((IEnumerable<string>) array1).Any<string>())
      {
        foreach (string str in array1)
        {
          string key = str.ToLower().Replace("/", string.Empty).Replace(".", string.Empty);
          if (!this.JavascriptFiles.ContainsKey(key))
            this.JavascriptFiles.Add(key, str);
        }
      }
      if (((IEnumerable<string>) array2).Any<string>())
      {
        foreach (string virtualPath in array2)
        {
          string key = virtualPath.ToLower().Replace("/", string.Empty).Replace(".", string.Empty);
          if (!this.CssFiles.ContainsKey(key))
            this.CssFiles.Add(key, hostingEnvironment.ToAbsolute(virtualPath));
        }
      }
      string str1 = type.RequiredJavascriptInitialization(field);
      if (string.IsNullOrEmpty(str1) || this.JavascriptCommands.Contains(str1))
        return;
      this.JavascriptCommands.Add(str1);
    }
  }
}
