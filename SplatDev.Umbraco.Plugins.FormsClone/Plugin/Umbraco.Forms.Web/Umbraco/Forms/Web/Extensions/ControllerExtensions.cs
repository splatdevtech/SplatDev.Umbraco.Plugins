
// Type: Umbraco.Forms.Web.Extensions.ControllerExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


#nullable enable
namespace Umbraco.Forms.Web.Extensions
{
  public static class ControllerExtensions
  {
    public static async Task<string> RenderViewAsync<TModel>(
      this Controller controller,
      string viewPath,
      TModel model)
    {
      controller.ViewData.Model = (object) (TModel) model;
      string str;
      using (StringWriter writer = new StringWriter())
      {
        ViewEngineResult viewEngineResult = controller.HttpContext.RequestServices.GetService(typeof (ICompositeViewEngine)) is ICompositeViewEngine service ? service.GetView(viewPath, viewPath, false) : (ViewEngineResult) null;
        if (viewEngineResult == null || !viewEngineResult.Success)
        {
          DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 2);
          interpolatedStringHandler.AppendLiteral("Could not render view to string. Controller: ");
          interpolatedStringHandler.AppendFormatted(controller.GetType().FullName);
          interpolatedStringHandler.AppendLiteral(", View Path: ");
          interpolatedStringHandler.AppendFormatted(viewPath);
          interpolatedStringHandler.AppendLiteral(".");
          throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
        }
        ViewContext context = new ViewContext((ActionContext) controller.ControllerContext, viewEngineResult.View, controller.ViewData, controller.TempData, (TextWriter) writer, new HtmlHelperOptions());
        await viewEngineResult.View.RenderAsync(context);
        str = writer.GetStringBuilder().ToString();
      }
      return str;
    }
  }
}
