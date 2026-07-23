
// Type: Umbraco.Forms.Web.ViewComponents.RenderFormScriptsViewComponent
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Umbraco.Forms.Web.Models;
using Umbraco.Forms.Web.Services;


#nullable enable
namespace Umbraco.Forms.Web.ViewComponents
{
  public class RenderFormScriptsViewComponent : RenderFormViewComponentBase
  {
    public RenderFormScriptsViewComponent(
      IFormRenderingService formRenderingService,
      IFormThemeResolver formThemeResolver)
      : base(formRenderingService, formThemeResolver)
    {
    }

    public async Task<IViewComponentResult> InvokeAsync(
      Guid formId,
      string theme = "")
    {
      RenderFormScriptsViewComponent scriptsViewComponent = this;
      IFormRenderingService renderingService = scriptsViewComponent.FormRenderingService;
      HttpContext httpContext = scriptsViewComponent.HttpContext;
      Guid formId1 = formId;
      string str = theme;
      Guid? recordId = null;
      string theme1 = str;
      FormViewModel formModelAsync = await renderingService.GetFormModelAsync(httpContext, formId1, recordId, theme1);
      string scriptView = scriptsViewComponent.FormThemeResolver.GetScriptView(formModelAsync);
      return (IViewComponentResult) scriptsViewComponent.View<FormViewModel>(scriptView, formModelAsync);
    }
  }
}
