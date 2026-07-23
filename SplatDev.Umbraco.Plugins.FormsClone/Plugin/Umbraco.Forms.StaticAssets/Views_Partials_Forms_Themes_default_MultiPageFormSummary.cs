using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.Services;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "931c0a278efe865abdfc337ac758c5371c62a26f03af0c8889ccb2f7c12b8096", "/Views/Partials/Forms/Themes/default/MultiPageFormSummary.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/MultiPageFormSummary.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_MultiPageFormSummary :
      RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_MultiPageFormSummary multiPageFormSummary = this;
            multiPageFormSummary.WriteLiteral("\n");
            multiPageFormSummary.WriteLiteral("\n<div class=\"umbraco-forms-entry-summary\">\n    <h4 class=\"umbraco-forms-caption\">");
            multiPageFormSummary.Write(multiPageFormSummary.Model.SummaryCaption);
            multiPageFormSummary.WriteLiteral("</h4>\n\n");
            List<string> ignoreFields =
              [
                "FieldType.Recaptcha2.cshtml",
                "FieldType.Recaptcha3.cshtml",
                "FieldType.RichText.cshtml",
                "FieldType.Text.cshtml",
                "FieldType.HiddenField.cshtml",
                "FieldType.Password.cshtml"
              ];
            int pageIndex = 0;
            foreach (PageViewModel page in multiPageFormSummary.Model.Pages)
            {
                multiPageFormSummary.WriteLiteral("            <h5 class=\"umbraco-forms-entry-summary-page-caption\">");
                multiPageFormSummary.Write(multiPageFormSummary.Model.GetPageCaption(pageIndex));
                multiPageFormSummary.WriteLiteral("</h5>\n");
                foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)page.Fieldsets)
                {
                    if (!string.IsNullOrWhiteSpace(fieldset.Caption))
                    {
                        multiPageFormSummary.WriteLiteral("                    <h6 class=\"umbraco-forms-entry-summary-fieldset-caption\">");
                        multiPageFormSummary.Write(fieldset.Caption);
                        multiPageFormSummary.WriteLiteral("</h6>\n");
                    }
                    foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                    {
                        multiPageFormSummary.WriteLiteral("                    <dl class=\"umbraco-forms-entry-summary-entries\">\n");
                        foreach (FieldViewModel fieldViewModel in container.Fields.Where(x => x.FieldType is not null && !ignoreFields.Contains(x.FieldType.FieldTypeViewName)))
                        {
                            multiPageFormSummary.WriteLiteral("                            <dt>");
                            multiPageFormSummary.Write(fieldViewModel.Caption);
                            multiPageFormSummary.WriteLiteral("</dt>\n                            <dd>\n");
                            string? readOnlyFieldView = multiPageFormSummary.FormThemeResolver?.GetReadOnlyFieldView(multiPageFormSummary.Model, fieldViewModel);
                            ViewEngineResult? getView = !string.IsNullOrEmpty(readOnlyFieldView) ? multiPageFormSummary.CompositeViewEngine?.GetView(string.Empty, readOnlyFieldView, false) : null;
                            if (getView is not null && getView!.Success)
                            {
                                readOnlyFieldView = multiPageFormSummary.FormThemeResolver?.GetGenericReadOnlyFieldView(multiPageFormSummary.Model);
                            }

                            multiPageFormSummary.WriteLiteral("                                ");
                            IHtmlContent htmlContent = await multiPageFormSummary.Html.PartialAsync(readOnlyFieldView, fieldViewModel);
                            multiPageFormSummary.Write(htmlContent);
                            multiPageFormSummary.WriteLiteral("\n                            </dd>\n");
                        }
                        multiPageFormSummary.WriteLiteral("                    </dl>\n");
                    }
                }
                ++pageIndex;
            }
            multiPageFormSummary.WriteLiteral("</div>\n");
            await Task.CompletedTask;
        }

        [RazorInject]
        public ICompositeViewEngine? CompositeViewEngine { get; private set; }

        [RazorInject]
        public IFormThemeResolver? FormThemeResolver { get; private set; }

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