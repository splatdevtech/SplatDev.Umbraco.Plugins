using FormBuilder.Core.Configuration;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single field type by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetByKeyFieldTypeController(
      IFieldTypeStorage fieldTypeStorage,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings,
      FieldCollection fieldCollection) : GetFieldTypeControllerBase(fieldTypeStorage, hostingEnvironment, formDesignSettings, fieldCollection)
    {
        /// <summary>
        /// Management API controller for retrieving a single field type by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(FieldTypeWithSettings), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id)
        {
            FieldType? fieldType = FieldCollection.SingleOrDefault(x => x.Id == id);
            return fieldType is null ? NotFound() : Ok(CreateFieldTypeWithSettings(fieldType));
        }
    }
}