
// Type: Umbraco.Forms.Web.TagHelpers.RenderFormScriptsTagHelper
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.ViewComponents;


#nullable enable
namespace Umbraco.Forms.Web.TagHelpers
{
  [HtmlTargetElement("umb-forms-render-scripts")]
  public class RenderFormScriptsTagHelper : TagHelper
  {
    private readonly IViewComponentHelper _viewComponentHelper;
    private readonly PackageOptionSettings _packageOptionSettings;

    public RenderFormScriptsTagHelper(
      IViewComponentHelper viewComponentHelper,
      IOptions<PackageOptionSettings> packageOptionSettings)
    {
      this._viewComponentHelper = viewComponentHelper;
      this._packageOptionSettings = packageOptionSettings.Value;
    }

    [HtmlAttributeName("form-id")]
    public Guid? FormId { get; set; }

    [HtmlAttributeName("theme")]
    public string? Theme { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
      ((IViewContextAware) this._viewComponentHelper).Contextualize(this.ViewContext);
      output.SuppressOutput();
      Guid? formId1 = this.FormId;
      if (formId1.HasValue)
      {
        Guid valueOrDefault = formId1.GetValueOrDefault();
        await this.RenderFormScriptsAsync(output, valueOrDefault);
      }
      else
      {
        if (this._packageOptionSettings.TrackRenderedFormsStorageMethod == TrackRenderedFormsStorageMethodOption.TempData)
        {
          IEnumerable<Guid> guids = (IEnumerable<Guid>) this.ViewContext.TempData.Get<List<Guid>>("UmbracoForms");
          if (guids != null)
          {
            ((IDictionary<string, object>) this.ViewContext.TempData).Remove("UmbracoForms");
            foreach (Guid formId2 in guids)
              await this.RenderFormScriptsAsync(output, formId2);
            return;
          }
        }
        object obj;
        if (this._packageOptionSettings.TrackRenderedFormsStorageMethod != TrackRenderedFormsStorageMethodOption.HttpContextItems || !this.ViewContext.HttpContext.Items.TryGetValue((object) "UmbracoForms", out obj) || !(obj is IEnumerable<Guid> guids1))
          return;
        foreach (Guid formId3 in guids1)
          await this.RenderFormScriptsAsync(output, formId3);
      }
    }

    private async Task RenderFormScriptsAsync(TagHelperOutput output, Guid formId) => output.PostElement.AppendHtml(await this._viewComponentHelper.InvokeAsync(typeof (RenderFormScriptsViewComponent), (object) new
    {
      formId = formId,
      theme = this.Theme
    }));
  }
}
