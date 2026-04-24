
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload_ReadOnly
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
  [RazorSourceChecksum("Sha256", "be628114c5f5fe25434b144ca0b888b80a42647616a130f8bdb562fe43b4e5f7", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.ReadOnly.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.ReadOnly.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload_ReadOnly : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload_ReadOnly fileUploadReadOnly = this;
      fileUploadReadOnly.WriteLiteral("\n");
      foreach (string str1 in fileUploadReadOnly.Model.Values)
      {
        string str2 = ((IEnumerable<string>) str1.Split(new string[1]
        {
          FileUpload.EncryptedFilePathAndFileNameSeparator
        }, StringSplitOptions.None)).Last<string>();
        fileUploadReadOnly.WriteLiteral("        <div>");
        fileUploadReadOnly.Write(str2);
        fileUploadReadOnly.WriteLiteral("</div>\n");
      }
      fileUploadReadOnly.WriteLiteral("\n\n\n");
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
