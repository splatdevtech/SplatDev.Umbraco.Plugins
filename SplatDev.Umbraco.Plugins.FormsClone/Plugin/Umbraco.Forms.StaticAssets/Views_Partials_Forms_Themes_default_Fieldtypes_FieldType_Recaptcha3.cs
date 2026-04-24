using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web;
using Umbraco.Forms.Web.Models;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "95b5d4e693e4ee5956bce98dcc4db1b6c6947ac98ea521280cf6b9374898b4d9", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Recaptcha3.cshtml")]
    //[RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Fieldtypes/FieldType.Recaptcha3.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Recaptcha3 :
      RazorPage<FieldViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Fieldtypes_FieldType_Recaptcha3 fieldTypeRecaptcha3 = this;
            fieldTypeRecaptcha3.WriteLiteral("\n");
            fieldTypeRecaptcha3.WriteLiteral("\n");
            string? siteKey = fieldTypeRecaptcha3.Configuration?.Value.SiteKey;
            string? domainName = fieldTypeRecaptcha3.Configuration?.Value.Domain.GetDomainName();
            if (!string.IsNullOrEmpty(siteKey))
            {
                fieldTypeRecaptcha3.Html?.AddFormThemeScriptFile("https://" + domainName + "/recaptcha/api.js?render=" + siteKey);
                fieldTypeRecaptcha3.Html?.AddFormThemeScriptFile("~/App_Plugins/UmbracoForms/assets/recaptcha.v3.init.min.js");
                fieldTypeRecaptcha3.WriteLiteral("        <input type=\"hidden\"");
                fieldTypeRecaptcha3.BeginWriteAttribute("id", " id=\"", 725, "\"", 739, 1);
                fieldTypeRecaptcha3.WriteAttributeValue("", 730, fieldTypeRecaptcha3.Model.Id, 730, 9, false);
                fieldTypeRecaptcha3.EndWriteAttribute();
                fieldTypeRecaptcha3.WriteLiteral(" name=\"g-recaptcha-response\" />\n");
                fieldTypeRecaptcha3.WriteLiteral("        <div class=\"umbraco-forms-recaptcha-v3-config umbraco-forms-hidden\"\n             data-id=\"");
                fieldTypeRecaptcha3.Write(fieldTypeRecaptcha3.Model.Id);
                fieldTypeRecaptcha3.WriteLiteral("\"\n             data-site-key=\"");
                fieldTypeRecaptcha3.Write(siteKey);
                fieldTypeRecaptcha3.WriteLiteral("\"></div>\n");
            }
            else
            {
                fieldTypeRecaptcha3.WriteLiteral("        <p class=\"error\">ERROR: reCAPTCHA v3 is missing the Site Key. Please update the configuration to include a value at: ");
                fieldTypeRecaptcha3.Write(Constants.Configuration.SectionKeys.FieldTypes.Recaptcha3);
                fieldTypeRecaptcha3.WriteLiteral(":SiteKey</p>\n");

                await Task.CompletedTask;
            }
        }

        [RazorInject]
        public IOptionsSnapshot<Recaptcha3Settings>? Configuration
        { get; private set; }

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