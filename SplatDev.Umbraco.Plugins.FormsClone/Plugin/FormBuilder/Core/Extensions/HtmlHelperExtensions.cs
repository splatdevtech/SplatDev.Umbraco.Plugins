using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Web.Helpers;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace FormBuilder.Core.Extensions
{
    /// <summary>
    /// Provides extension methods on     /// </summary>
    public static class HtmlHelperExtensions
    {
        private const string CssKey = "form.css.";
        private const string WrapperCssKey = "form.css.wrapper.";
        private const string DefaultEditor = "defaultEditor";
        private const string DefaultWrapper = "defaultWrapper";
        private const string FormsScriptFiles = "__formsScriptFiles";
        internal const string FormsScriptsRenderTracker = "__formsScriptsRendered";

        private static readonly string[] _formScriptDependencies =
        [
          "~/App_Plugins/FormBuilder/assets/aspnet-client-validation/dist/aspnet-validation.min.js",
          "~/App_Plugins/FormBuilder/assets/json-logic-js/logic.min.js"
        ];

        private const string FormsStylesheetFiles = "__formsCssFiles";
        internal const string FormsStylesheetRenderTracker = "__formsCssRendered";

        /// <summary>
        /// Adds a class to the form field html element of a given type.
        /// If no type is given, it will add the class to all fields.
        /// </summary>
        public static IHtmlContent SetFormFieldClass(
          this IHtmlHelper htmlHelper,
          string className,
          string editor = "defaultEditor")
        {
            htmlHelper.ViewData.Add("form.css." + editor, className);
            return HtmlString.Empty;
        }

        /// <summary>
        /// Retrieves all classes for a given field type, used when rendering form fields.
        /// </summary>
        public static IHtmlContent GetFormFieldClass(
          this IHtmlHelper htmlHelper,
          string? editor = "defaultEditor")
        {
            return (htmlHelper.ViewData.TryGetValue("form.css." + editor, out object? obj) || htmlHelper.ViewData.TryGetValue("form.css.defaultEditor", out obj)) && obj is not null ? new HtmlString(obj.ToString()) : (IHtmlContent)HtmlString.Empty;
        }

        /// <summary>
        /// Retrieves all wrapper classes for a given field type, used when rendering form fields.
        /// This class wraps both label, help-text and the field itself.
        /// </summary>
        public static IHtmlContent GetFormFieldWrapperClass(
          this IHtmlHelper htmlHelper,
          string editor = "defaultWrapper")
        {
            return (htmlHelper.ViewData.TryGetValue("form.css.wrapper." + editor, out object? obj) || htmlHelper.ViewData.TryGetValue("form.css.wrapper.defaultWrapper", out obj)) && obj is not null ? new HtmlString(obj.ToString()) : (IHtmlContent)HtmlString.Empty;
        }

        /// <summary>
        /// Adds a class to the div element wrapping around form fields of a given type.
        /// If no type is given, it will add the class to all fields.
        /// </summary>
        public static IHtmlContent SetFormFieldWrapperClass(
          this IHtmlHelper htmlHelper,
          string className,
          string editor = "defaultWrapper")
        {
            htmlHelper.ViewData.Add("form.css.wrapper." + editor, className);
            return HtmlString.Empty;
        }

        private static Dictionary<string, string> GetViewFileList(
          IHtmlHelper htmlHelper,
          string key)
        {
            if (!htmlHelper.ViewContext.HttpContext.Items.TryGetValue(key, out object? obj) || obj is not Dictionary<string, string> viewFileList)
                htmlHelper.ViewContext.HttpContext.Items[key] = viewFileList = [];
            return viewFileList;
        }

        private static HashSet<string> GetRenderedFiles(IHtmlHelper htmlHelper, string key)
        {
            if (!htmlHelper.ViewContext.HttpContext.Items.TryGetValue(key, out object? obj) || obj is not HashSet<string> renderedFiles)
                htmlHelper.ViewContext.HttpContext.Items[key] = renderedFiles = [];
            return renderedFiles;
        }

        private static bool FileIsNotAlreadyRendered(
          KeyValuePair<string, string> x,
          HashSet<string> styleSheetsRendered)
        {
            return !styleSheetsRendered.Contains(x.Key);
        }

        private static void TrackRenderedFile(
          IHtmlHelper html,
          HashSet<string> filesRendered,
          string contextItemsKey)
        {
            html.ViewContext.HttpContext.Items[contextItemsKey] = filesRendered;
        }

        private static string CreateVersionedUrl(
          IUrlHelper urlHelper,
          string scriptSrc,
          string installedPackageVersion)
        {
            string contentPath = scriptSrc + (scriptSrc.Contains('?') ? "&" : "?") + "v=" + installedPackageVersion;
            return urlHelper.Content(contentPath);
        }

        /// <summary>Add a JavaScript file path to include on form render.</summary>
        public static void AddFormThemeScriptFile(this IHtmlHelper htmlHelper, string path) => GetViewFileList(htmlHelper, "__formsScriptFiles")[path] = path;

        /// <summary>
        /// Returns a list of script files to include for the current form page being rendered.
        /// </summary>
        public static Dictionary<string, string> GetThemeScriptFiles(
          this IHtmlHelper htmlHelper,
          FormViewModel model)
        {
            Dictionary<string, string> viewFileList = GetViewFileList(htmlHelper, "__formsScriptFiles");
            if (model.CurrentPage is not null)
                viewFileList.MergeLeft(model.CurrentPage.JavascriptFiles);
            return viewFileList;
        }

        /// <summary>
        /// Renders the script tags for the forms scripts and ensures they are only output once per page.
        /// </summary>
        public static IHtmlContent RenderFormsScripts(
          this IHtmlHelper htmlHelper,
          IUrlHelper urlHelper,
          FormViewModel model,
          object? htmlAttributes = null)
        {
            HashSet<string> scriptsRendered = GetRenderedFiles(htmlHelper, "__formsScriptsRendered");
            IDictionary<string, object?> htmlAttributes1 = (IDictionary<string, object?>)(htmlAttributes is not IDictionary<string, string> source ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) : source.ToDictionary<KeyValuePair<string, string>, string, object>(x => x.Key, x => x.Value));
            HtmlContentBuilder htmlContentBuilder = new();
            string installedPackageVersion = VersionHelper.GetInstalledPackageVersion();
            foreach (KeyValuePair<string, string> keyValuePair in htmlHelper.GetThemeScriptFiles(model).Where(x => FileIsNotAlreadyRendered(x, scriptsRendered)))
            {
                TagBuilder tagBuilder = new("script");
                tagBuilder.Attributes.Add("type", "application/javascript");
                tagBuilder.Attributes.Add("src", CreateVersionedUrl(urlHelper, keyValuePair.Value, installedPackageVersion));
                tagBuilder.MergeAttributes(htmlAttributes1);
                htmlContentBuilder.AppendHtml(tagBuilder);
                scriptsRendered.Add(keyValuePair.Key);
            }
            TrackRenderedFile(htmlHelper, scriptsRendered, "__formsScriptsRendered");
            return new HtmlString(htmlContentBuilder.ToHtmlString());
        }

        /// <summary>Renders the Umbraco form dependencies.</summary>
        public static IHtmlContent RenderFormBuilderDependencies(
          this IHtmlHelper htmlHelper,
          IUrlHelper urlHelper,
          object? htmlAttributes = null)
        {
            return htmlHelper.RenderFormBuilderDependencies(urlHelper, StaticServiceProvider.Instance.GetRequiredService<IOptions<PackageOptionSettings>>().Value, htmlAttributes);
        }

        /// <summary>Renders the Umbraco form dependencies.</summary>
        public static IHtmlContent RenderFormBuilderDependencies(
          this IHtmlHelper htmlHelper,
          IUrlHelper urlHelper,
          PackageOptionSettings packageOptionSettings,
          object? htmlAttributes = null)
        {
            ArgumentNullException.ThrowIfNull(htmlHelper);

            IDictionary<string, object?> htmlAttributes1 = AnonymousObjectToHtmlAttributes(htmlAttributes);
            HtmlContentBuilder htmlContentBuilder = new();
            string installedPackageVersion = VersionHelper.GetInstalledPackageVersion();
            foreach (string scriptDependency in _formScriptDependencies)
            {
                if (packageOptionSettings.EnableAdvancedValidationRules || !scriptDependency.Contains("json-logic-js"))
                {
                    TagBuilder tagBuilder = new("script");
                    tagBuilder.Attributes.Add("type", "application/javascript");
                    tagBuilder.Attributes.Add("src", CreateVersionedUrl(urlHelper, scriptDependency, installedPackageVersion));
                    tagBuilder.MergeAttributes(htmlAttributes1);
                    htmlContentBuilder.AppendHtml(tagBuilder);
                }
            }
            return htmlContentBuilder;
        }

        /// <summary>
        /// Set the primary form theme stylesheet path.
        /// This overrides an already assigned stylesheet.
        /// </summary>
        public static void SetFormThemeCssFile(this IHtmlHelper htmlHelper, string path) => GetViewFileList(htmlHelper, "__formsCssFiles")["default"] = path;

        /// <summary>Add a CSS file path to include on form render.</summary>
        public static void AddFormThemeCssFile(this IHtmlHelper htmlHelper, string path) => GetViewFileList(htmlHelper, "__formsCssFiles")[path] = path;

        /// <summary>
        /// Returns a list of stylesheets to include in the current form page being rendered.
        /// </summary>
        public static Dictionary<string, string> GetThemeCssFiles(
          this IHtmlHelper htmlHelper,
          FormViewModel model)
        {
            Dictionary<string, string> viewFileList = GetViewFileList(htmlHelper, "__formsCssFiles");
            if (model.CurrentPage is not null)
                viewFileList.MergeLeft(model.CurrentPage.CssFiles);
            return viewFileList;
        }

        /// <summary>
        /// Renders the script tags for the forms stylesheets and ensure they are only output once per page.
        /// </summary>
        public static IHtmlContent RenderFormsStylesheets(
          this IHtmlHelper htmlHelper,
          IUrlHelper urlHelper,
          FormViewModel model,
          object? htmlAttributes = null)
        {
            HashSet<string> styleSheetsRendered = GetRenderedFiles(htmlHelper, "__formsCssRendered");
            IDictionary<string, object?> htmlAttributes1 = AnonymousObjectToHtmlAttributes(htmlAttributes);
            HtmlContentBuilder htmlContentBuilder = new();
            string installedPackageVersion = VersionHelper.GetInstalledPackageVersion();
            foreach (KeyValuePair<string, string> keyValuePair in htmlHelper.GetThemeCssFiles(model).Where(x => FileIsNotAlreadyRendered(x, styleSheetsRendered)))
            {
                TagBuilder tagBuilder = new("link")
                {
                    TagRenderMode = TagRenderMode.SelfClosing
                };
                tagBuilder.Attributes.Add("rel", "stylesheet");
                tagBuilder.Attributes.Add("href", CreateVersionedUrl(urlHelper, keyValuePair.Value, installedPackageVersion));
                tagBuilder.MergeAttributes(htmlAttributes1);
                htmlContentBuilder.AppendHtml(tagBuilder);
                styleSheetsRendered.Add(keyValuePair.Key);
            }
            TrackRenderedFile(htmlHelper, styleSheetsRendered, "__formsCssRendered");
            return new HtmlString(htmlContentBuilder.ToHtmlString());
        }

        private static IDictionary<string, object?> AnonymousObjectToHtmlAttributes(object? htmlAttributes)
        {
            return htmlAttributes is IDictionary<string, string> source ? source.ToDictionary<KeyValuePair<string, string>, string, object>(x => x.Key, x => x.Value) : HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }
    }
}