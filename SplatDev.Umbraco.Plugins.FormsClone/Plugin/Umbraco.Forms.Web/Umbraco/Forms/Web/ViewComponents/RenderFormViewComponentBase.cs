
// Type: Umbraco.Forms.Web.ViewComponents.RenderFormViewComponentBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Umbraco.Forms.Web.Services;


#nullable enable
namespace Umbraco.Forms.Web.ViewComponents
{
  public abstract class RenderFormViewComponentBase : ViewComponent
  {
    protected RenderFormViewComponentBase(
      IFormRenderingService formRenderingService,
      IFormThemeResolver formThemeResolver)
    {
      this.FormRenderingService = formRenderingService;
      this.FormThemeResolver = formThemeResolver;
    }

    protected IFormRenderingService FormRenderingService { get; }

    protected IFormThemeResolver FormThemeResolver { get; }
  }
}
