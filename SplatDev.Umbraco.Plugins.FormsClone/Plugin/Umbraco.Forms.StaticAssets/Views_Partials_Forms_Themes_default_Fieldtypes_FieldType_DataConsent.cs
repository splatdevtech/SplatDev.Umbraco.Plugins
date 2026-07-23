using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Web.Models;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "b6b67a9ad4e769af456967593a37361492130f24f3dec2bafeabac8debeac531", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DataConsent.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.DataConsent.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DataConsent :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_DataConsent fieldTypeDataConsent = this;
            fieldTypeDataConsent.Model.AdditionalSettings.TryGetValue("AcceptCopy", out _);
            fieldTypeDataConsent.WriteLiteral("<input type=\"checkbox\"");
            fieldTypeDataConsent.BeginWriteAttribute("name", " name=\"", 187, "\"", 205, 1);
            fieldTypeDataConsent.WriteAttributeValue("", 194, fieldTypeDataConsent.Model.Name, 194, 11, false);
            fieldTypeDataConsent.EndWriteAttribute();
            fieldTypeDataConsent.BeginWriteAttribute("id", " id=\"", 206, "\"", 220, 1);
            fieldTypeDataConsent.WriteAttributeValue("", 211, fieldTypeDataConsent.Model.Id, 211, 9, false);
            fieldTypeDataConsent.EndWriteAttribute();
            fieldTypeDataConsent.WriteLiteral(" value=\"true\" data-umb=\"");
            fieldTypeDataConsent.Write(fieldTypeDataConsent.Model.Id);
            fieldTypeDataConsent.WriteLiteral("\"\n       ");
            if (fieldTypeDataConsent.Model.Mandatory)
            {
                fieldTypeDataConsent.WriteLiteral(" data-val=\"true\" data-val-required=\"");
                fieldTypeDataConsent.Write(fieldTypeDataConsent.Model.RequiredErrorMessage);
                fieldTypeDataConsent.WriteLiteral("\" data-rule-required=\"true\" data-msg-required=\"");
                fieldTypeDataConsent.Write(fieldTypeDataConsent.Model.RequiredErrorMessage);
                fieldTypeDataConsent.WriteLiteral("\" aria-required=\"true\"");
            }
            fieldTypeDataConsent.WriteLiteral("       ");
            if (fieldTypeDataConsent.Model.ContainsValue(true) || fieldTypeDataConsent.Model.ContainsValue("true") || fieldTypeDataConsent.Model.ContainsValue("on"))
                fieldTypeDataConsent.WriteLiteral("checked=\"checked\"");
            fieldTypeDataConsent.WriteLiteral("       ");
            if (!string.IsNullOrEmpty(fieldTypeDataConsent.Model.ToolTip))
            {
                fieldTypeDataConsent.WriteLiteral(" aria-describedby=\"");
                fieldTypeDataConsent.Write(fieldTypeDataConsent.Model.Id);
                fieldTypeDataConsent.WriteLiteral("_description\" ");
            }
            fieldTypeDataConsent.WriteLiteral("/>\n<input type=\"hidden\"");
            fieldTypeDataConsent.BeginWriteAttribute("name", " name=\"", 892, "\"", 908, 1);
            fieldTypeDataConsent.WriteAttributeValue("", 899, fieldTypeDataConsent.Model.Id, 899, 9, false);
            fieldTypeDataConsent.EndWriteAttribute();
            fieldTypeDataConsent.WriteLiteral(" value=\"false\" />\n\n<label");
            fieldTypeDataConsent.BeginWriteAttribute("for", " for=\"", 934, "\"", 949, 1);
            fieldTypeDataConsent.WriteAttributeValue("", 940, fieldTypeDataConsent.Model.Id, 940, 9, false);
            fieldTypeDataConsent.EndWriteAttribute();
            fieldTypeDataConsent.WriteLiteral(">");
            fieldTypeDataConsent.Write(string.Empty);
            fieldTypeDataConsent.WriteLiteral("</label>\n");
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