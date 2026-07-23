
// Type: Umbraco.Forms.Web.FormViewModelExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace Umbraco.Forms.Web
{
  public static class FormViewModelExtensions
  {
    public static string AllFieldsAsJson(this FormViewModel model)
    {
      Dictionary<string, string> dictionary = new Dictionary<string, string>();
      foreach (PageViewModel page in model.Pages)
      {
        foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>) page.Fieldsets)
        {
          foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>) fieldset.Containers)
          {
            foreach (FieldViewModel field in (IEnumerable<FieldViewModel>) container.Fields)
              dictionary.Add(field.Name, !string.IsNullOrWhiteSpace(HtmlContentExtensions.ToHtmlString((IHtmlContent) field.ValueAsHtmlString)) ? HtmlContentExtensions.ToHtmlString((IHtmlContent) field.ValueAsHtmlString) : string.Empty);
          }
        }
      }
      return JsonSerializer.Serialize<Dictionary<string, string>>(dictionary, FormsJsonSerializerOptions.Default);
    }

    public static string FieldsetConditionsAsJson(this FormViewModel model) => JsonSerializer.Serialize<Dictionary<Guid, ConditionViewModel>>(model.FieldsetConditions, FormsJsonSerializerOptions.Default);

    public static string FieldConditionsAsJson(this FormViewModel model) => JsonSerializer.Serialize<Dictionary<Guid, ConditionViewModel>>(model.FieldConditions, FormsJsonSerializerOptions.Default);
  }
}
