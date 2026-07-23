using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Web;
using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.Services;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "2d6bf9011227386038abdee4f817c76ee98e17b0456e18d8da21812c446c007f", "/Views/Partials/Forms/Themes/bootstrap3-horizontal/Form.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/bootstrap3-horizontal/Form.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_bootstrap3_horizontal_Form :
      RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_bootstrap3_horizontal_Form bootstrap3HorizontalForm1 = this;
            bootstrap3HorizontalForm1.WriteLiteral("\n");
            bootstrap3HorizontalForm1.WriteLiteral("\n");
            bootstrap3HorizontalForm1.WriteLiteral("\n");
            bootstrap3HorizontalForm1.Write(bootstrap3HorizontalForm1.Html?.SetFormFieldClass("form-control"));
            bootstrap3HorizontalForm1.WriteLiteral("\n\n");
            bootstrap3HorizontalForm1.Write(bootstrap3HorizontalForm1.Html?.SetFormFieldWrapperClass("form-group"));
            bootstrap3HorizontalForm1.WriteLiteral("\n\n");
            if (!bootstrap3HorizontalForm1.Model.DisableDefaultStylesheet)
                bootstrap3HorizontalForm1.Html?.SetFormThemeCssFile("~/App_Plugins/UmbracoForms/assets/themes/bootstrap3-horizontal/style.min.css");
            bootstrap3HorizontalForm1.WriteLiteral("\n<div class=\"umbraco-forms-page form-horizontal\"");
            bootstrap3HorizontalForm1.BeginWriteAttribute("id", " id=\"", 711, "\"", 798, 1);
            bootstrap3HorizontalForm1.WriteAttributeValue("", 716, bootstrap3HorizontalForm1.Model.CurrentPage is not null ? bootstrap3HorizontalForm1.Model.CurrentPage.Id : "umbraco-forms-summary-page", 716, 82, false);
            bootstrap3HorizontalForm1.EndWriteAttribute();
            bootstrap3HorizontalForm1.WriteLiteral(">\n\n");
            if (bootstrap3HorizontalForm1.Model.IsMultiPage && bootstrap3HorizontalForm1.Model.ShowPagingOnMultiPageFormsAtTop)
                await bootstrap3HorizontalForm1.RenderPaging();
            bootstrap3HorizontalForm1.WriteLiteral("\n");
            if (bootstrap3HorizontalForm1.Model.ShowSummaryPage)
            {
                string? pageFormSummaryView = bootstrap3HorizontalForm1.FormThemeResolver?.GetMultiPageFormSummaryView(bootstrap3HorizontalForm1.Model);
                await bootstrap3HorizontalForm1.Html.RenderPartialAsync(pageFormSummaryView);
            }
            else
            {
                if (!string.IsNullOrEmpty(bootstrap3HorizontalForm1.Model.CurrentPage?.Caption))
                {
                    bootstrap3HorizontalForm1.WriteLiteral("            <h4 class=\"umbraco-forms-caption\">");
                    bootstrap3HorizontalForm1.Write(bootstrap3HorizontalForm1.Model.CurrentPage.Caption);
                    bootstrap3HorizontalForm1.WriteLiteral("</h4>\n");
                }
                if (bootstrap3HorizontalForm1.Model.ShowValidationSummary)
                    bootstrap3HorizontalForm1.Write(bootstrap3HorizontalForm1.Html.ValidationSummary(false));
                var fieldSets = bootstrap3HorizontalForm1.Model.CurrentPage?.Fieldsets ?? [];
                foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)fieldSets)
                {
                    Views_Partials_Forms_Themes_bootstrap3_horizontal_Form bootstrap3HorizontalForm = bootstrap3HorizontalForm1;
                    bool hideFieldSetWhenRendering = fieldset.HasCondition && fieldset.ConditionActionType == FieldConditionActionType.Show;
                    bootstrap3HorizontalForm1.WriteLiteral("            <fieldset");
                    bootstrap3HorizontalForm1.BeginWriteAttribute("class", " class=\"", 1640, "\"", 1744, 2);
                    bootstrap3HorizontalForm1.WriteAttributeValue("", 1648, "umbraco-forms-fieldset", 1648, 22, true);
                    bootstrap3HorizontalForm1.WriteAttributeValue("", 1670, new HelperResult(async __razor_attribute_value_writer =>
                    {
                        bootstrap3HorizontalForm.PushWriter(__razor_attribute_value_writer);
                        if (hideFieldSetWhenRendering)
                            bootstrap3HorizontalForm.WriteLiteral(" umbraco-forms-hidden");
                        bootstrap3HorizontalForm.PopWriter();
                        await Task.CompletedTask;
                    }), 1670, 74, false);
                    bootstrap3HorizontalForm1.EndWriteAttribute();
                    bootstrap3HorizontalForm1.BeginWriteAttribute("id", " id=\"", 1745, "\"", 1756, 1);
                    bootstrap3HorizontalForm1.WriteAttributeValue("", 1750, fieldset.Id, 1750, 6, false);
                    bootstrap3HorizontalForm1.EndWriteAttribute();
                    bootstrap3HorizontalForm1.WriteLiteral(">\n\n");
                    if (!string.IsNullOrEmpty(fieldset.Caption))
                    {
                        bootstrap3HorizontalForm1.WriteLiteral("                    <legend>");
                        bootstrap3HorizontalForm1.Write(fieldset.Caption);
                        bootstrap3HorizontalForm1.WriteLiteral("</legend>\n");
                    }
                    bootstrap3HorizontalForm1.WriteLiteral("\n                <div class=\"row-fluid\">\n\n");
                    foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                    {
                        bootstrap3HorizontalForm1.WriteLiteral("                        <div");
                        bootstrap3HorizontalForm1.BeginWriteAttribute("class", " class=\"", 2055, "\"", 2109, 2);
                        bootstrap3HorizontalForm1.WriteAttributeValue("", 2063, "umbraco-forms-container", 2063, 23, true);
                        bootstrap3HorizontalForm1.WriteAttributeValue(" ", 2086, "col-md-" + container.Width.ToString(), 2087, 22, false);
                        bootstrap3HorizontalForm1.EndWriteAttribute();
                        bootstrap3HorizontalForm1.WriteLiteral(">\n\n");
                        foreach (FieldViewModel field in (IEnumerable<FieldViewModel>)container.Fields)
                        {
                            FieldViewModel f = field;
                            bool hideFieldWhenRendering = f.HasCondition && f.ConditionActionType == FieldConditionActionType.Show;
                            bootstrap3HorizontalForm1.WriteLiteral("                                <div");
                            bootstrap3HorizontalForm1.BeginWriteAttribute("class", " class=\"", 2384, "\"", 2523, 3);
                            bootstrap3HorizontalForm1.WriteAttributeValue("", 2392, bootstrap3HorizontalForm1.Html?.GetFormFieldWrapperClass(f.FieldTypeName), 2392, 47, false);
                            bootstrap3HorizontalForm1.WriteAttributeValue(" ", 2439, f.CssClass, 2440, 11, false);
                            bootstrap3HorizontalForm1.WriteAttributeValue(" ", 2451, new HelperResult(async __razor_attribute_value_writer =>
                            {
                                __razor_attribute_value_writer.Write(" ");
                                if (hideFieldWhenRendering)
                                    __razor_attribute_value_writer.Write("umbraco-forms-hidden");
                                await Task.CompletedTask;
                            }), 2452, 71, false);
                            bootstrap3HorizontalForm1.EndWriteAttribute();
                            bootstrap3HorizontalForm1.WriteLiteral(">\n\n                                    <label");
                            bootstrap3HorizontalForm1.BeginWriteAttribute("for", " for=\"", 2569, "\"", 2580, 1);
                            bootstrap3HorizontalForm1.WriteAttributeValue("", 2575, f.Id, 2575, 5, false);
                            bootstrap3HorizontalForm1.EndWriteAttribute();
                            bootstrap3HorizontalForm1.BeginWriteAttribute("class", " class=\"", 2581, "\"", 2692, 4);
                            bootstrap3HorizontalForm1.WriteAttributeValue("", 2589, "col-sm-2", 2589, 8, true);
                            bootstrap3HorizontalForm1.WriteAttributeValue(" ", 2597, "control-label", 2598, 14, true);
                            bootstrap3HorizontalForm1.WriteAttributeValue(" ", 2611, "umbraco-forms-label", 2612, 20, true);
                            bootstrap3HorizontalForm1.WriteAttributeValue("", 2631, new HelperResult(async __razor_attribute_value_writer =>
                            {
                                __razor_attribute_value_writer.Write(" ");
                                if (f.HideLabel)
                                    __razor_attribute_value_writer.Write("umbraco-forms-hidden");
                                await Task.CompletedTask;
                            }), 2631, 61, false);
                            bootstrap3HorizontalForm1.EndWriteAttribute();
                            bootstrap3HorizontalForm1.WriteLiteral(">\n                                        ");
                            bootstrap3HorizontalForm1.Write(f.Caption);
                            bootstrap3HorizontalForm1.WriteLiteral("\n");
                            if (f.ShowIndicator)
                            {
                                bootstrap3HorizontalForm1.WriteLiteral("                                            <span class=\"UmbracoForms-Indicator\">");
                                bootstrap3HorizontalForm1.Write(bootstrap3HorizontalForm1.Model.Indicator);
                                bootstrap3HorizontalForm1.WriteLiteral("</span>\n");
                            }
                            bootstrap3HorizontalForm1.WriteLiteral("                                    </label>\n\n                                    <div class=\"col-sm-10 umbraco-forms-field-wrapper\">\n\n                                        ");
                            IHtmlContent htmlContent = await bootstrap3HorizontalForm1.Html.PartialAsync(bootstrap3HorizontalForm1.FormThemeResolver?.GetFieldView(bootstrap3HorizontalForm1.Model, f), f);
                            bootstrap3HorizontalForm1.Write(htmlContent);
                            bootstrap3HorizontalForm1.WriteLiteral("\n\n");
                            if (bootstrap3HorizontalForm1.Model.ShowFieldValidaton)
                                bootstrap3HorizontalForm1.Write(bootstrap3HorizontalForm1.Html.ValidationMessage(f.Name));
                            bootstrap3HorizontalForm1.WriteLiteral("\n");
                            if (!string.IsNullOrEmpty(f.ToolTip))
                            {
                                bootstrap3HorizontalForm1.WriteLiteral("                                            <p class=\"help-block\">");
                                bootstrap3HorizontalForm1.Write(f.ToolTip);
                                bootstrap3HorizontalForm1.WriteLiteral("</p>\n");
                            }
                            bootstrap3HorizontalForm1.WriteLiteral("                                    </div>\n\n\n                                </div>\n");
                        }
                        bootstrap3HorizontalForm1.WriteLiteral("\n                        </div>\n");
                    }
                    bootstrap3HorizontalForm1.WriteLiteral("\n                </div>\n\n            </fieldset>\n");
                }
            }
            bootstrap3HorizontalForm1.WriteLiteral("\n    <div class=\"umbraco-forms-hidden\" aria-hidden=\"true\">\n        <input type=\"text\"");
            bootstrap3HorizontalForm1.BeginWriteAttribute("name", " name=\"", 4037, "\"", 4085, 1);
            bootstrap3HorizontalForm1.WriteAttributeValue("", 4044, bootstrap3HorizontalForm1.Model.FormId.ToString().Replace("-", ""), 4044, 41, false);
            bootstrap3HorizontalForm1.EndWriteAttribute();
            bootstrap3HorizontalForm1.WriteLiteral(" />\n    </div>\n\n\n    <div class=\"umbraco-forms-navigation row-fluid\">\n\n        <div class=\"col-sm-10 col-sm-offset-2\">\n");
            if (bootstrap3HorizontalForm1.Model.IsMultiPage)
            {
                if (!bootstrap3HorizontalForm1.Model.IsFirstPage)
                {
                    bootstrap3HorizontalForm1.WriteLiteral("                    <input class=\"btn prev cancel\" type=\"submit\"");
                    bootstrap3HorizontalForm1.BeginWriteAttribute("value", " value=\"", 4377, "\"", 4407, 1);
                    bootstrap3HorizontalForm1.WriteAttributeValue("", 4385, bootstrap3HorizontalForm1.Model.PreviousCaption, 4385, 22, false);
                    bootstrap3HorizontalForm1.EndWriteAttribute();
                    bootstrap3HorizontalForm1.WriteLiteral(" name=\"__prev\" data-form-navigate=\"prev\" />\n");
                }
                if (!bootstrap3HorizontalForm1.Model.IsLastPage)
                {
                    bootstrap3HorizontalForm1.WriteLiteral("                    <input type=\"submit\" class=\"btn next\"");
                    bootstrap3HorizontalForm1.BeginWriteAttribute("value", " value=\"", 4584, "\"", 4610, 1);
                    bootstrap3HorizontalForm1.WriteAttributeValue("", 4592, bootstrap3HorizontalForm1.Model.NextCaption, 4592, 18, false);
                    bootstrap3HorizontalForm1.EndWriteAttribute();
                    bootstrap3HorizontalForm1.WriteLiteral(" name=\"next\" data-form-navigate=\"next\" />\n");
                }
                if (bootstrap3HorizontalForm1.Model.IsLastPage)
                {
                    bootstrap3HorizontalForm1.WriteLiteral("                    <input type=\"submit\" class=\"btn primary\"");
                    bootstrap3HorizontalForm1.BeginWriteAttribute("value", " value=\"", 4787, "\"", 4815, 1);
                    bootstrap3HorizontalForm1.WriteAttributeValue("", 4795, bootstrap3HorizontalForm1.Model.SubmitCaption, 4795, 20, false);
                    bootstrap3HorizontalForm1.EndWriteAttribute();
                    bootstrap3HorizontalForm1.WriteLiteral(" name=\"submitbtn\" data-form-navigate=\"next\" />\n");
                }
            }
            else
            {
                bootstrap3HorizontalForm1.WriteLiteral("                <input type=\"submit\" class=\"btn btn-primary\"");
                bootstrap3HorizontalForm1.BeginWriteAttribute("value", " value=\"", 4986, "\"", 5014, 1);
                bootstrap3HorizontalForm1.WriteAttributeValue("", 4994, bootstrap3HorizontalForm1.Model.SubmitCaption, 4994, 20, false);
                bootstrap3HorizontalForm1.EndWriteAttribute();
                bootstrap3HorizontalForm1.WriteLiteral(" name=\"submitbtn\" data-form-navigate=\"next\" />\n");
            }
            bootstrap3HorizontalForm1.WriteLiteral("        </div>\n\n\n    </div>\n\n");
            if (bootstrap3HorizontalForm1.Model.IsMultiPage && bootstrap3HorizontalForm1.Model.ShowPagingOnMultiPageFormsAtBottom)
                await bootstrap3HorizontalForm1.RenderPaging();
            bootstrap3HorizontalForm1.WriteLiteral("\n</div>\n\n");
            IHtmlContent htmlContent1 = await bootstrap3HorizontalForm1.Html.PartialAsync("Forms/Themes/default/ScrollToFormScript");
            bootstrap3HorizontalForm1.Write(htmlContent1);
            bootstrap3HorizontalForm1.WriteLiteral("\n\n");
        }

        private async Task RenderPaging()
        {
            Views_Partials_Forms_Themes_bootstrap3_horizontal_Form bootstrap3HorizontalForm = this;
            string? pagingDetailsView = bootstrap3HorizontalForm.FormThemeResolver?.GetMultiPageFormPagingDetailsView(bootstrap3HorizontalForm.Model);
            await bootstrap3HorizontalForm.Html.RenderPartialAsync(pagingDetailsView);
        }

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