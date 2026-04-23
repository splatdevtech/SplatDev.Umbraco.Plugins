using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Core.Providers.FieldTypes;
using Umbraco.Forms.Web.Models;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "be628114c5f5fe25434b144ca0b888b80a42647616a130f8bdb562fe43b4e5f7", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.ReadOnly.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.ReadOnly.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload_ReadOnly :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload_ReadOnly fileUploadReadOnly = this;
            fileUploadReadOnly.WriteLiteral("\n");
            foreach (string str1 in fileUploadReadOnly.Model.Values.Cast<string>())
            {
                string str2 = str1.Split(
                [
                    FileUpload.EncryptedFilePathAndFileNameSeparator
                ], StringSplitOptions.None).Last();
                fileUploadReadOnly.WriteLiteral("        <div>");
                fileUploadReadOnly.Write(str2);
                fileUploadReadOnly.WriteLiteral("</div>\n");
            }
            fileUploadReadOnly.WriteLiteral("\n\n\n");
            await Task.CompletedTask;
        }

        [RazorInject]
        public IModelExpressionProvider? ModelExpressionProvider { get; private set; }

        [RazorInject]
        public IUrlHelper? Url { get; private set; }

        [RazorInject]
        public IViewComponentHelper? Component { get; private set; }

        [RazorInject]
        public IJsonHelper? Json { get; private set; }

        [RazorInject]
        public IHtmlHelper<FieldViewModel>? Html { get; private set; }
    }
}