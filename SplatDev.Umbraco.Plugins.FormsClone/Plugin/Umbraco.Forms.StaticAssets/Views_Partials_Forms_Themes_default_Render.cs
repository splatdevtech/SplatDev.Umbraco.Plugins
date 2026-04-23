using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using Umbraco.Extensions;
using Umbraco.Forms.Web.Controllers;
using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.Services;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "10cd1000e69306e1319da0a3d02ddd6ff2fbd774a41dc11ac8df3a2a08c23ff1", "/Views/Partials/Forms/Themes/default/Render.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Themes/default/Render.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Themes_default_Render :
        RazorPage<FormViewModel>
    {
        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Themes_default_Render themesDefaultRender = this;
            themesDefaultRender.WriteLiteral("\n");
            string? formView = themesDefaultRender.FormThemeResolver?.GetFormView(themesDefaultRender.Model);
            string? formScriptView = themesDefaultRender.FormThemeResolver?.GetScriptView(themesDefaultRender.Model);
            themesDefaultRender.WriteLiteral("\n");
            if (themesDefaultRender.Model.SubmitHandled)
            {
                await themesDefaultRender.Html.RenderPartialAsync(formView);
            }
            else
            {
                themesDefaultRender.WriteLiteral("    <div");
                themesDefaultRender.BeginWriteAttribute("id", " id=\"", 1121, "\"", 1158, 2);
                themesDefaultRender.WriteAttributeValue("", 1126, "umbraco_form_", 1126, 13, true);
                themesDefaultRender.WriteAttributeValue("", 1139, themesDefaultRender.Model.FormClientId, 1139, 19, false);
                themesDefaultRender.EndWriteAttribute();
                themesDefaultRender.BeginWriteAttribute("class", " class=\"", 1159, "\"", 1228, 4);
                themesDefaultRender.WriteAttributeValue("", 1167, "umbraco-forms-form", 1167, 18, true);
                themesDefaultRender.WriteAttributeValue(" ", 1185, themesDefaultRender.Model.CssClass, 1186, 15, false);
                themesDefaultRender.WriteAttributeValue(" ", 1201, "umbraco-forms-", 1202, 15, true);
                themesDefaultRender.WriteAttributeValue("", 1216, themesDefaultRender.Model.Theme, 1216, 12, false);
                themesDefaultRender.EndWriteAttribute();
                themesDefaultRender.WriteLiteral(">\n");
                using (themesDefaultRender.Html?.BeginUmbracoForm<UmbracoFormsController>("HandleForm", null, themesDefaultRender.Model.HtmlAttributes, FormMethod.Post, new bool?(themesDefaultRender.Model.RenderAntiForgeryToken)))
                {
                    themesDefaultRender.Write(themesDefaultRender.Html?.HiddenFor((Expression<Func<FormViewModel, Guid>>)(x => themesDefaultRender.Model.FormId), new
                    {
                        id = (string?)null
                    }));
                    themesDefaultRender.Write(themesDefaultRender.Html?.HiddenFor((Expression<Func<FormViewModel, string>>)(x => themesDefaultRender.Model.FormName), new
                    {
                        id = (string?)null
                    }));
                    themesDefaultRender.Write(themesDefaultRender.Html?.HiddenFor((Expression<Func<FormViewModel, Guid>>)(x => themesDefaultRender.Model.RecordId), new
                    {
                        id = (string?)null
                    }));
                    themesDefaultRender.Write(themesDefaultRender.Html?.HiddenFor((Expression<Func<FormViewModel, string>>)(x => themesDefaultRender.Model.PreviousClicked ?? ""), new
                    {
                        id = (string?)null
                    }));
                    themesDefaultRender.Write(themesDefaultRender.Html?.HiddenFor((Expression<Func<FormViewModel, string>>)(x => themesDefaultRender.Model.Theme ?? ""), new
                    {
                        id = (string?)null
                    }));
                    themesDefaultRender.Write(themesDefaultRender.Html?.HiddenFor((Expression<Func<FormViewModel, Guid?>>)(x => themesDefaultRender.Model.RedirectToPageId), new
                    {
                        id = (string?)null
                    }));
                    themesDefaultRender.WriteLiteral("            <input type=\"hidden\" name=\"FormStep\"");
                    themesDefaultRender.BeginWriteAttribute("value", " value=\"", 2051, "\"", 2074, 1);
                    themesDefaultRender.WriteAttributeValue("", 2059, themesDefaultRender.Model.FormStep, 2059, 15, false);
                    themesDefaultRender.EndWriteAttribute();
                    themesDefaultRender.WriteLiteral(" />\n            <input type=\"hidden\" name=\"RecordState\"");
                    themesDefaultRender.BeginWriteAttribute("value", " value=\"", 2130, "\"", 2156, 1);
                    themesDefaultRender.WriteAttributeValue("", 2138, themesDefaultRender.Model.RecordState, 2138, 18, false);
                    themesDefaultRender.EndWriteAttribute();
                    themesDefaultRender.WriteLiteral(" />\n");
                    if (!string.IsNullOrEmpty(themesDefaultRender.Model.AdditionalData))
                    {
                        themesDefaultRender.WriteLiteral("                <input type=\"hidden\" name=\"AdditionalData\"");
                        themesDefaultRender.BeginWriteAttribute("value", " value=\"", 2295, "\"", 2324, 1);
                        themesDefaultRender.WriteAttributeValue("", 2303, themesDefaultRender.Model.AdditionalData, 2303, 21, false);
                        themesDefaultRender.EndWriteAttribute();
                        themesDefaultRender.WriteLiteral(" />\n");
                    }
                    await themesDefaultRender.Html.RenderPartialAsync(formView);
                }
                themesDefaultRender.WriteLiteral("    </div>\n");
            }
            themesDefaultRender.WriteLiteral("\n");
            if (!themesDefaultRender.Model.RenderScriptFiles)
            {
                formScriptView = null;
            }
            else
            {
                await themesDefaultRender.Html.RenderPartialAsync(formScriptView);
                formScriptView = null;
            }
            await Task.CompletedTask;
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