using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single prevalue source type by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetByKeyPrevalueSourceTypeController(
      FieldPrevalueSourceCollection prevalueSourceTypeCollection,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : PrevalueSourceTypeControllerBase(prevalueSourceTypeCollection, hostingEnvironment, formDesignSettings)
    {
        /// <summary>
        /// Management API controller for retrieving a single prevalue source type by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PrevalueSourceTypeWithSettings), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id)
        {
            FieldPrevalueSourceType? type = PrevalueSourceTypeCollection.SingleOrDefault(x => x.Id == id);
            return type is null ? NotFound() : Ok(CreatePrevalueSourceTypeWithSettings(type));
        }
    }
}