using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

namespace FormBuilder.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "8e0cdaa01300836ee48a3946da55108a8d7c698a397ca85a8702e27b5afbf986", "/Views/Partials/Forms/Themes/default/ScrollToFormScript.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/ScrollToFormScript.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_ScrollToFormScript :
      RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_ScrollToFormScript scrollToFormScript = this;
            if (scrollToFormScript.ViewData.ModelState.IsValid && !scrollToFormScript.Model.SubmitHandled && !scrollToFormScript.Model.PageHandled)
                return;
            scrollToFormScript.WriteLiteral("    <div id=\"umbraco-forms-form-submitted\" data-form-client-id=\"");
            scrollToFormScript.Write(scrollToFormScript.Model.FormClientId);
            scrollToFormScript.WriteLiteral("\" class=\"umbraco-forms-hidden\"></div>\n");
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
        public IHtmlHelper<FormViewModel>? Html { get; private set; }
    }
}