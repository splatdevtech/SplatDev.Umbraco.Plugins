
// Type: Umbraco.Forms.Web.TagHelpers.RenderFormTagHelper
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.ViewComponents;


#nullable enable
namespace Umbraco.Forms.Web.TagHelpers
{
  [HtmlTargetElement("umb-forms-render")]
  public class RenderFormTagHelper : TagHelper
  {
    private readonly IViewComponentHelper _viewComponentHelper;
    private readonly IOptions<PackageOptionSettings> _packageOptionSettings;

    public RenderFormTagHelper(
      IViewComponentHelper viewComponentHelper,
      IOptions<PackageOptionSettings> packageOptionSettings)
    {
      this._viewComponentHelper = viewComponentHelper;
      this._packageOptionSettings = packageOptionSettings;
    }

    [HtmlAttributeName("form-id")]
    public Guid FormId { get; set; }

    [HtmlAttributeName("theme")]
    public string? Theme { get; set; }

    [HtmlAttributeName("record-id")]
    public Guid? RecordId { get; set; }

    [HtmlAttributeName("exclude-scripts")]
    public bool ExcludeScripts { get; set; }

    [HtmlAttributeName("redirect-to-page-id")]
    public Guid? RedirectToPageId { get; set; }

    [HtmlAttributeName("additional-data")]
    public IDictionary<string, string?>? AdditionalData { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      ((IViewContextAware) this._viewComponentHelper).Contextualize(this.ViewContext);
      output.SuppressOutput();
      Guid? nullable = this.RecordId;
      StringValues stringValues;
      Guid result;
      if (!nullable.HasValue && this._packageOptionSettings.Value.AllowEditableFormSubmissions && this.ViewContext.HttpContext.Request.Query.TryGetValue("recordId", out stringValues) && Guid.TryParse(stringValues.ToString(), out result))
        nullable = new Guid?(result);
      output.PostElement.SetHtmlContent(await this._viewComponentHelper.InvokeAsync(typeof (RenderFormViewComponent), (object) new
      {
        formId = this.FormId,
        recordId = nullable,
        theme = this.Theme,
        includeScripts = !this.ExcludeScripts,
        redirectToPageId = this.RedirectToPageId,
        additionalData = this.AdditionalData
      }));
    }
  }
}
