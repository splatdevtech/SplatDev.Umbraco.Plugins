
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_HiddenField
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
using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
  [RazorSourceChecksum("Sha256", "49d02a827fe4d0feada0991a5d458290724ccca463e961f83395af4e750da7f9", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.HiddenField.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.HiddenField.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_HiddenField : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_HiddenField fieldTypeHiddenField = this;
      fieldTypeHiddenField.WriteLiteral("<input type=\"hidden\"");
      fieldTypeHiddenField.BeginWriteAttribute("name", " name=\"", 67, "\"", 85, 1);
      fieldTypeHiddenField.WriteAttributeValue("", 74, (object) fieldTypeHiddenField.Model.Name, 74, 11, false);
      fieldTypeHiddenField.EndWriteAttribute();
      fieldTypeHiddenField.BeginWriteAttribute("id", " id=\"", 86, "\"", 100, 1);
      fieldTypeHiddenField.WriteAttributeValue("", 91, (object) fieldTypeHiddenField.Model.Id, 91, 9, false);
      fieldTypeHiddenField.EndWriteAttribute();
      fieldTypeHiddenField.WriteLiteral(" data-umb=\"");
      fieldTypeHiddenField.Write(fieldTypeHiddenField.Model.Id);
      fieldTypeHiddenField.WriteLiteral("\" class=\"hidden\"");
      fieldTypeHiddenField.BeginWriteAttribute("value", " value=\"", 137, "\"", 169, 1);
      fieldTypeHiddenField.WriteAttributeValue("", 145, (object) fieldTypeHiddenField.Model.ValueAsHtmlString, 145, 24, false);
      fieldTypeHiddenField.EndWriteAttribute();
      fieldTypeHiddenField.WriteLiteral("/>\n\n\n");
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
