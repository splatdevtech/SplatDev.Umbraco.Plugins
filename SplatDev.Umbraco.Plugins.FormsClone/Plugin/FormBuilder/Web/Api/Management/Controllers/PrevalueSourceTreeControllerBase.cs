using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with the prevalue sources tree.
    /// </summary>
    [ApiExplorerSettings(GroupName = "Prevalue Source")]
    [Route("/formBuilder/management/api/v1/tree/prevalue-source")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManagePrevalueSources")]
    public abstract class PrevalueSourceTreeControllerBase : FormsManagementApiControllerBase
    {
    }
}