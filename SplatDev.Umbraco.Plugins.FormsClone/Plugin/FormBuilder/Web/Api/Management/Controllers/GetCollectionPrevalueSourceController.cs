using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Api.Common.ViewModels.Pagination;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a collection of prevalue sources.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    public class GetCollectionPrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService) : PrevalueSourceControllerBase(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
        /// <summary>
        /// Management API endpoint for retrieving a collection of prevalue sources.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedViewModel<FieldPrevalueSource>), 200)]
        public IActionResult GetCollection(int skip = 0, int take = int.MaxValue)
        {
            List<FieldPrevalueSource> list = [.. PrevalueSourceService.Get().OrderBy(x => x.Name)];
            return Ok(new PagedViewModel<FieldPrevalueSource>()
            {
                Total = list.Count,
                Items = list.Skip(skip).Take(take)
            });
        }
    }
}