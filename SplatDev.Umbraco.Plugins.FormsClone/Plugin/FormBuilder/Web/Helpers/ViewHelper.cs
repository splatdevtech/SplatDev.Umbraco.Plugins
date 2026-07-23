using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace FormBuilder.Web.Helpers
{
    public class ViewHelper
    {
        /// <summary>Renders a partial view to a string.</summary>
        public static async Task<string> RenderPartialViewToString(
          HttpContext httpContext,
          string filePath,
          object model)
        {
            FakeController? controller = new();
            ControllerContext? controllerContext = new()
            {
                ActionDescriptor = new ControllerActionDescriptor(),
                HttpContext = httpContext,
                RouteData = new RouteData()
            };
            controller.ControllerContext = controllerContext;
            return await controller.RenderViewAsync(filePath, model);
        }

        private class FakeController : Controller
        {
        }
    }
}