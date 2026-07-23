
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Text
// Assembly: Umbraco.Forms.StaticAssets, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DDF8130F-541E-4CFE-9C57-74C2C790186A

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
  [RazorSourceChecksum("Sha256", "a150360f49932c8d7c1061e3fe25fa1bcf02802df9867bfdc03185e4750ea2b5", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Text.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Text.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Text : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Text fieldtypesFieldTypeText = this;
      fieldtypesFieldTypeText.WriteLiteral("\n");
      fieldtypesFieldTypeText.WriteLiteral("\n");
      fieldtypesFieldTypeText.WriteLiteral("\n");
      IDictionary<string, string> additionalSettings = fieldtypesFieldTypeText.Model.AdditionalSettings;
      int num = !additionalSettings.ContainsKey("Caption") ? 0 : (!string.IsNullOrEmpty(additionalSettings["Caption"]) ? 1 : 0);
      bool flag = additionalSettings.ContainsKey("BodyText") && !string.IsNullOrEmpty(additionalSettings["BodyText"]);
      string settingValue = fieldtypesFieldTypeText.Model.GetSettingValue<string>("CaptionTag", "h2");
      fieldtypesFieldTypeText.WriteLiteral("\n<div");
      fieldtypesFieldTypeText.BeginWriteAttribute("id", " id=\"", 549, "\"", 563, 1);
      fieldtypesFieldTypeText.WriteAttributeValue("", 554, (object) fieldtypesFieldTypeText.Model.Id, 554, 9, false);
      fieldtypesFieldTypeText.EndWriteAttribute();
      fieldtypesFieldTypeText.WriteLiteral(" data-umb=\"");
      fieldtypesFieldTypeText.Write(fieldtypesFieldTypeText.Model.Id);
      fieldtypesFieldTypeText.WriteLiteral("\"");
      fieldtypesFieldTypeText.BeginWriteAttribute("class", " class=\"", 585, "\"", 637, 1);
      fieldtypesFieldTypeText.WriteAttributeValue("", 593, (object) fieldtypesFieldTypeText.Html.GetFormFieldClass(fieldtypesFieldTypeText.Model.FieldTypeName), 593, 44, false);
      fieldtypesFieldTypeText.EndWriteAttribute();
      fieldtypesFieldTypeText.WriteLiteral(">\n");
      if (num != 0)
      {
        fieldtypesFieldTypeText.Write((object) fieldtypesFieldTypeText.Html.Raw("<" + settingValue + ">"));
        fieldtypesFieldTypeText.Write(additionalSettings["Caption"]);
        fieldtypesFieldTypeText.Write((object) fieldtypesFieldTypeText.Html.Raw("</" + settingValue + ">"));
      }
      if (flag)
      {
        if (fieldtypesFieldTypeText.Configuration.Value.AllowUnsafeHtmlRendering)
        {
          fieldtypesFieldTypeText.WriteLiteral("            <p>");
          fieldtypesFieldTypeText.Write((object) fieldtypesFieldTypeText.Html.Raw(additionalSettings["BodyText"].Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "<br />")));
          fieldtypesFieldTypeText.WriteLiteral("</p>\n");
        }
        else
        {
          fieldtypesFieldTypeText.WriteLiteral("            <p>");
          fieldtypesFieldTypeText.Write(additionalSettings["BodyText"]);
          fieldtypesFieldTypeText.WriteLiteral("</p>\n");
        }
      }
      fieldtypesFieldTypeText.WriteLiteral("</div>\n");
    }

    [RazorInject]
    public 
    #nullable enable
    IOptionsSnapshot<TitleAndDescriptionSettings> Configuration { get; private set; }

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
