using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    public class RazorEmailViewController : Controller
    {
        public IActionResult Index() => Content(string.Empty);
    }
}