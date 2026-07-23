
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DropDownList
// Assembly: Umbraco.Forms.StaticAssets, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DDF8130F-541E-4CFE-9C57-74C2C790186A

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbraco.Forms.Web;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
  [RazorSourceChecksum("Sha256", "b882bf388dea13d486a96d3d29fa91279cd7e675eba6bc4d37ca08712c514bf4", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DropDownList.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DropDownList.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DropDownList : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DropDownList typeDropDownList = this;
      IDictionary<string, string> additionalSettings = typeDropDownList.Model.AdditionalSettings;
      string settingValue1 = typeDropDownList.Model.GetSettingValue<string>("AutocompleteAttribute", string.Empty);
      int num = !additionalSettings.ContainsKey("AllowMultipleSelections") ? 0 : (additionalSettings["AllowMultipleSelections"].ToLower() == "true" ? 1 : 0);
      string settingValue2 = typeDropDownList.Model.GetSettingValue<string>("SelectPrompt", string.Empty);
      typeDropDownList.WriteLiteral("\n<select");
      typeDropDownList.BeginWriteAttribute("class", " class=\"", 448, "\"", 500, 1);
      typeDropDownList.WriteAttributeValue("", 456, (object) typeDropDownList.Html.GetFormFieldClass(typeDropDownList.Model.FieldTypeName), 456, 44, false);
      typeDropDownList.EndWriteAttribute();
      typeDropDownList.BeginWriteAttribute("name", "\n        name=\"", 501, "\"", 527, 1);
      typeDropDownList.WriteAttributeValue("", 516, (object) typeDropDownList.Model.Name, 516, 11, false);
      typeDropDownList.EndWriteAttribute();
      typeDropDownList.BeginWriteAttribute("id", "\n        id=\"", 528, "\"", 550, 1);
      typeDropDownList.WriteAttributeValue("", 541, (object) typeDropDownList.Model.Id, 541, 9, false);
      typeDropDownList.EndWriteAttribute();
      typeDropDownList.WriteLiteral("\n        data-umb=\"");
      typeDropDownList.Write(typeDropDownList.Model.Id);
      typeDropDownList.WriteLiteral("\"\n        ");
      if (!string.IsNullOrEmpty(settingValue1))
      {
        typeDropDownList.WriteLiteral("autocomplete=\"");
        typeDropDownList.Write(settingValue1);
        typeDropDownList.WriteLiteral("\"");
      }
      typeDropDownList.WriteLiteral("        ");
      if (num != 0)
        typeDropDownList.WriteLiteral(" multiple ");
      typeDropDownList.WriteLiteral("        ");
      if (typeDropDownList.Model.Mandatory)
      {
        typeDropDownList.WriteLiteral(" data-val=\"true\" data-val-required=\"");
        typeDropDownList.Write(typeDropDownList.Model.RequiredErrorMessage);
        typeDropDownList.WriteLiteral("\" aria-required=\"true\" ");
      }
      typeDropDownList.WriteLiteral("        ");
      if (!string.IsNullOrEmpty(typeDropDownList.Model.ToolTip))
      {
        typeDropDownList.WriteLiteral(" aria-describedby=\"");
        typeDropDownList.Write(typeDropDownList.Model.Id);
        typeDropDownList.WriteLiteral("_description\" ");
      }
      typeDropDownList.WriteLiteral(">\n");
      if (num == 0)
      {
        typeDropDownList.WriteLiteral("        <option");
        typeDropDownList.BeginWriteAttribute("value", " value=\"", 1053, "\"", 1061, 0);
        typeDropDownList.EndWriteAttribute();
        typeDropDownList.WriteLiteral(">");
        typeDropDownList.Write(settingValue2);
        typeDropDownList.WriteLiteral("</option>\n");
      }
      foreach (PrevalueViewModel preValue in typeDropDownList.Model.PreValues)
      {
        typeDropDownList.WriteLiteral("        ");
        typeDropDownList.WriteLiteral(" <option value=\"");
        typeDropDownList.Write(preValue.Value);
        typeDropDownList.WriteLiteral("\" ");
        typeDropDownList.Write(typeDropDownList.Model.ContainsValue((object) preValue.Value) ? "selected" : string.Empty);
        typeDropDownList.WriteLiteral(">");
        typeDropDownList.Write(preValue.Caption);
        typeDropDownList.WriteLiteral("</option>\n");
      }
      typeDropDownList.WriteLiteral("</select>\n");
    }

    [RazorInject]
    public 
    #nullable enable
    IModelExpressionProvider ModelExpressionProvider { get; private set; }

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
