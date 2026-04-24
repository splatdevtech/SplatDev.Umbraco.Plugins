using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with themes.
    /// </summary>
    [Authorize(Policy = "BackOfficeAccess")]
    [ApiExplorerSettings(GroupName = "Theme")]
    [Route("/formBuilder/management/api/v1/theme")]
    public abstract class ThemeControllerBase : FormsManagementApiControllerBase
    {
    }
}