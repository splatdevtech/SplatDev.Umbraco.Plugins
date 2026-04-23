using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with the datasource wizard.
    /// </summary>
    [ApiExplorerSettings(GroupName = "Data Source")]
    [Route("/formBuilder/management/api/v1/datasource/wizard")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManageForms")]
    public abstract class DataSourceWizardControllerBase : FormsManagementApiControllerBase
    {
    }
}