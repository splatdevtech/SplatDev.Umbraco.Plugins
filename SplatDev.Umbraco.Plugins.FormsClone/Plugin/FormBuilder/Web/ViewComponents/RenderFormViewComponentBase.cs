using FormBuilder.Core.Services;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.ViewComponents
{
    public abstract class RenderFormViewComponentBase(
      IFormRenderingService formRenderingService,
      IFormThemeResolver formThemeResolver) : ViewComponent
    {
        protected IFormRenderingService FormRenderingService { get; } = formRenderingService;

        protected IFormThemeResolver FormThemeResolver { get; } = formThemeResolver;
    }
}