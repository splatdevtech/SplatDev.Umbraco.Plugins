using FormBuilder.Core.Models;

using Microsoft.AspNetCore.Components;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with pickers related to document types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Route("/formBuilder/management/api/v1/picker/document-type")]
    public abstract class DocumentTypePickerControllerBase(IContentTypeService contentTypeService) : PickerControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        public IContentTypeService ContentTypeService { get; } = contentTypeService;

        /// <summary>Gets all properties for the provided document type.</summary>
        protected static IList<PickerItem> GetPropertiesForDocumentType(
          IContentType docType)
        {
            List<PickerItem> source = [];
            foreach (PropertyType compositionPropertyType in docType.CompositionPropertyTypes.Cast<PropertyType>())
            {
                PickerItem pickerItem = new()
                {
                    Id = compositionPropertyType.Alias,
                    Value = compositionPropertyType.Name
                };
                if (!source.Contains(pickerItem))
                    source.Add(pickerItem);
            }
            return [.. source.OrderBy(x => x.Value)];
        }
    }
}