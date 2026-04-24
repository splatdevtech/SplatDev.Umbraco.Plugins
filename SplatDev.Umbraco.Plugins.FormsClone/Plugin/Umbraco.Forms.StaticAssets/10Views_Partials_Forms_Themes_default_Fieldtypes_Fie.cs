
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_PasswordField
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
  [RazorSourceChecksum("Sha256", "dc4e9fae7c6b318190d6f030e482c395bc766a65fb566181e7e4acb37252e203", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.PasswordField.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.PasswordField.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_PasswordField : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_PasswordField typePasswordField = this;
      typePasswordField.WriteLiteral("\n<input type=\"password\"");
      typePasswordField.BeginWriteAttribute("name", " name=\"", 95, "\"", 113, 1);
      typePasswordField.WriteAttributeValue("", 102, (object) typePasswordField.Model.Name, 102, 11, false);
      typePasswordField.EndWriteAttribute();
      typePasswordField.BeginWriteAttribute("id", " id=\"", 114, "\"", 128, 1);
      typePasswordField.WriteAttributeValue("", 119, (object) typePasswordField.Model.Id, 119, 9, false);
      typePasswordField.EndWriteAttribute();
      typePasswordField.WriteLiteral(" data-umb=\"");
      typePasswordField.Write(typePasswordField.Model.Id);
      typePasswordField.WriteLiteral("\" spellcheck=\"false\"");
      typePasswordField.BeginWriteAttribute("class", " class=\"", 169, "\"", 226, 2);
      typePasswordField.WriteAttributeValue("", 177, (object) typePasswordField.Html.GetFormFieldClass(typePasswordField.Model.FieldTypeName), 177, 44, false);
      typePasswordField.WriteAttributeValue(" ", 221, (object) "text", 222, 5, true);
      typePasswordField.EndWriteAttribute();
      typePasswordField.BeginWriteAttribute("value", " value=\"", 227, "\"", 259, 1);
      typePasswordField.WriteAttributeValue("", 235, (object) typePasswordField.Model.ValueAsHtmlString, 235, 24, false);
      typePasswordField.EndWriteAttribute();
      typePasswordField.WriteLiteral("\n");
      if (!string.IsNullOrEmpty(typePasswordField.Model.PlaceholderText))
      {
        typePasswordField.WriteLiteral("placeholder=\"");
        typePasswordField.Write(typePasswordField.Model.PlaceholderText);
        typePasswordField.WriteLiteral("\"");
      }
      typePasswordField.WriteLiteral("\n");
      if (typePasswordField.Model.Mandatory || typePasswordField.Model.Validate)
        typePasswordField.WriteLiteral("data-val=\"true\"");
      typePasswordField.WriteLiteral("\n");
      if (typePasswordField.Model.Mandatory)
      {
        typePasswordField.WriteLiteral("data-val-required=\"");
        typePasswordField.Write(typePasswordField.Model.RequiredErrorMessage);
        typePasswordField.WriteLiteral("\" aria-required=\"true\"");
      }
      typePasswordField.WriteLiteral("\n");
      if (typePasswordField.Model.Validate)
      {
        typePasswordField.WriteLiteral("data-val-regex=\"");
        typePasswordField.Write(typePasswordField.Model.InvalidErrorMessage);
        typePasswordField.WriteLiteral("\" data-val-regex-pattern=\"");
        typePasswordField.Write(typePasswordField.Model.Regex);
        typePasswordField.WriteLiteral("\"");
      }
      typePasswordField.WriteLiteral("\n");
      if (!string.IsNullOrEmpty(typePasswordField.Model.ToolTip))
      {
        typePasswordField.WriteLiteral("aria-describedby=\"");
        typePasswordField.Write(typePasswordField.Model.Id);
        typePasswordField.WriteLiteral("_description\" ");
      }
      typePasswordField.WriteLiteral("\n/>");
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
