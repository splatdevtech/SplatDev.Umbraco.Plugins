using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Api.Management.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Core.Export
{
    [ApiExplorerSettings(GroupName = "Export")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/export")]
    public abstract class ExportControllerBase(IFormService formService) : FormsManagementApiControllerBase
    {
        protected IFormService FormService { get; } = formService;
    }
}