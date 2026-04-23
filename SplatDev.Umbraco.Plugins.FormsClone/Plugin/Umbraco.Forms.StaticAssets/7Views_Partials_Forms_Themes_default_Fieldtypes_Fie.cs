
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload
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
using Umbraco.Forms.Core.Providers.FieldTypes;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
  [RazorSourceChecksum("Sha256", "bb8504720b2726398276b3fd49273cac35d991f01024ab457334497d4e4bfd92", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload fieldTypeFileUpload = this;
      fieldTypeFileUpload.WriteLiteral("\n");
      bool flag = fieldTypeFileUpload.Model.Mandatory && fieldTypeFileUpload.Model.Values != null && fieldTypeFileUpload.Model.Values.Any<object>() || !fieldTypeFileUpload.Model.Mandatory;
      string str1;
      if (!fieldTypeFileUpload.Model.AdditionalSettings.TryGetValue("SelectedFilesListHeading", out str1) || string.IsNullOrWhiteSpace(str1))
        str1 = "Current file(s)";
      fieldTypeFileUpload.WriteLiteral("<input type=\"file\"");
      fieldTypeFileUpload.BeginWriteAttribute("name", " name=\"", 455, "\"", 473, 1);
      fieldTypeFileUpload.WriteAttributeValue("", 462, (object) fieldTypeFileUpload.Model.Name, 462, 11, false);
      fieldTypeFileUpload.EndWriteAttribute();
      fieldTypeFileUpload.BeginWriteAttribute("id", " id=\"", 474, "\"", 488, 1);
      fieldTypeFileUpload.WriteAttributeValue("", 479, (object) fieldTypeFileUpload.Model.Id, 479, 9, false);
      fieldTypeFileUpload.EndWriteAttribute();
      fieldTypeFileUpload.WriteLiteral(" data-umb=\"");
      fieldTypeFileUpload.Write(fieldTypeFileUpload.Model.Id);
      fieldTypeFileUpload.WriteLiteral("\" ");
      fieldTypeFileUpload.Write(fieldTypeFileUpload.Model.AllowMultipleFileUploads ? "multiple" : string.Empty);
      fieldTypeFileUpload.WriteLiteral("\n       data-val=\"");
      fieldTypeFileUpload.Write(!flag ? "true" : (string) null);
      fieldTypeFileUpload.WriteLiteral("\"\n       data-val-required=\"");
      fieldTypeFileUpload.Write(!flag ? fieldTypeFileUpload.Model.RequiredErrorMessage : (string) null);
      fieldTypeFileUpload.WriteLiteral("\"\n       ");
      if (!string.IsNullOrEmpty(fieldTypeFileUpload.Model.ToolTip))
      {
        fieldTypeFileUpload.WriteLiteral(" aria-describedby=\"");
        fieldTypeFileUpload.Write(fieldTypeFileUpload.Model.Id);
        fieldTypeFileUpload.WriteLiteral("_description\" ");
      }
      fieldTypeFileUpload.WriteLiteral("/>\n\n");
      if (fieldTypeFileUpload.Model.Values == null || !fieldTypeFileUpload.Model.Values.Any<object>())
        return;
      fieldTypeFileUpload.WriteLiteral("    <p>\n        <strong>");
      fieldTypeFileUpload.Write(str1);
      fieldTypeFileUpload.WriteLiteral(":</strong><br />\n");
      foreach (string str2 in fieldTypeFileUpload.Model.Values)
      {
        string str3 = ((IEnumerable<string>) str2.Split(new string[1]
        {
          FileUpload.EncryptedFilePathAndFileNameSeparator
        }, StringSplitOptions.None)).Last<string>();
        fieldTypeFileUpload.WriteLiteral("            <a>");
        fieldTypeFileUpload.Write(str3);
        fieldTypeFileUpload.WriteLiteral("</a><br />\n            <input type=\"hidden\"");
        fieldTypeFileUpload.BeginWriteAttribute("name", " name=\"", 1203, "\"", 1240, 3);
        fieldTypeFileUpload.WriteAttributeValue("", 1210, (object) fieldTypeFileUpload.Model.Name, 1210, 13, false);
        fieldTypeFileUpload.WriteAttributeValue("", 1223, (object) "_file_", 1223, 6, true);
        fieldTypeFileUpload.WriteAttributeValue("", 1229, (object) str3, 1229, 11, false);
        fieldTypeFileUpload.EndWriteAttribute();
        fieldTypeFileUpload.BeginWriteAttribute("value", " value=\"", 1241, "\"", 1258, 1);
        fieldTypeFileUpload.WriteAttributeValue("", 1249, (object) str2, 1249, 9, false);
        fieldTypeFileUpload.EndWriteAttribute();
        fieldTypeFileUpload.WriteLiteral(" />\n");
      }
      fieldTypeFileUpload.WriteLiteral("    </p>\n");
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
