using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for updating the list of mapped properties for a document type mapper.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class UpdateMappedDocumentTypePropertiesController(IContentTypeService contentTypeService) : DocumentTypePickerControllerBase(contentTypeService)
    {
        /// <summary>
        /// Management API controller updating the list of mapped properties for a document type mapper.
        /// </summary>
        [HttpPost("mappings/refresh")]
        [ProducesResponseType(typeof(IEnumerable<MappedDocumentTypePropertyModel>), 200)]
        [ProducesResponseType(404)]
        public IActionResult RefreshMappedProperties(MappedDocumentTypeModel data)
        {
            IContentType? docType = ContentTypeService.Get(data.DoctypeAlias);
            if (docType is null)
                return NotFound();
            IList<PickerItem> propertiesForDocumentType = GetPropertiesForDocumentType(docType);
            List<MappedDocumentTypePropertyModel> source = [];
            foreach (MappedDocumentTypePropertyModel currentProperty1 in data.CurrentProperties)
            {
                MappedDocumentTypePropertyModel currentProperty = currentProperty1;
                PickerItem? pickerItem = propertiesForDocumentType.SingleOrDefault(x => x.Id == currentProperty.Id);
                if (pickerItem is not null)
                    source.Add(new MappedDocumentTypePropertyModel()
                    {
                        Id = currentProperty.Id,
                        Value = pickerItem.Value,
                        Field = currentProperty.Field,
                        StaticValue = currentProperty.StaticValue
                    });
            }
            foreach (PickerItem pickerItem in (IEnumerable<PickerItem>)propertiesForDocumentType)
            {
                PickerItem docTypeProperty = pickerItem;
                if (source.SingleOrDefault(x => x.Id == docTypeProperty.Id) is null)
                    source.Add(new MappedDocumentTypePropertyModel()
                    {
                        Id = docTypeProperty.Id,
                        Value = docTypeProperty.Value,
                        Field = string.Empty,
                        StaticValue = string.Empty
                    });
            }
            return Ok(source.OrderBy(x => x.Value).ToList());
        }
    }
}