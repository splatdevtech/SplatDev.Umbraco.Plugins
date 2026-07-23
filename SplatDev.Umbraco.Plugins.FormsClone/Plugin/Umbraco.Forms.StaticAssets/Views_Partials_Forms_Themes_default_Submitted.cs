using Microsoft.AspNetCore.Html;
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
    //[RazorSourceChecksum("Sha256", "a9baf9fa575105f39bea4de70543b5d3c1ebd6dbf3a7abc84f179ba536fdf171", "/Views/Partials/Forms/Themes/default/Submitted.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Submitted.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Submitted :
        RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Submitted defaultSubmitted = this;
            defaultSubmitted.WriteLiteral("\n");
            if (defaultSubmitted.Model.MessageOnSubmitIsHtml)
            {
                defaultSubmitted.WriteLiteral("    <div class=\"formbuilder-forms-submitmessage-html\">\n        ");
                defaultSubmitted.Write(defaultSubmitted.Model.GetMessageOnSubmit());
                defaultSubmitted.WriteLiteral("\n    </div>\n");
            }
            else
            {
                defaultSubmitted.WriteLiteral("    <div>\n        <span class=\"formbuilder-forms-submitmessage\">\n            ");
                defaultSubmitted.Write(defaultSubmitted.Model.GetMessageOnSubmit());
                defaultSubmitted.WriteLiteral("\n        </span>\n    </div>\n");
            }
            IHtmlContent htmlContent = await defaultSubmitted.Html.PartialAsync("Forms/Themes/default/ScrollToFormScript");
            defaultSubmitted.Write(htmlContent);
            defaultSubmitted.WriteLiteral("\n");
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
        public IHtmlHelper<FormViewModel>? Html { get; private set; }
    }
}