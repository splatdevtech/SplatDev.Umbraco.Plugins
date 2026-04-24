using FormBuilder.Core.Extensions;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Runtime.CompilerServices;
using System.Text.Json;

namespace FormBuilder.StaticAssets
{
    [RazorSourceChecksum("Sha256", "72624c998cc1b5de769ee71c302b3d9ed8bb373b57cc17855816d8d5f0176802", "/Views/Partials/Forms/Themes/default/Script.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Script.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Script :
        RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Script themesDefaultScript = this;
            themesDefaultScript.WriteLiteral("\n");
            themesDefaultScript.WriteLiteral("\n");
            themesDefaultScript.Html?.AddFormThemeScriptFile("~/App_Plugins/FormBuilder/assets/themes/default/FormBuilders.min.js");
            themesDefaultScript.WriteLiteral("\n<div class=\"umbraco-forms-form-config umbraco-forms-hidden\"\n     data-id=\"");
            themesDefaultScript.Write(themesDefaultScript.Model.FormClientId);
            themesDefaultScript.WriteLiteral("\"\n     data-serialized-page-button-conditions=\"");
            themesDefaultScript.Write(JsonSerializer.Serialize(themesDefaultScript.Model.PageButtonConditions, FormsJsonSerializerOptions.Default));
            themesDefaultScript.WriteLiteral("\"\n     data-serialized-fieldset-conditions=\"");
            themesDefaultScript.Write(JsonSerializer.Serialize(themesDefaultScript.Model.FieldsetConditions, FormsJsonSerializerOptions.Default));
            themesDefaultScript.WriteLiteral("\"\n     data-serialized-field-conditions=\"");
            themesDefaultScript.Write(JsonSerializer.Serialize(themesDefaultScript.Model.FieldConditions, FormsJsonSerializerOptions.Default));
            themesDefaultScript.WriteLiteral("\"\n     data-serialized-fields-not-displayed=\"");
            themesDefaultScript.Write(JsonSerializer.Serialize(themesDefaultScript.Model.GetFieldsNotDisplayed(themesDefaultScript.Model.FormElementHtmlIdPrefix), FormsJsonSerializerOptions.Default));
            themesDefaultScript.WriteLiteral("\"\n     data-trigger-conditions-check-on=\"");
            themesDefaultScript.Write(themesDefaultScript.Model.TriggerConditionsCheckOn);
            themesDefaultScript.WriteLiteral("\"\n     data-form-element-html-id-prefix=\"");
            themesDefaultScript.Write(themesDefaultScript.Model.FormElementHtmlIdPrefix);
            themesDefaultScript.WriteLiteral("\"\n     data-disable-validation-dependency-check=\"");
            themesDefaultScript.Write(themesDefaultScript.Model.DisableClientSideValidationDependencyCheck.ToString().ToLower());
            themesDefaultScript.WriteLiteral("\"\n     data-serialized-validation-rules=\"");
            themesDefaultScript.Write(JsonSerializer.Serialize(themesDefaultScript.Model.ValidationRules, FormsJsonSerializerOptions.Default));
            themesDefaultScript.WriteLiteral("\"></div>\n\n");
            if (themesDefaultScript.Model.CurrentPage is not null && themesDefaultScript.Model.CurrentPage.PartialViewFiles.Count != 0)
            {
                foreach (KeyValuePair<string, string> partialViewFile in themesDefaultScript.Model.CurrentPage.PartialViewFiles)
                {
                    IHtmlContent htmlContent = await themesDefaultScript.Html.PartialAsync(partialViewFile.Value);
                    themesDefaultScript.Write(htmlContent);
                }
            }
            themesDefaultScript.WriteLiteral("\n");
            themesDefaultScript.Write(themesDefaultScript.Html?.RenderFormsScripts(themesDefaultScript.Url!, themesDefaultScript.Model, themesDefaultScript.Model.JavaScriptTagAttributes));
            themesDefaultScript.WriteLiteral("\n");
            themesDefaultScript.Write(themesDefaultScript.Html?.RenderFormsStylesheets(themesDefaultScript.Url!, themesDefaultScript.Model));
            themesDefaultScript.WriteLiteral("\n\n");
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