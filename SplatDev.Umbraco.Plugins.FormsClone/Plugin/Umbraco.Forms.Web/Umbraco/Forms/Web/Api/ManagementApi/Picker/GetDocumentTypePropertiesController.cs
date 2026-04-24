
// Type: Umbraco.Forms.Web.Api.ManagementApi.Picker.GetDocumentTypePropertiesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Picker
{
  public class GetDocumentTypePropertiesController : DocumentTypePickerControllerBase
  {
    public GetDocumentTypePropertiesController(IContentTypeService contentTypeService)
      : base(contentTypeService)
    {
    }

    [HttpGet("{alias}/properties")]
    [ProducesResponseType(typeof (IEnumerable<PickerItem>), 200)]
    [ProducesResponseType(404)]
    public IActionResult GetProperties(string alias)
    {
      IContentType docType = this.ContentTypeService.Get(alias);
      return docType == null ? (IActionResult) this.NotFound() : (IActionResult) this.Ok((object) DocumentTypePickerControllerBase.GetPropertiesForDocumentType(docType));
    }
  }
}
