using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with the datas ources tree.
    /// </summary>
    [ApiExplorerSettings(GroupName = "Data Source")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManageDataSources")]
    [Route("/formBuilder/management/api/v1/tree/data-source")]
    public abstract class DataSourceTreeControllerBase : FormsManagementApiControllerBase
    {
    }
}