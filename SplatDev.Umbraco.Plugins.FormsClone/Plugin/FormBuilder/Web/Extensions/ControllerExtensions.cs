using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

using System.Runtime.CompilerServices;

namespace FormBuilder.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<string> RenderViewAsync<TModel>(
          this Microsoft.AspNetCore.Mvc.Controller controller,
          string viewPath,
          TModel model)
        {
            controller.ViewData.Model = model;
            string str;
            using (StringWriter writer = new())
            {
                ViewEngineResult? viewEngineResult = controller.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) is ICompositeViewEngine service ? service.GetView(viewPath, viewPath, false) : null;
                if (viewEngineResult is null || !viewEngineResult.Success)
                {
                    DefaultInterpolatedStringHandler interpolatedStringHandler = new(59, 2);
                    interpolatedStringHandler.AppendLiteral("Could not render view to string. Controller: ");
                    interpolatedStringHandler.AppendFormatted(controller.GetType().FullName);
                    interpolatedStringHandler.AppendLiteral(", View Path: ");
                    interpolatedStringHandler.AppendFormatted(viewPath);
                    interpolatedStringHandler.AppendLiteral(".");
                    throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
                }
                ViewContext context = new(controller.ControllerContext, viewEngineResult.View, controller.ViewData, controller.TempData, writer, new HtmlHelperOptions());
                await viewEngineResult.View.RenderAsync(context);
                str = writer.GetStringBuilder().ToString();
            }
            return str;
        }
    }
}