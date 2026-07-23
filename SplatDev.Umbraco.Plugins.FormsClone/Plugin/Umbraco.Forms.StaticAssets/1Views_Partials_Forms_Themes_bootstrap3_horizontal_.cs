
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_bootstrap3_horizontal_Fieldtypes_FieldType_RadioButtonList
// Assembly: Umbraco.Forms.StaticAssets, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DDF8130F-541E-4CFE-9C57-74C2C790186A

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbraco.Forms.Web;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
  [RazorSourceChecksum("Sha256", "fc67909754b7839ccdd0a23db7c809be75cdbc3ece1776cd87e6859e4860a1a4", "/Views/Partials/Forms/Themes/bootstrap3-horizontal/Fieldtypes/FieldType.RadioButtonList.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/bootstrap3-horizontal/Fieldtypes/FieldType.RadioButtonList.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_bootstrap3_horizontal_Fieldtypes_FieldType_RadioButtonList : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_bootstrap3_horizontal_Fieldtypes_FieldType_RadioButtonList typeRadioButtonList = this;
      typeRadioButtonList.WriteLiteral("  \n");
      int num = 0;
      string lower = (typeRadioButtonList.Model.GetSettingValue<string>("DisplayLayout", "Vertical") ?? string.Empty).ToLower();
      typeRadioButtonList.WriteLiteral("\n<div class=\"radiobuttonlist\"");
      typeRadioButtonList.BeginWriteAttribute("id", " id=\"", 238, "\"", 252, 1);
      typeRadioButtonList.WriteAttributeValue("", 243, (object) typeRadioButtonList.Model.Id, 243, 9, false);
      typeRadioButtonList.EndWriteAttribute();
      typeRadioButtonList.WriteLiteral(" data-umb=\"");
      typeRadioButtonList.Write(typeRadioButtonList.Model.Id);
      typeRadioButtonList.WriteLiteral("\">\n");
      foreach (PrevalueViewModel preValue in typeRadioButtonList.Model.PreValues)
      {
        typeRadioButtonList.WriteLiteral("        <div");
        typeRadioButtonList.BeginWriteAttribute("class", " class=\"", 376, "\"", 465, 2);
        typeRadioButtonList.WriteAttributeValue("", 384, (object) "form-check", 384, 10, true);
        typeRadioButtonList.WriteAttributeValue("", 394, lower == "horizontal" ? (object) " form-check-inline" : (object) string.Empty, 394, 71, false);
        typeRadioButtonList.EndWriteAttribute();
        typeRadioButtonList.WriteLiteral(">\n            <input type=\"radio\"\n                   class=\"form-check-input\"");
        typeRadioButtonList.BeginWriteAttribute("name", "\n                   name=\"", 543, "\"", 578, 1);
        typeRadioButtonList.WriteAttributeValue("", 569, (object) typeRadioButtonList.Model.Id, 569, 9, false);
        typeRadioButtonList.EndWriteAttribute();
        typeRadioButtonList.BeginWriteAttribute("id", "\n                   id=\"", 579, "\"", 633, 1);
        typeRadioButtonList.WriteAttributeValue("", 603, (object) (typeRadioButtonList.Model.Id + "_" + (object) num), 603, 30, false);
        typeRadioButtonList.EndWriteAttribute();
        typeRadioButtonList.WriteLiteral("\n                   data-umb=\"");
        typeRadioButtonList.Write(typeRadioButtonList.Model.Id + "_" + (object) num);
        typeRadioButtonList.WriteLiteral("\"");
        typeRadioButtonList.BeginWriteAttribute("value", "\n                   value=\"", 697, "\"", 733, 1);
        typeRadioButtonList.WriteAttributeValue("", 724, (object) preValue.Value, 724, 9, false);
        typeRadioButtonList.EndWriteAttribute();
        typeRadioButtonList.WriteLiteral("\n            ");
        if (typeRadioButtonList.Model.Mandatory)
        {
          typeRadioButtonList.WriteLiteral("data-val=\"true\" data-val-required=\"");
          typeRadioButtonList.Write(typeRadioButtonList.Model.RequiredErrorMessage);
          typeRadioButtonList.WriteLiteral("\" data-rule-required=\"true\" data-msg-required=\"");
          typeRadioButtonList.Write(typeRadioButtonList.Model.RequiredErrorMessage);
          typeRadioButtonList.WriteLiteral("\"");
        }
        typeRadioButtonList.WriteLiteral("            ");
        if (typeRadioButtonList.Model.ContainsValue((object) preValue.Value))
          typeRadioButtonList.WriteLiteral("checked=\"checked\"");
        typeRadioButtonList.WriteLiteral(" />\n\n            <label class=\"form-check-label\">");
        typeRadioButtonList.Write(preValue.Caption);
        typeRadioButtonList.WriteLiteral("</label>    \n\t    </div>\n");
        ++num;
      }
      typeRadioButtonList.WriteLiteral("</div>\n");
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
