using FormBuilder.Core.Models;
using FormBuilder.Core.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.ViewComponents
{
    public class RenderFormScriptsViewComponent(
      IFormRenderingService formRenderingService,
      IFormThemeResolver formThemeResolver) : RenderFormViewComponentBase(formRenderingService, formThemeResolver)
    {
        public async Task<IViewComponentResult> InvokeAsync(
          Guid formId,
          string theme = "")
        {
            RenderFormScriptsViewComponent scriptsViewComponent = this;
            IFormRenderingService renderingService = scriptsViewComponent.FormRenderingService;
            HttpContext httpContext = scriptsViewComponent.HttpContext;
            Guid formId1 = formId;
            string str = theme;
            Guid? recordId = new Guid?();
            string theme1 = str;
            FormViewModel formModelAsync = await renderingService.GetFormModelAsync(httpContext, formId1, recordId, theme1);
            string? scriptView = scriptsViewComponent.FormThemeResolver.GetScriptView(formModelAsync);
            return scriptsViewComponent.View(scriptView, formModelAsync);
        }
    }
}