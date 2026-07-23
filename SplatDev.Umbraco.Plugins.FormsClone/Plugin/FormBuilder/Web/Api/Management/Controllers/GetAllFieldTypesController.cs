using FormBuilder.Core.Configuration;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all field types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetAllFieldTypesController(
      IFieldTypeStorage fieldTypeStorage,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings,
      FieldCollection fieldCollection) : GetFieldTypeControllerBase(fieldTypeStorage, hostingEnvironment, formDesignSettings, fieldCollection)
    {
        /// <summary>
        /// Management API controller for retrieving all field types.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FieldTypeWithSettings>), 200)]
        public IActionResult GetAll()
        {
            List<FieldTypeWithSettings> source = [];
            foreach (var field in (BuilderCollectionBase<FieldType>)FieldCollection)
            {
                FieldTypeWithSettings typeWithSettings = CreateFieldTypeWithSettings(field);
                source.Add(typeWithSettings);
            }
            return Ok(source.OrderBy(x => x.Name));
        }
    }
}