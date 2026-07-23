
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBoxList
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
  [RazorSourceChecksum("Sha256", "8d1115e65426a83c2dc4320810ef39bca3bc85d6379995dec873e5ee308c983f", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBoxList.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.CheckBoxList.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBoxList : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_CheckBoxList typeCheckBoxList = this;
      typeCheckBoxList.WriteLiteral("\n");
      int num = 0;
      string lower = (typeCheckBoxList.Model.GetSettingValue<string>("DisplayLayout", "Vertical") ?? string.Empty).ToLower();
      typeCheckBoxList.WriteLiteral("          \n<div");
      typeCheckBoxList.BeginWriteAttribute("class", " class=\"", 220, "\"", 268, 3);
      typeCheckBoxList.WriteAttributeValue("", 228, (object) "checkboxlist", 228, 12, true);
      typeCheckBoxList.WriteAttributeValue(" ", 240, (object) "checkboxlist-", 241, 14, true);
      typeCheckBoxList.WriteAttributeValue("", 254, (object) lower, 254, 14, false);
      typeCheckBoxList.EndWriteAttribute();
      typeCheckBoxList.BeginWriteAttribute("id", " id=\"", 269, "\"", 283, 1);
      typeCheckBoxList.WriteAttributeValue("", 274, (object) typeCheckBoxList.Model.Id, 274, 9, false);
      typeCheckBoxList.EndWriteAttribute();
      typeCheckBoxList.WriteLiteral(" data-umb=\"");
      typeCheckBoxList.Write(typeCheckBoxList.Model.Id);
      typeCheckBoxList.WriteLiteral("\">\n");
      foreach (PrevalueViewModel preValue in typeCheckBoxList.Model.PreValues)
      {
        typeCheckBoxList.WriteLiteral("        <div>\n            <input type=\"checkbox\"");
        typeCheckBoxList.BeginWriteAttribute("class", "\n                   class=\"", 441, "\"", 512, 1);
        typeCheckBoxList.WriteAttributeValue("", 468, (object) typeCheckBoxList.Html.GetFormFieldClass(typeCheckBoxList.Model.FieldTypeName), 468, 44, false);
        typeCheckBoxList.EndWriteAttribute();
        typeCheckBoxList.BeginWriteAttribute("name", "\n                   name=\"", 513, "\"", 550, 1);
        typeCheckBoxList.WriteAttributeValue("", 539, (object) typeCheckBoxList.Model.Name, 539, 11, false);
        typeCheckBoxList.EndWriteAttribute();
        typeCheckBoxList.BeginWriteAttribute("id", " id=\"", 551, "\"", 586, 1);
        typeCheckBoxList.WriteAttributeValue("", 556, (object) (typeCheckBoxList.Model.Id + "_" + (object) num), 556, 30, false);
        typeCheckBoxList.EndWriteAttribute();
        typeCheckBoxList.WriteLiteral(" data-umb=\"");
        typeCheckBoxList.Write(typeCheckBoxList.Model.Id + "_" + (object) num);
        typeCheckBoxList.WriteLiteral("\"");
        typeCheckBoxList.BeginWriteAttribute("value", " value=\"", 631, "\"", 648, 1);
        typeCheckBoxList.WriteAttributeValue("", 639, (object) preValue.Value, 639, 9, false);
        typeCheckBoxList.EndWriteAttribute();
        typeCheckBoxList.WriteLiteral("\n            ");
        if (typeCheckBoxList.Model.Mandatory)
        {
          typeCheckBoxList.WriteLiteral("data-val=\"true\" data-val-required=\"");
          typeCheckBoxList.Write(typeCheckBoxList.Model.RequiredErrorMessage);
          typeCheckBoxList.WriteLiteral("\" data-rule-required=\"true\" data-msg-required=\"");
          typeCheckBoxList.Write(typeCheckBoxList.Model.RequiredErrorMessage);
          typeCheckBoxList.WriteLiteral("\"");
        }
        typeCheckBoxList.WriteLiteral("            ");
        if (typeCheckBoxList.Model.ContainsValue((object) preValue.Value))
          typeCheckBoxList.WriteLiteral("checked=\"checked\"");
        typeCheckBoxList.WriteLiteral(" />\n\n            <label");
        typeCheckBoxList.BeginWriteAttribute("for", " for=\"", 1024, "\"", 1060, 1);
        typeCheckBoxList.WriteAttributeValue("", 1030, (object) (typeCheckBoxList.Model.Id + "_" + (object) num), 1030, 30, false);
        typeCheckBoxList.EndWriteAttribute();
        typeCheckBoxList.WriteLiteral(">");
        typeCheckBoxList.Write(preValue.Caption);
        typeCheckBoxList.WriteLiteral("</label>\n        </div>\n");
        ++num;
      }
      typeCheckBoxList.WriteLiteral("</div>\n");
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
