using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;

namespace FormBuilder.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "704dfe8a489bcdbe3ea51d350b4cfb2c6e7d3e6abd59b0dd013e93c40b5f7ca2", "/Views/Partials/Forms/Themes/default/Form.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Form.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Form :
        RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Form themesDefaultForm1 = this;
            themesDefaultForm1.WriteLiteral("\n");
            themesDefaultForm1.WriteLiteral("\n");
            themesDefaultForm1.WriteLiteral("\n");
            if (!themesDefaultForm1.Model.DisableDefaultStylesheet)
                themesDefaultForm1.Html?.SetFormThemeCssFile("~/App_Plugins/FormBuilder/assets/themes/default/style.min.css");
            themesDefaultForm1.WriteLiteral("\n<div class=\"umbraco-forms-page\"");
            themesDefaultForm1.BeginWriteAttribute("id", " id=\"", 660, "\"", 747, 1);
            themesDefaultForm1.WriteAttributeValue("", 665, themesDefaultForm1.Model.CurrentPage is not null ? themesDefaultForm1.Model.CurrentPage.Id : "umbraco-forms-summary-page", 665, 82, false);
            themesDefaultForm1.EndWriteAttribute();
            themesDefaultForm1.WriteLiteral(">\n\n");
            if (themesDefaultForm1.Model.IsMultiPage && themesDefaultForm1.Model.ShowPagingOnMultiPageFormsAtTop)
                await themesDefaultForm1.RenderPaging();
            themesDefaultForm1.WriteLiteral("\n");
            if (themesDefaultForm1.Model.ShowSummaryPage)
            {
                string? pageFormSummaryView = themesDefaultForm1.FormThemeResolver?.GetMultiPageFormSummaryView(themesDefaultForm1.Model);
                await themesDefaultForm1.Html.RenderPartialAsync(pageFormSummaryView);
            }
            else
            {
                if (!string.IsNullOrEmpty(themesDefaultForm1.Model.CurrentPage?.Caption))
                {
                    themesDefaultForm1.WriteLiteral("            <h4 class=\"umbraco-forms-caption\">");
                    themesDefaultForm1.Write(themesDefaultForm1.Model.CurrentPage.Caption);
                    themesDefaultForm1.WriteLiteral("</h4>\n");
                }
                if (themesDefaultForm1.Model.ShowValidationSummary)
                    themesDefaultForm1.Write(themesDefaultForm1.Html.ValidationSummary(false));
                var fieldSet = themesDefaultForm1.Model.CurrentPage?.Fieldsets ?? [];
                foreach (FieldsetViewModel fieldset in (IEnumerable<FieldsetViewModel>)fieldSet)
                {
                    Views_Partials_Forms_Themes_default_Form themesDefaultForm = themesDefaultForm1;
                    bool hideFieldSetWhenRendering = fieldset.HasCondition && fieldset.ConditionActionType == FieldConditionActionType.Show;
                    themesDefaultForm1.WriteLiteral("            <fieldset");
                    themesDefaultForm1.BeginWriteAttribute("class", " class=\"", 1589, "\"", 1694, 2);
                    themesDefaultForm1.WriteAttributeValue("", 1597, "umbraco-forms-fieldset", 1597, 22, true);
                    themesDefaultForm1.WriteAttributeValue("", 1619, new HelperResult(async __razor_attribute_value_writer =>
                    {
                        themesDefaultForm.PushWriter(__razor_attribute_value_writer);
                        if (hideFieldSetWhenRendering)
                            themesDefaultForm.WriteLiteral(" umbraco-forms-hidden");
                        themesDefaultForm.PopWriter();
                        await Task.CompletedTask;
                    }), 1619, 75, false);
                    themesDefaultForm1.EndWriteAttribute();
                    themesDefaultForm1.BeginWriteAttribute("id", " id=\"", 1695, "\"", 1706, 1);
                    themesDefaultForm1.WriteAttributeValue("", 1700, fieldset.Id, 1700, 6, false);
                    themesDefaultForm1.EndWriteAttribute();
                    themesDefaultForm1.WriteLiteral(">\n\n");
                    if (!string.IsNullOrEmpty(fieldset.Caption))
                    {
                        themesDefaultForm1.WriteLiteral("                    <legend>");
                        themesDefaultForm1.Write(fieldset.Caption);
                        themesDefaultForm1.WriteLiteral("</legend>\n");
                    }
                    themesDefaultForm1.WriteLiteral("\n                <div class=\"row-fluid\">\n\n");
                    foreach (FieldsetContainerViewModel container in (IEnumerable<FieldsetContainerViewModel>)fieldset.Containers)
                    {
                        themesDefaultForm1.WriteLiteral("                        <div");
                        themesDefaultForm1.BeginWriteAttribute("class", " class=\"", 2005, "\"", 2059, 2);
                        themesDefaultForm1.WriteAttributeValue("", 2013, "umbraco-forms-container", 2013, 23, true);
                        themesDefaultForm1.WriteAttributeValue(" ", 2036, "col-md-" + container.Width.ToString(), 2037, 22, false);
                        themesDefaultForm1.EndWriteAttribute();
                        themesDefaultForm1.WriteLiteral(">\n\n");
                        foreach (FieldViewModel field in (IEnumerable<FieldViewModel>)container.Fields)
                        {
                            FieldViewModel f = field;
                            bool hideFieldWhenRendering = f.HasCondition && f.ConditionActionType == FieldConditionActionType.Show;
                            FieldType? fieldType = f.FieldType;
                            switch (fieldType is not null ? (int)fieldType.RenderInputType : 0)
                            {
                                case 0:
                                    themesDefaultForm1.WriteLiteral("                                        <div");
                                    themesDefaultForm1.BeginWriteAttribute("class", " class=\"", 2537, "\"", 2677, 3);
                                    themesDefaultForm1.WriteAttributeValue("", 2545, themesDefaultForm1.Html?.GetFormFieldWrapperClass(f.FieldTypeName), 2545, 47, false);
                                    themesDefaultForm1.WriteAttributeValue(" ", 2592, f.CssClass, 2593, 11, false);
                                    themesDefaultForm1.WriteAttributeValue(" ", 2604, new HelperResult(async __razor_attribute_value_writer =>
                                    {
                                        themesDefaultForm.PushWriter(__razor_attribute_value_writer);
                                        if (hideFieldWhenRendering)
                                            themesDefaultForm.WriteLiteral(" umbraco-forms-hidden");
                                        themesDefaultForm.PopWriter();
                                        await Task.CompletedTask;
                                    }), 2605, 72, false);
                                    themesDefaultForm1.EndWriteAttribute();
                                    themesDefaultForm1.WriteLiteral(">\n\n                                            <label");
                                    themesDefaultForm1.BeginWriteAttribute("for", " for=\"", 2731, "\"", 2742, 1);
                                    themesDefaultForm1.WriteAttributeValue("", 2737, f.Id, 2737, 5, false);
                                    themesDefaultForm1.EndWriteAttribute();
                                    themesDefaultForm1.BeginWriteAttribute("class", " class=\"", 2743, "\"", 2831, 2);
                                    themesDefaultForm1.WriteAttributeValue("", 2751, "umbraco-forms-label", 2751, 19, true);
                                    themesDefaultForm1.WriteAttributeValue("", 2770, new HelperResult(async __razor_attribute_value_writer =>
                                    {
                                        themesDefaultForm.PushWriter(__razor_attribute_value_writer);
                                        if (f.HideLabel)
                                            themesDefaultForm.WriteLiteral(" umbraco-forms-hidden");
                                        themesDefaultForm.PopWriter();
                                        await Task.CompletedTask;
                                    }), 2770, 61, false);
                                    themesDefaultForm1.EndWriteAttribute();
                                    themesDefaultForm1.WriteLiteral(">\n");
                                    themesDefaultForm1.RenderCaption(f);
                                    themesDefaultForm1.WriteLiteral("                                            </label>\n\n");
                                    await themesDefaultForm1.RenderField(f);
                                    themesDefaultForm1.WriteLiteral("\n                                        </div>\n");
                                    continue;
                                case 1:
                                    themesDefaultForm1.WriteLiteral("                                        <fieldset");
                                    themesDefaultForm1.BeginWriteAttribute("class", " class=\"", 3434, "\"", 3574, 3);
                                    themesDefaultForm1.WriteAttributeValue("", 3442, themesDefaultForm1.Html?.GetFormFieldWrapperClass(f.FieldTypeName), 3442, 47, false);
                                    themesDefaultForm1.WriteAttributeValue(" ", 3489, f.CssClass, 3490, 11, false);
                                    themesDefaultForm1.WriteAttributeValue(" ", 3501, new HelperResult(async __razor_attribute_value_writer =>
                                    {
                                        themesDefaultForm.PushWriter(__razor_attribute_value_writer);
                                        if (hideFieldWhenRendering)
                                            themesDefaultForm.WriteLiteral(" umbraco-forms-hidden");
                                        themesDefaultForm.PopWriter();
                                        await Task.CompletedTask;
                                    }), 3502, 72, false);
                                    themesDefaultForm1.EndWriteAttribute();
                                    themesDefaultForm1.WriteLiteral(">\n\n                                            <legend");
                                    themesDefaultForm1.BeginWriteAttribute("class", " class=\"", 3629, "\"", 3718, 2);
                                    themesDefaultForm1.WriteAttributeValue("", 3637, "umbraco-forms-legend", 3637, 20, true);
                                    themesDefaultForm1.WriteAttributeValue("", 3657, new HelperResult(async __razor_attribute_value_writer =>
                                    {
                                        themesDefaultForm.PushWriter(__razor_attribute_value_writer);
                                        if (f.HideLabel)
                                            themesDefaultForm.WriteLiteral(" umbraco-forms-hidden");
                                        themesDefaultForm.PopWriter();
                                        await Task.CompletedTask;
                                    }), 3657, 61, false);
                                    themesDefaultForm1.EndWriteAttribute();
                                    themesDefaultForm1.WriteLiteral(">\n");
                                    themesDefaultForm1.RenderCaption(f);
                                    themesDefaultForm1.WriteLiteral("                                            </legend>\n\n");
                                    await themesDefaultForm1.RenderField(f);
                                    themesDefaultForm1.WriteLiteral("\n                                        </fieldset>\n");
                                    continue;
                                case 2:
                                    IHtmlContent htmlContent = await themesDefaultForm1.Html.PartialAsync(themesDefaultForm1.FormThemeResolver?.GetFieldView(themesDefaultForm1.Model, f), f);
                                    themesDefaultForm1.Write(htmlContent);
                                    continue;
                                default:
                                    continue;
                            }
                        }
                        themesDefaultForm1.WriteLiteral("\n                        </div>\n");
                    }
                    themesDefaultForm1.WriteLiteral("                </div>\n\n            </fieldset>\n");
                }
            }
            themesDefaultForm1.WriteLiteral("\n    <div class=\"umbraco-forms-hidden\" aria-hidden=\"true\">\n        <input type=\"text\"");
            themesDefaultForm1.BeginWriteAttribute("name", " name=\"", 4700, "\"", 4748, 1);
            themesDefaultForm1.WriteAttributeValue("", 4707, themesDefaultForm1.Model.FormId.ToString().Replace("-", ""), 4707, 41, false);
            themesDefaultForm1.EndWriteAttribute();
            themesDefaultForm1.WriteLiteral(" />\n    </div>\n\n    <div class=\"umbraco-forms-navigation row-fluid\">\n\n        <div class=\"col-md-12\">\n");
            if (themesDefaultForm1.Model.IsMultiPage)
            {
                if (!themesDefaultForm1.Model.IsFirstPage)
                {
                    themesDefaultForm1.WriteLiteral("                    <input type=\"submit\"\n                           hidden\n                           name=\"__next\"\n                           data-form-navigate=\"next\"\n                           data-umb=\"");
                    themesDefaultForm1.Write(themesDefaultForm1.Model.IsLastPage ? "submit" : "next");
                    themesDefaultForm1.WriteLiteral("-forms-form\" />\n");
                    themesDefaultForm1.WriteLiteral("                    <input class=\"btn prev cancel\"\n                           type=\"submit\"");
                    themesDefaultForm1.BeginWriteAttribute("value", "\n                           value=\"", 5721, "\"", 5778, 1);
                    themesDefaultForm1.WriteAttributeValue("", 5756, themesDefaultForm1.Model.PreviousCaption, 5756, 22, false);
                    themesDefaultForm1.EndWriteAttribute();
                    themesDefaultForm1.WriteLiteral("\n                           name=\"__prev\"\n                           formnovalidate\n                           data-form-navigate=\"prev\"\n                           data-umb=\"prev-forms-form\"/>\n");
                }
                if (!themesDefaultForm1.Model.IsLastPage)
                {
                    themesDefaultForm1.WriteLiteral("                    <input type=\"submit\"\n                           class=\"btn next\"");
                    themesDefaultForm1.BeginWriteAttribute("value", "\n                           value=\"", 6132, "\"", 6185, 1);
                    themesDefaultForm1.WriteAttributeValue("", 6167, themesDefaultForm1.Model.NextCaption, 6167, 18, false);
                    themesDefaultForm1.EndWriteAttribute();
                    themesDefaultForm1.WriteLiteral("\n                           name=\"__next\"\n                           data-form-navigate=\"next\"\n                           data-umb=\"next-forms-form\"/>\n");
                }
                if (themesDefaultForm1.Model.IsLastPage)
                {
                    themesDefaultForm1.WriteLiteral("                    <input type=\"submit\"\n                           class=\"btn primary\"");
                    themesDefaultForm1.BeginWriteAttribute("value", "\n                           value=\"", 6499, "\"", 6554, 1);
                    themesDefaultForm1.WriteAttributeValue("", 6534, themesDefaultForm1.Model.SubmitCaption, 6534, 20, false);
                    themesDefaultForm1.EndWriteAttribute();
                    themesDefaultForm1.WriteLiteral("\n                           name=\"__next\"\n                           data-form-navigate=\"next\"\n                           data-umb=\"submit-forms-form\"/>\n");
                }
            }
            else
            {
                themesDefaultForm1.WriteLiteral("                <input type=\"submit\"\n                       class=\"btn primary\"");
                themesDefaultForm1.BeginWriteAttribute("value", "\n                       value=\"", 6850, "\"", 6901, 1);
                themesDefaultForm1.WriteAttributeValue("", 6881, themesDefaultForm1.Model.SubmitCaption, 6881, 20, false);
                themesDefaultForm1.EndWriteAttribute();
                themesDefaultForm1.WriteLiteral("\n                       name=\"__next\"\n                       data-form-navigate=\"next\"\n                       data-umb=\"submit-forms-form\" />\n");
            }
            themesDefaultForm1.WriteLiteral("        </div>\n    </div>\n\n");
            if (themesDefaultForm1.Model.IsMultiPage && themesDefaultForm1.Model.ShowPagingOnMultiPageFormsAtBottom)
                await themesDefaultForm1.RenderPaging();
            themesDefaultForm1.WriteLiteral("\n</div>\n\n");
            IHtmlContent htmlContent1 = await themesDefaultForm1.Html.PartialAsync("Forms/Themes/default/ScrollToFormScript");
            themesDefaultForm1.Write(htmlContent1);
            themesDefaultForm1.WriteLiteral("\n\n");

            await Task.CompletedTask;
        }

        private async Task RenderPaging()
        {
            Views_Partials_Forms_Themes_default_Form themesDefaultForm = this;
            string? pagingDetailsView = themesDefaultForm.FormThemeResolver?.GetMultiPageFormPagingDetailsView(themesDefaultForm.Model);
            await themesDefaultForm.Html.RenderPartialAsync(pagingDetailsView);
        }

        private void RenderCaption(FieldViewModel field)
        {
            Write(field.Caption);
            if (!field.ShowIndicator)
                return;
            WriteLiteral("            <span class=\"umbraco-forms-indicator\">");
            Write(Model.Indicator);
            WriteLiteral("</span>\n");
        }

        private async Task RenderField(FieldViewModel field)
        {
            Views_Partials_Forms_Themes_default_Form themesDefaultForm = this;
            if (!string.IsNullOrEmpty(field.ToolTip))
            {
                themesDefaultForm.WriteLiteral("            <span");
                themesDefaultForm.BeginWriteAttribute("id", " id=\"", 7830, "\"", 7858, 2);
                themesDefaultForm.WriteAttributeValue("", 7835, field.Id, 7835, 11, false);
                themesDefaultForm.WriteAttributeValue("", 7846, "_description", 7846, 12, true);
                themesDefaultForm.EndWriteAttribute();
                themesDefaultForm.WriteLiteral(" class=\"umbraco-forms-tooltip help-block\">");
                themesDefaultForm.Write(field.ToolTip);
                themesDefaultForm.WriteLiteral("</span>\n");
            }
            themesDefaultForm.WriteLiteral("        <div class=\"umbraco-forms-field-wrapper\">\n\n            ");
            IHtmlContent htmlContent = await themesDefaultForm.Html.PartialAsync(themesDefaultForm.FormThemeResolver?.GetFieldView(themesDefaultForm.Model, field), field);
            themesDefaultForm.Write(htmlContent);
            themesDefaultForm.WriteLiteral("\n\n");
            if (themesDefaultForm.Model.ShowFieldValidaton)
                themesDefaultForm.Write(themesDefaultForm.Html.ValidationMessage(field.Name, new
                {
                    role = "alert"
                }));
            themesDefaultForm.WriteLiteral("\n        </div>\n");
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