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
    //[RazorSourceChecksum("Sha256", "bb8504720b2726398276b3fd49273cac35d991f01024ab457334497d4e4bfd92", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.FileUpload.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_FileUpload fieldTypeFileUpload = this;
            fieldTypeFileUpload.WriteLiteral("\n");
            bool flag = fieldTypeFileUpload.Model.Mandatory && fieldTypeFileUpload.Model.Values is not null && fieldTypeFileUpload.Model.Values.Any() || !fieldTypeFileUpload.Model.Mandatory;
            if (!fieldTypeFileUpload.Model.AdditionalSettings.TryGetValue("SelectedFilesListHeading", out string? str1) || string.IsNullOrWhiteSpace(str1))
                str1 = "Current file(s)";
            fieldTypeFileUpload.WriteLiteral("<input type=\"file\"");
            fieldTypeFileUpload.BeginWriteAttribute("name", " name=\"", 455, "\"", 473, 1);
            fieldTypeFileUpload.WriteAttributeValue("", 462, fieldTypeFileUpload.Model.Name, 462, 11, false);
            fieldTypeFileUpload.EndWriteAttribute();
            fieldTypeFileUpload.BeginWriteAttribute("id", " id=\"", 474, "\"", 488, 1);
            fieldTypeFileUpload.WriteAttributeValue("", 479, fieldTypeFileUpload.Model.Id, 479, 9, false);
            fieldTypeFileUpload.EndWriteAttribute();
            fieldTypeFileUpload.WriteLiteral(" data-umb=\"");
            fieldTypeFileUpload.Write(fieldTypeFileUpload.Model.Id);
            fieldTypeFileUpload.WriteLiteral("\" ");
            fieldTypeFileUpload.Write(fieldTypeFileUpload.Model.AllowMultipleFileUploads ? "multiple" : string.Empty);
            fieldTypeFileUpload.WriteLiteral("\n       data-val=\"");
            fieldTypeFileUpload.Write(!flag ? "true" : null);
            fieldTypeFileUpload.WriteLiteral("\"\n       data-val-required=\"");
            fieldTypeFileUpload.Write(!flag ? fieldTypeFileUpload.Model.RequiredErrorMessage : null);
            fieldTypeFileUpload.WriteLiteral("\"\n       ");
            if (!string.IsNullOrEmpty(fieldTypeFileUpload.Model.ToolTip))
            {
                fieldTypeFileUpload.WriteLiteral(" aria-describedby=\"");
                fieldTypeFileUpload.Write(fieldTypeFileUpload.Model.Id);
                fieldTypeFileUpload.WriteLiteral("_description\" ");
            }
            fieldTypeFileUpload.WriteLiteral("/>\n\n");
            if (fieldTypeFileUpload.Model.Values is null || !fieldTypeFileUpload.Model.Values.Any())
                return;
            fieldTypeFileUpload.WriteLiteral("    <p>\n        <strong>");
            fieldTypeFileUpload.Write(str1);
            fieldTypeFileUpload.WriteLiteral(":</strong><br />\n");
            foreach (string str2 in fieldTypeFileUpload.Model.Values.Cast<string>())
            {
                string str3 = str2.Split(
                [
                    FileUpload.EncryptedFilePathAndFileNameSeparator
                ], StringSplitOptions.None).Last();

                fieldTypeFileUpload.WriteLiteral("            <a>");
                fieldTypeFileUpload.Write(str3);
                fieldTypeFileUpload.WriteLiteral("</a><br />\n            <input type=\"hidden\"");
                fieldTypeFileUpload.BeginWriteAttribute("name", " name=\"", 1203, "\"", 1240, 3);
                fieldTypeFileUpload.WriteAttributeValue("", 1210, fieldTypeFileUpload.Model.Name, 1210, 13, false);
                fieldTypeFileUpload.WriteAttributeValue("", 1223, "_file_", 1223, 6, true);
                fieldTypeFileUpload.WriteAttributeValue("", 1229, str3, 1229, 11, false);
                fieldTypeFileUpload.EndWriteAttribute();
                fieldTypeFileUpload.BeginWriteAttribute("value", " value=\"", 1241, "\"", 1258, 1);
                fieldTypeFileUpload.WriteAttributeValue("", 1249, str2, 1249, 9, false);
                fieldTypeFileUpload.EndWriteAttribute();
                fieldTypeFileUpload.WriteLiteral(" />\n");
            }
            fieldTypeFileUpload.WriteLiteral("    </p>\n");
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