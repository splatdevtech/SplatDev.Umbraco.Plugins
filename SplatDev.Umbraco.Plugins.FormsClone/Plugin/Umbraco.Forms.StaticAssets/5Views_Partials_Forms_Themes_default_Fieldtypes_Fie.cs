
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DatePicker
// Assembly: Umbraco.Forms.StaticAssets, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DDF8130F-541E-4CFE-9C57-74C2C790186A

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
  [RazorSourceChecksum("Sha256", "93c781c3dba69a82d60cbd26d22cbad5ce654229ea4bfaa820f3c1b0be752398", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DatePicker.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DatePicker.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DatePicker : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DatePicker fieldTypeDatePicker = this;
      fieldTypeDatePicker.WriteLiteral("\n");
      fieldTypeDatePicker.WriteLiteral("\n");
      string text = fieldTypeDatePicker.Model.GetSettingValue<string>("AriaLabel", string.Empty);
      if (!string.IsNullOrWhiteSpace(text))
        text = text.ParsePlaceHolders(fieldTypeDatePicker.PlaceholderParsingService, false);
      IEnumerable<object> values = fieldTypeDatePicker.Model.Values;
      string str = (values != null ? values.LastOrDefault<object>()?.ToString() : (string) null) ?? string.Empty;
      if (fieldTypeDatePicker.Model.ValueAsObject != null)
      {
        if (!object.Equals(fieldTypeDatePicker.Model.ValueAsObject, (object) string.Empty))
        {
          try
          {
            str = ((DateTime) fieldTypeDatePicker.Model.ValueAsObject).ToShortDateString();
          }
          catch
          {
          }
        }
      }
      fieldTypeDatePicker.WriteLiteral("\n<input type=\"hidden\"");
      fieldTypeDatePicker.BeginWriteAttribute("name", " name=\"", 817, "\"", 835, 1);
      fieldTypeDatePicker.WriteAttributeValue("", 824, (object) fieldTypeDatePicker.Model.Name, 824, 11, false);
      fieldTypeDatePicker.EndWriteAttribute();
      fieldTypeDatePicker.BeginWriteAttribute("id", " id=\"", 836, "\"", 854, 2);
      fieldTypeDatePicker.WriteAttributeValue("", 841, (object) fieldTypeDatePicker.Model.Id, 841, 11, false);
      fieldTypeDatePicker.WriteAttributeValue("", 852, (object) "_1", 852, 2, true);
      fieldTypeDatePicker.EndWriteAttribute();
      fieldTypeDatePicker.WriteLiteral(" class=\"datepickerfieldshadow\"");
      fieldTypeDatePicker.BeginWriteAttribute("value", " value=\"", 885, "\"", 897, 1);
      fieldTypeDatePicker.WriteAttributeValue("", 893, (object) str, 893, 4, false);
      fieldTypeDatePicker.EndWriteAttribute();
      fieldTypeDatePicker.WriteLiteral(" data-umb=\"");
      fieldTypeDatePicker.Write(fieldTypeDatePicker.Model.Id);
      fieldTypeDatePicker.WriteLiteral("\"  />\n<input type=\"text\"");
      fieldTypeDatePicker.BeginWriteAttribute("name", "\n       name=\"", 942, "\"", 967, 1);
      fieldTypeDatePicker.WriteAttributeValue("", 956, (object) fieldTypeDatePicker.Model.Name, 956, 11, false);
      fieldTypeDatePicker.EndWriteAttribute();
      fieldTypeDatePicker.BeginWriteAttribute("id", "\n       id=\"", 968, "\"", 991, 1);
      fieldTypeDatePicker.WriteAttributeValue("", 980, (object) fieldTypeDatePicker.Model.Id, 980, 11, false);
      fieldTypeDatePicker.EndWriteAttribute();
      fieldTypeDatePicker.WriteLiteral("\n       class=\"datepickerfield\"\n       autocomplete=\"off\"");
      fieldTypeDatePicker.BeginWriteAttribute("value", "\n       value=\"", 1049, "\"", 1068, 1);
      fieldTypeDatePicker.WriteAttributeValue("", 1064, (object) str, 1064, 4, false);
      fieldTypeDatePicker.EndWriteAttribute();
      fieldTypeDatePicker.WriteLiteral("\n       ");
      if (fieldTypeDatePicker.Model.Mandatory)
      {
        fieldTypeDatePicker.WriteLiteral(" data-val=\"true\" data-val-required=\"");
        fieldTypeDatePicker.Write(fieldTypeDatePicker.Model.RequiredErrorMessage);
        fieldTypeDatePicker.WriteLiteral("\" ");
      }
      fieldTypeDatePicker.WriteLiteral("\n       ");
      if (!string.IsNullOrWhiteSpace(fieldTypeDatePicker.Model.PlaceholderText))
      {
        fieldTypeDatePicker.WriteLiteral("placeholder=\"");
        fieldTypeDatePicker.Write(fieldTypeDatePicker.Model.PlaceholderText);
        fieldTypeDatePicker.WriteLiteral("\"");
      }
      fieldTypeDatePicker.WriteLiteral("\n       ");
      if (!string.IsNullOrWhiteSpace(text))
      {
        fieldTypeDatePicker.WriteLiteral("aria-label=\"");
        fieldTypeDatePicker.Write(text);
        fieldTypeDatePicker.WriteLiteral("\"");
      }
      fieldTypeDatePicker.WriteLiteral("\n       ");
      if (!string.IsNullOrWhiteSpace(fieldTypeDatePicker.Model.ToolTip))
      {
        fieldTypeDatePicker.WriteLiteral("aria-describedby=\"");
        fieldTypeDatePicker.Write(fieldTypeDatePicker.Model.Id);
        fieldTypeDatePicker.WriteLiteral("_description\" ");
      }
      fieldTypeDatePicker.WriteLiteral("/>\n");
    }

    [RazorInject]
    public 
    #nullable enable
    IPlaceholderParsingService PlaceholderParsingService { get; private set; }

    [RazorInject]
    public IModelExpressionProvider ModelExpressionProvider { get; private set; }

    [RazorInject]
    public IUrlHelper Url { get; private set; }

    [RazorInject]
    public IViewComponentHelper Component { get; private set; }

    [RazorInject]
    public IJsonHelper Json { get; private set; }

    [RazorInject]
    public IHtmlHelper<FieldViewModel> Html { get; private set; }
  }
}
