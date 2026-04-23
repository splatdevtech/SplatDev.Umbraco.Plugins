using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with pickers.
    /// </summary>
    [ApiExplorerSettings(GroupName = "Picker")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/picker")]
    public abstract class PickerControllerBase : FormsManagementApiControllerBase
    {
    }
}