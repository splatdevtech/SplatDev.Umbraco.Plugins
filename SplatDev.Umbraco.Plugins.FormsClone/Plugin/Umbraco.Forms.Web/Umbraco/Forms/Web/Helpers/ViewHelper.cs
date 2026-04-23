
// Type: Umbraco.Forms.Web.Helpers.ViewHelper
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;
using Umbraco.Forms.Web.Extensions;


#nullable enable
namespace Umbraco.Forms.Web.Helpers
{
  public class ViewHelper
  {
    public static async Task<string> RenderPartialViewToString(
      HttpContext httpContext,
      string filePath,
      object model)
    {
      ViewHelper.FakeController controller = new ViewHelper.FakeController();
      ControllerContext controllerContext = new ControllerContext();
      controllerContext.ActionDescriptor = new ControllerActionDescriptor();
      controllerContext.HttpContext = httpContext;
      controllerContext.RouteData = new RouteData();
      controller.ControllerContext = controllerContext;
      return await controller.RenderViewAsync<object>(filePath, model);
    }

    private class FakeController : Controller
    {
    }
  }
}
