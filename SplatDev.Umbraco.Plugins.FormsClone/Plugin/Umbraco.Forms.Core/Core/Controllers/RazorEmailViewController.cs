
// Type: Umbraco.Forms.Core.Controllers.RazorEmailViewController
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Mvc;


#nullable enable
namespace Umbraco.Forms.Core.Controllers
{
  public class RazorEmailViewController : Controller
  {
    public IActionResult Index() => (IActionResult) this.Content(string.Empty);
  }
}
