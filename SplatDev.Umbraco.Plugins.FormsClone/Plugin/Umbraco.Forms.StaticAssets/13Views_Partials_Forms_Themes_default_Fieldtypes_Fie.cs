
// Type: AspNetCoreGeneratedDocument.Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Recaptcha2
// Assembly: Umbraco.Forms.StaticAssets, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: DDF8130F-541E-4CFE-9C57-74C2C790186A

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace AspNetCoreGeneratedDocument
{
  [RazorSourceChecksum("Sha256", "f6ed55deb710f09f8c07e6659f15e0ed9080af26385d3dc528269aa5707dddf8", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Recaptcha2.cshtml")]
  [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Recaptcha2.cshtml")]
  [CreateNewOnMetadataUpdate]
  internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Recaptcha2 : 
    RazorPage<FieldViewModel>
  {
    public override async 
    #nullable disable
    Task ExecuteAsync()
    {
      Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Recaptcha2 fieldTypeRecaptcha2 = this;
      fieldTypeRecaptcha2.WriteLiteral("\n");
      fieldTypeRecaptcha2.WriteLiteral("\n");
      string publicKey = fieldTypeRecaptcha2.Configuration.Value.PublicKey;
      string str1 = "clean";
      KeyValuePair<string, string> keyValuePair1 = fieldTypeRecaptcha2.Model.AdditionalSettings.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Key == "Theme"));
      if (keyValuePair1.Value != "")
        str1 = keyValuePair1.Value;
      string str2 = "normal";
      KeyValuePair<string, string> keyValuePair2 = fieldTypeRecaptcha2.Model.AdditionalSettings.FirstOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Key == "Size"));
      if (keyValuePair2.Value != "")
        str2 = keyValuePair2.Value;
      if (!string.IsNullOrEmpty(publicKey))
      {
        fieldTypeRecaptcha2.WriteLiteral("        <script src=\"https://www.google.com/recaptcha/api.js\" async defer type=\"application/javascript\"></script>\n        <div class=\"g-recaptcha\" data-sitekey=\"");
        fieldTypeRecaptcha2.Write(publicKey);
        fieldTypeRecaptcha2.WriteLiteral("\" data-theme=\"");
        fieldTypeRecaptcha2.Write(str1);
        fieldTypeRecaptcha2.WriteLiteral("\" data-size=\"");
        fieldTypeRecaptcha2.Write(str2);
        fieldTypeRecaptcha2.WriteLiteral("\"></div>\n");
      }
      else
      {
        fieldTypeRecaptcha2.WriteLiteral("        <p class=\"error\">ERROR: ReCaptcha v2 is missing the Site Key. Please update the configuration to include a value at: ");
        fieldTypeRecaptcha2.Write(Constants.Configuration.SectionKeys.FieldTypes.Recaptcha2);
        fieldTypeRecaptcha2.WriteLiteral(":PublicKey</p>\n");
      }
    }

    [RazorInject]
    public 
    #nullable enable
    IOptionsSnapshot<Recaptcha2Settings> Configuration { get; private set; }

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
