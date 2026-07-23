using FormBuilder.Core.Models;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all form templates.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class GetAllFormTemplatesController(IFormTemplateStorage formTemplateStorage) : FormTemplateControllerBase(formTemplateStorage)
    {

        /// <summary>
        /// Management API controller for retrieving all form templates.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormTemplateBase>), 200)]
        public IActionResult GetAll() => Ok(FormTemplateStorage.GetAllTemplates());
    }
}