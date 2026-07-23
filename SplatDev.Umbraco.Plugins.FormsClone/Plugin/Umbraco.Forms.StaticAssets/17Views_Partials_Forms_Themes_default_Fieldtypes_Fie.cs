
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Textarea
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
  [RazorSourceChecksum("Sha256", "1c2a1f1134b9ddff02b8099684997964feb1f17bb6dd508c35a79e3c50bcc9eb", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Textarea.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Textarea.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Textarea : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Textarea fieldTypeTextarea = this;
      fieldTypeTextarea.WriteLiteral("\n");
      string settingValue1 = fieldTypeTextarea.Model.GetSettingValue<string>("AutocompleteAttribute", string.Empty);
      int settingValue2 = fieldTypeTextarea.Model.GetSettingValue<int>("NumberOfRows", 2);
      int settingValue3 = fieldTypeTextarea.Model.GetSettingValue<int>("MaximumLength");
      fieldTypeTextarea.WriteLiteral("<textarea");
      fieldTypeTextarea.BeginWriteAttribute("class", " class=\"", 402, "\"", 454, 1);
      fieldTypeTextarea.WriteAttributeValue("", 410, (object) fieldTypeTextarea.Html.GetFormFieldClass(fieldTypeTextarea.Model.FieldTypeName), 410, 44, false);
      fieldTypeTextarea.EndWriteAttribute();
      fieldTypeTextarea.BeginWriteAttribute("name", "\n          name=\"", 455, "\"", 483, 1);
      fieldTypeTextarea.WriteAttributeValue("", 472, (object) fieldTypeTextarea.Model.Name, 472, 11, false);
      fieldTypeTextarea.EndWriteAttribute();
      fieldTypeTextarea.BeginWriteAttribute("id", "\n          id=\"", 484, "\"", 508, 1);
      fieldTypeTextarea.WriteAttributeValue("", 499, (object) fieldTypeTextarea.Model.Id, 499, 9, false);
      fieldTypeTextarea.EndWriteAttribute();
      fieldTypeTextarea.WriteLiteral("\n          data-umb=\"");
      fieldTypeTextarea.Write(fieldTypeTextarea.Model.Id);
      fieldTypeTextarea.WriteLiteral("\"");
      fieldTypeTextarea.BeginWriteAttribute("rows", "\n          rows=\"", 540, "\"", 570, 1);
      fieldTypeTextarea.WriteAttributeValue("", 557, (object) settingValue2, 557, 13, false);
      fieldTypeTextarea.EndWriteAttribute();
      fieldTypeTextarea.WriteLiteral("\n          cols=\"20\"\n          ");
      if (!string.IsNullOrEmpty(fieldTypeTextarea.Model.PlaceholderText))
      {
        fieldTypeTextarea.WriteLiteral(" placeholder=\"");
        fieldTypeTextarea.Write(fieldTypeTextarea.Model.PlaceholderText);
        fieldTypeTextarea.WriteLiteral("\" ");
      }
      fieldTypeTextarea.WriteLiteral("\n          ");
      if (!string.IsNullOrEmpty(settingValue1))
      {
        fieldTypeTextarea.WriteLiteral("autocomplete=\"");
        fieldTypeTextarea.Write(settingValue1);
        fieldTypeTextarea.WriteLiteral("\" ");
      }
      fieldTypeTextarea.WriteLiteral("\n          ");
      if (settingValue3 > 0)
      {
        fieldTypeTextarea.WriteLiteral("maxlength=\"");
        fieldTypeTextarea.Write((object) settingValue3);
        fieldTypeTextarea.WriteLiteral("\" ");
      }
      fieldTypeTextarea.WriteLiteral("\n          ");
      if (fieldTypeTextarea.Model.Mandatory || fieldTypeTextarea.Model.Validate)
        fieldTypeTextarea.WriteLiteral("data-val=\"true\" ");
      fieldTypeTextarea.WriteLiteral("\n          ");
      if (fieldTypeTextarea.Model.Mandatory)
      {
        fieldTypeTextarea.WriteLiteral("data-val-required=\"");
        fieldTypeTextarea.Write(fieldTypeTextarea.Model.RequiredErrorMessage);
        fieldTypeTextarea.WriteLiteral("\" aria-required=\"true\" ");
      }
      fieldTypeTextarea.WriteLiteral("\n          ");
      if (fieldTypeTextarea.Model.Validate)
      {
        fieldTypeTextarea.WriteLiteral("data-val-regex=\"");
        fieldTypeTextarea.Write(fieldTypeTextarea.Model.InvalidErrorMessage);
        fieldTypeTextarea.WriteLiteral("\" data-val-regex-pattern=\"");
        fieldTypeTextarea.Write((object) fieldTypeTextarea.Html.Raw(fieldTypeTextarea.Model.Regex));
        fieldTypeTextarea.WriteLiteral("\" ");
      }
      fieldTypeTextarea.WriteLiteral("\n          ");
      if (!string.IsNullOrEmpty(fieldTypeTextarea.Model.ToolTip))
      {
        fieldTypeTextarea.WriteLiteral("aria-describedby=\"");
        fieldTypeTextarea.Write(fieldTypeTextarea.Model.Id);
        fieldTypeTextarea.WriteLiteral("_description\" ");
      }
      fieldTypeTextarea.WriteLiteral(">");
      fieldTypeTextarea.Write((object) fieldTypeTextarea.Model.ValueAsHtmlString);
      fieldTypeTextarea.WriteLiteral("</textarea>\n\n");
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
