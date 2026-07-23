using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving all document types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetAllDocumentTypesController(IContentTypeService contentTypeService) : DocumentTypePickerControllerBase(contentTypeService)
    {
        /// <summary>
        /// Management API controller for retrieving all document types.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PickerItem>), 200)]
        public IActionResult GetAll()
        {
            List<PickerItem> source = [];
            foreach (IContentType contentType in ContentTypeService.GetAll().Where(x => !x.IsElement))
            {
                PickerItem pickerItem = new()
                {
                    Id = contentType.Alias,
                    Value = contentType.Name ?? string.Empty
                };
                source.Add(pickerItem);
            }
            return Ok(source.OrderBy(x => x.Value).ToList());
        }
    }
}