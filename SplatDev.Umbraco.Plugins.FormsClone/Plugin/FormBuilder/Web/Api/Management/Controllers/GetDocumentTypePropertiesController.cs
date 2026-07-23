using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all properties of a document type.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetDocumentTypePropertiesController(IContentTypeService contentTypeService) : DocumentTypePickerControllerBase(contentTypeService)
    {
        /// <summary>
        /// Management API controller for retrieving all properties of a document type.
        /// </summary>
        [HttpGet("{alias}/properties")]
        [ProducesResponseType(typeof(IEnumerable<PickerItem>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetProperties(string alias)
        {
            IContentType? docType = ContentTypeService.Get(alias);
            return docType is null ? NotFound() : Ok(DocumentTypePickerControllerBase.GetPropertiesForDocumentType(docType));
        }
    }
}