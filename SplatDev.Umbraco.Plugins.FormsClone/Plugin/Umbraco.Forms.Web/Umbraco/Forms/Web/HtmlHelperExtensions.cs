
// Type: Umbraco.Forms.Web.HtmlHelperExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Helpers;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace Umbraco.Forms.Web
{
  public static class HtmlHelperExtensions
  {
    private const string CssKey = "form.css.";
    private const string WrapperCssKey = "form.css.wrapper.";
    private const string DefaultEditor = "defaultEditor";
    private const string DefaultWrapper = "defaultWrapper";
    private const string FormsScriptFiles = "__formsScriptFiles";
    internal const string FormsScriptsRenderTracker = "__formsScriptsRendered";
    private static readonly string[] _formScriptDependencies = new string[2]
    {
      "~/App_Plugins/UmbracoForms/assets/aspnet-client-validation/dist/aspnet-validation.min.js",
      "~/App_Plugins/UmbracoForms/assets/json-logic-js/logic.min.js"
    };
    private const string FormsStylesheetFiles = "__formsCssFiles";
    internal const string FormsStylesheetRenderTracker = "__formsCssRendered";

    public static IHtmlContent SetFormFieldClass(
      this IHtmlHelper htmlHelper,
      string className,
      string editor = "defaultEditor")
    {
      htmlHelper.ViewData.Add("form.css." + editor, (object) className);
      return (IHtmlContent) HtmlString.Empty;
    }

    public static IHtmlContent GetFormFieldClass(
      this IHtmlHelper htmlHelper,
      string editor = "defaultEditor")
    {
      object obj;
      return (htmlHelper.ViewData.TryGetValue("form.css." + editor, out obj) || htmlHelper.ViewData.TryGetValue("form.css.defaultEditor", out obj)) && obj != null ? (IHtmlContent) new HtmlString(obj.ToString()) : (IHtmlContent) HtmlString.Empty;
    }

    public static IHtmlContent GetFormFieldWrapperClass(
      this IHtmlHelper htmlHelper,
      string editor = "defaultWrapper")
    {
      object obj;
      return (htmlHelper.ViewData.TryGetValue("form.css.wrapper." + editor, out obj) || htmlHelper.ViewData.TryGetValue("form.css.wrapper.defaultWrapper", out obj)) && obj != null ? (IHtmlContent) new HtmlString(obj.ToString()) : (IHtmlContent) HtmlString.Empty;
    }

    public static IHtmlContent SetFormFieldWrapperClass(
      this IHtmlHelper htmlHelper,
      string className,
      string editor = "defaultWrapper")
    {
      htmlHelper.ViewData.Add("form.css.wrapper." + editor, (object) className);
      return (IHtmlContent) HtmlString.Empty;
    }

    private static Dictionary<string, string> GetViewFileList(
      IHtmlHelper htmlHelper,
      string key)
    {
      object obj;
      if (!htmlHelper.ViewContext.HttpContext.Items.TryGetValue((object) key, out obj) || !(obj is Dictionary<string, string> viewFileList))
        htmlHelper.ViewContext.HttpContext.Items[(object) key] = (object) (viewFileList = new Dictionary<string, string>());
      return viewFileList;
    }

    private static HashSet<string> GetRenderedFiles(IHtmlHelper htmlHelper, string key)
    {
      object obj;
      if (!htmlHelper.ViewContext.HttpContext.Items.TryGetValue((object) key, out obj) || !(obj is HashSet<string> renderedFiles))
        htmlHelper.ViewContext.HttpContext.Items[(object) key] = (object) (renderedFiles = new HashSet<string>());
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
      html.ViewContext.HttpContext.Items[(object) contextItemsKey] = (object) filesRendered;
    }

    private static IDictionary<string, object?> AnonymousObjectToHtmlAttributes(
      object? htmlAttributes)
    {
      return !(htmlAttributes is IDictionary<string, string> source) ? HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes) : (IDictionary<string, object>) source.ToDictionary<KeyValuePair<string, string>, string, object>((Func<KeyValuePair<string, string>, string>) (x => x.Key), (Func<KeyValuePair<string, string>, object>) (x => (object) x.Value));
    }

    private static string CreateVersionedUrl(
      IUrlHelper urlHelper,
      string scriptSrc,
      string installedPackageVersion)
    {
      string contentPath = scriptSrc + (scriptSrc.Contains("?") ? "&" : "?") + "v=" + installedPackageVersion;
      return urlHelper.Content(contentPath);
    }

    public static void AddFormThemeScriptFile(this IHtmlHelper htmlHelper, string path) => HtmlHelperExtensions.GetViewFileList(htmlHelper, "__formsScriptFiles")[path] = path;

    public static Dictionary<string, string> GetThemeScriptFiles(
      this IHtmlHelper htmlHelper,
      FormViewModel model)
    {
      Dictionary<string, string> viewFileList = HtmlHelperExtensions.GetViewFileList(htmlHelper, "__formsScriptFiles");
      if (model.CurrentPage != null)
        viewFileList.MergeLeft<Dictionary<string, string>, string, string>((IDictionary<string, string>) model.CurrentPage.JavascriptFiles);
      return viewFileList;
    }

    public static IHtmlContent RenderFormsScripts(
      this IHtmlHelper htmlHelper,
      IUrlHelper urlHelper,
      FormViewModel model,
      object? htmlAttributes = null)
    {
      HashSet<string> scriptsRendered = HtmlHelperExtensions.GetRenderedFiles(htmlHelper, "__formsScriptsRendered");
      IDictionary<string, object> htmlAttributes1 = HtmlHelperExtensions.AnonymousObjectToHtmlAttributes(htmlAttributes);
      HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
      string installedPackageVersion = VersionHelper.GetInstalledPackageVersion();
      foreach (KeyValuePair<string, string> keyValuePair in htmlHelper.GetThemeScriptFiles(model).Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => HtmlHelperExtensions.FileIsNotAlreadyRendered(x, scriptsRendered))))
      {
        TagBuilder tagBuilder = new TagBuilder("script");
        tagBuilder.Attributes.Add("type", "application/javascript");
        tagBuilder.Attributes.Add("src", HtmlHelperExtensions.CreateVersionedUrl(urlHelper, keyValuePair.Value, installedPackageVersion));
        tagBuilder.MergeAttributes<string, object>(htmlAttributes1);
        htmlContentBuilder.AppendHtml((IHtmlContent) tagBuilder);
        scriptsRendered.Add(keyValuePair.Key);
      }
      HtmlHelperExtensions.TrackRenderedFile(htmlHelper, scriptsRendered, "__formsScriptsRendered");
      return (IHtmlContent) new HtmlString(HtmlContentExtensions.ToHtmlString((IHtmlContent) htmlContentBuilder));
    }

    public static IHtmlContent RenderUmbracoFormDependencies(
      this IHtmlHelper htmlHelper,
      IUrlHelper urlHelper,
      object? htmlAttributes = null)
    {
      return htmlHelper.RenderUmbracoFormDependencies(urlHelper, ServiceProviderServiceExtensions.GetRequiredService<IOptions<PackageOptionSettings>>(StaticServiceProvider.Instance).Value, htmlAttributes);
    }

    public static IHtmlContent RenderUmbracoFormDependencies(
      this IHtmlHelper htmlHelper,
      IUrlHelper urlHelper,
      PackageOptionSettings packageOptionSettings,
      object? htmlAttributes = null)
    {
      IDictionary<string, object> htmlAttributes1 = HtmlHelperExtensions.AnonymousObjectToHtmlAttributes(htmlAttributes);
      HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
      string installedPackageVersion = VersionHelper.GetInstalledPackageVersion();
      foreach (string scriptDependency in HtmlHelperExtensions._formScriptDependencies)
      {
        if (packageOptionSettings.EnableAdvancedValidationRules || !scriptDependency.Contains("json-logic-js"))
        {
          TagBuilder tagBuilder = new TagBuilder("script");
          tagBuilder.Attributes.Add("type", "application/javascript");
          tagBuilder.Attributes.Add("src", HtmlHelperExtensions.CreateVersionedUrl(urlHelper, scriptDependency, installedPackageVersion));
          tagBuilder.MergeAttributes<string, object>(htmlAttributes1);
          htmlContentBuilder.AppendHtml((IHtmlContent) tagBuilder);
        }
      }
      return (IHtmlContent) htmlContentBuilder;
    }

    public static void SetFormThemeCssFile(this IHtmlHelper htmlHelper, string path) => HtmlHelperExtensions.GetViewFileList(htmlHelper, "__formsCssFiles")["default"] = path;

    public static void AddFormThemeCssFile(this IHtmlHelper htmlHelper, string path) => HtmlHelperExtensions.GetViewFileList(htmlHelper, "__formsCssFiles")[path] = path;

    public static Dictionary<string, string> GetThemeCssFiles(
      this IHtmlHelper htmlHelper,
      FormViewModel model)
    {
      Dictionary<string, string> viewFileList = HtmlHelperExtensions.GetViewFileList(htmlHelper, "__formsCssFiles");
      if (model.CurrentPage != null)
        viewFileList.MergeLeft<Dictionary<string, string>, string, string>((IDictionary<string, string>) model.CurrentPage.CssFiles);
      return viewFileList;
    }

    public static IHtmlContent RenderFormsStylesheets(
      this IHtmlHelper htmlHelper,
      IUrlHelper urlHelper,
      FormViewModel model,
      object? htmlAttributes = null)
    {
      HashSet<string> styleSheetsRendered = HtmlHelperExtensions.GetRenderedFiles(htmlHelper, "__formsCssRendered");
      IDictionary<string, object> htmlAttributes1 = HtmlHelperExtensions.AnonymousObjectToHtmlAttributes(htmlAttributes);
      HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
      string installedPackageVersion = VersionHelper.GetInstalledPackageVersion();
      foreach (KeyValuePair<string, string> keyValuePair in htmlHelper.GetThemeCssFiles(model).Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => HtmlHelperExtensions.FileIsNotAlreadyRendered(x, styleSheetsRendered))))
      {
        TagBuilder tagBuilder = new TagBuilder("link")
        {
          TagRenderMode = TagRenderMode.SelfClosing
        };
        tagBuilder.Attributes.Add("rel", "stylesheet");
        tagBuilder.Attributes.Add("href", HtmlHelperExtensions.CreateVersionedUrl(urlHelper, keyValuePair.Value, installedPackageVersion));
        tagBuilder.MergeAttributes<string, object>(htmlAttributes1);
        htmlContentBuilder.AppendHtml((IHtmlContent) tagBuilder);
        styleSheetsRendered.Add(keyValuePair.Key);
      }
      HtmlHelperExtensions.TrackRenderedFile(htmlHelper, styleSheetsRendered, "__formsCssRendered");
      return (IHtmlContent) new HtmlString(HtmlContentExtensions.ToHtmlString((IHtmlContent) htmlContentBuilder));
    }
  }
}
