using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Management API controller for retrieving form items.</summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Form")]
    [Authorize(Policy = "BackOfficeAccess")]
    [Route("/formBuilder/management/api/v1/item/form")]
    public class FormItemController(IFormService formService) : FormsManagementApiControllerBase
    {
        private readonly IFormService _formService = formService;

        /// <summary>Management API controller for retrieving form items.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormItemResponseModel>), 200)]
        public IActionResult Item([FromQuery(Name = "id")] HashSet<Guid> ids)
        {
            List<FormSlim> source = [];
            foreach (Guid id in ids)
            {
                FormSlim? slim = _formService.GetSlim(id);
                if (slim is not null)
                    source.Add(slim);
            }
            return Ok(source.Select(x =>
            {
                return new FormItemResponseModel()
                {
                    Id = x.Id,
                    Name = x.Name
                };
            }).ToList());
        }
    }
}